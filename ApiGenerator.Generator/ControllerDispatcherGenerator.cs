using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;

namespace ApiGenerator.Generator;

[Generator]
public class ControllerDispatcherGenerator : IIncrementalGenerator
{
  private const string GenerateControllerAttribute = "ApiGenerator.Attributes.GenerateControllerAttribute";

  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
#if DEBUG
    if (!Debugger.IsAttached)
    {
      //Debugger.Launch();
    }
#endif 

    Debug.WriteLine("Execute code generator");

    var taskTypes = context.CompilationProvider.Select((c, ct) => (TaskGeneric: c.GetTypeByMetadataName("System.Task`1"), Task: c.GetTypeByMetadataName("System.Task")));

    var controllerDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
      static (node, ct) => node is ClassDeclarationSyntax,
      static (ctx, ct) =>
      {
        var classDeclarationSyntax = (ClassDeclarationSyntax)ctx.Node;

        foreach (AttributeListSyntax attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
          foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
          {
            if (ctx.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
            {
              continue;
            }

            INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
            string fullName = attributeContainingTypeSymbol.ToDisplayString();

            if (fullName == GenerateControllerAttribute)
            {
              return (classDeclarationSyntax, ctx.SemanticModel);
            }
          }
        }
        return (null, null)!;
      })
      .Where(static c => c.classDeclarationSyntax is not null && c.SemanticModel is not null);

    var methodDeclarations = controllerDeclarations.SelectMany(static (o, ct) =>
    {
      return o.classDeclarationSyntax.DescendantNodes()
        .OfType<MethodDeclarationSyntax>()
        .Where(m => m.Modifiers.Where(mod => mod.IsKind(SyntaxKind.PublicKeyword)).Any())
        .Select(methodDeclaration => (methodDeclaration, o.SemanticModel, o.classDeclarationSyntax));
    });

    var responseDtoDeclarations = methodDeclarations
      .Select(static (o, ct) =>
      {
        var returnType = o.methodDeclaration.ReturnType;
        var typeInfo = o.SemanticModel.GetTypeInfo(returnType, ct);
        
        var typeParams = (returnType as GenericNameSyntax)?.TypeArgumentList.Arguments;
        var name = (typeInfo.Type as INamedTypeSymbol)?.Name;

        if ( $"{typeInfo.Type?.ContainingNamespace}.{typeInfo.Type?.Name}" == "System.Threading.Tasks.Task")
        {
          if (typeParams?.FirstOrDefault() is TypeSyntax dtoTypeSyntax)
          {
            var ti = o.SemanticModel.GetTypeInfo(dtoTypeSyntax, ct);
            return $"{ti.Type?.ContainingNamespace}.{ti.Type?.Name}";
          }
        }
        return null;
      })
      .Where(static s => s is not null)
      .Collect();

    var requestDtoDeclarations = methodDeclarations
      .SelectMany(static (o, ct) =>
      {
        var parameters = o.methodDeclaration.ParameterList.Parameters;

        return parameters
          .Select(p =>
          {
            if (p.Type is null)
            {
              return null;
            }
            var typeInfo = o.SemanticModel.GetTypeInfo(p.Type, ct);

            var name = $"{typeInfo.Type?.ContainingNamespace}.{typeInfo.Type?.Name}";
                        
            if (name == "System.Threading.CancellationToken")
            {
              return null;
            }

            return name;
          });
      })
      .Where(s => s is not null)
      .Collect();

    var dtos = responseDtoDeclarations
      .Combine(requestDtoDeclarations)
      .Select((o, ct) => o.Left.Concat(o.Right).Distinct().ToImmutableArray());

