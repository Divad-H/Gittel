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

        ctx.AddSource("RequestDispatchImpl.cs", $@"namespace ApiGeneration.Generated;

public class RequestDispatcherImpl : global::ApiGenerator.IRequestDispatcherImpl
{{
  public static void RegisterService(global::Microsoft.Extensions.DependencyInjection.IServiceCollection serviceCollection)
  {{
    global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<global::ApiGenerator.IRequestDispatcherImpl, RequestDispatcherImpl>(serviceCollection);
  }}

  private readonly global::System.IServiceProvider _serviceProvider;

  public RequestDispatcherImpl(global::System.IServiceProvider serviceProvider)
  {{
    _serviceProvider = serviceProvider;
  }}

  private static async Task<string?> ToNull(Func<Task> f)
  {{
    await f();
    return (string)null;
  }}

  { string.Join("\n\n", eps.Select(g => $@"private Task<string?> ExecuteOn{ g.Key }(global::System.Func<{ g.First().controllerFullName }, Task<string?>> f)
  {{
    using var scope = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.CreateScope(_serviceProvider);
    var controller = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<{g.First().controllerFullName}>(scope.ServiceProvider);
    return f(controller);
  }}")) }

  async Task<string?> ApiGenerator.IRequestDispatcherImpl.DispatchRequest(ApiGenerator.RequestDto request, global::System.Text.Json.JsonSerializerOptions jsonSerializerOptions, CancellationToken ct)
  {{
    return request.Controller switch
    {{
      { string.Join("      \n", eps.Select(g => @$"""{getControllerWireName(g.Key!)}"" => request.Function switch
      {{
        { string.Join("\n", g.Select(o => @$"""{ o.methodName }"" => await ExecuteOn{ g.Key }(async controller => { (o.hasReturnType 
          ? $"global::System.Text.Json.JsonSerializer.Serialize(await controller.{ o.methodName }( {
              string.Join(", ", o.parameters.Select((p, i) => p == "System.Threading.CancellationToken" ? "ct" : $"global::System.Text.Json.JsonSerializer.Deserialize<{ p }>(request.Data[{i}] ?? throw new global::System.ArgumentNullException(), jsonSerializerOptions) ?? throw new global::System.InvalidOperationException()"))
            }), jsonSerializerOptions)" 
          : $"await ToNull(async () => await controller.{ o.methodName }( {
              string.Join(", ", o.parameters.Select((p, i) => p == "System.Threading.CancellationToken" ? "ct" : $"global::System.Text.Json.JsonSerializer.Deserialize<{ p }>(request.Data[{i}] ?? throw new global::System.ArgumentNullException(), jsonSerializerOptions) ?? throw new global::System.InvalidOperationException()"))
            }))") }) ,")) }
        _ => throw new global::System.InvalidOperationException(""Function not found."")
      }},"))}
      _ => throw new global::System.InvalidOperationException(""Controller not found."")
    }};
  }}
}}
          ");
      });
  }
}