    context.RegisterSourceOutput(dtos,
      static (ctx, dtoTypeNames) =>
      {
        ctx.AddSource("DtosGenerationSpec.cs", $@"
using TypeGen.Core.SpecGeneration;

namespace ApiGeneration.Generated;

public partial class DtosGenerationSpec : GenerationSpec
{{
  public DtosGenerationSpec()
  {{
    AddInterface<ApiGenerator.RequestDto>();
    AddInterface<ApiGenerator.ResponseDto>();
    { string.Join( "\n", dtoTypeNames.Select(n => $"AddInterface<{ n }>();")) }
  }}
}}");
      });

    var endPoints = methodDeclarations
      .Select(static (o, ct) =>
      {
        var controllerSymbol = o.SemanticModel.GetDeclaredSymbol(o.classDeclarationSyntax, ct);
        var controllerName = controllerSymbol?.Name;
        var controllerFullName = controllerSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var methodName = o.SemanticModel.GetDeclaredSymbol(o.methodDeclaration, ct)?.Name;

        var parameters = o.methodDeclaration.ParameterList.Parameters
          .Select(p =>
          {
            if (p.Type is null)
            {
              return null;
            }
            var typeInfo = o.SemanticModel.GetTypeInfo(p.Type, ct);

            return $"{typeInfo.Type?.ContainingNamespace}.{typeInfo.Type?.Name}";
          })
          .Where(p => p is not null)
          .ToImmutableArray();

        var hasReturnType = (o.methodDeclaration.ReturnType as GenericNameSyntax)?.TypeArgumentList.Arguments.Any() ?? false;

        return (controllerName, controllerFullName, methodName, parameters, hasReturnType);
      })
      .Where(static o => o.controllerName is not null && o.methodName is not null)
      .Collect()
      .Select(static (os, ct) => os.GroupBy(static o => o.controllerName).ToImmutableArray());

    context.RegisterSourceOutput(endPoints,
      (ctx, eps) =>
      {
        string getControllerWireName(string controllerName)
        {
          if (controllerName.EndsWith("Controller"))
            return controllerName.Substring(0, controllerName.Length - "Controller".Length);
          return controllerName;
        }

        string getControllerArgName(string controllerName)
        {
          return char.ToLowerInvariant(controllerName![0]) + controllerName.Substring(1);
        }

        string getControllerMemberName(string controllerName)
        {
          return "_" + getControllerArgName(controllerName);
        }

        ctx.AddSource("RequestDispatchImpl.cs", $@"namespace ApiGeneration.Generated;

public partial class RequestDispatcherImpl : ApiGenerator.IRequestDispatcherImpl
{{
  { string.Join("\n", eps.Select(g => @$"private readonly { g.First().controllerFullName } { getControllerMemberName(g.Key!) };")) }

  public RequestDispatcherImpl({string.Join(",", eps.Select(g => @$"{ g.First().controllerFullName } { getControllerArgName(g.Key!) }"))})
  {{
    { string.Join("\n", eps.Select(g => @$"{getControllerMemberName(g.Key!)} = {getControllerArgName(g.Key!)};")) }
  }}

  async Task<string?> ApiGenerator.IRequestDispatcherImpl.DispatchRequest(ApiGenerator.RequestDto request, System.Text.Json.JsonSerializerOptions jsonSerializerOptions, CancellationToken ct)
  {{
    return request.Controller switch
    {{
      { string.Join("\n", eps.Select(g => @$"""{getControllerWireName(g.Key!)}"" => request.Function switch
      {{
        { string.Join("\n", g.Select(o => @$"""{ o.methodName }"" => { (o.hasReturnType 
          ? $"System.Text.Json.JsonSerializer.Serialize(await { getControllerMemberName(g.Key!) }.{ o.methodName }( {
              string.Join(", ", o.parameters.Select(p => p == "System.Threading.CancellationToken" ? "ct" : $"System.Text.Json.JsonSerializer.Deserialize<{ p }>(request.Data ?? throw new System.ArgumentNullException(), jsonSerializerOptions) ?? throw new System.InvalidOperationException()"))
            }), jsonSerializerOptions)" 
          : "TODO (Return type is Task)") } ,")) }
        _ => throw new InvalidOperationException(""Function not found."")
      }},"))}
      _ => throw new InvalidOperationException(""Controller not found."")
    }};
  }}
}}
          ");
      });
  }
}
