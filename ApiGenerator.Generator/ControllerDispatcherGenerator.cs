using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

    var methodDeclarations = controllerDeclarations.SelectMany((o, ct) =>
    {
      return o.classDeclarationSyntax.DescendantNodes()
        .OfType<MethodDeclarationSyntax>()
        .Where(m => m.Modifiers.Where(mod => mod.IsKind(SyntaxKind.PublicKeyword)).Any())
        .Select(methodDeclaration => (methodDeclaration, o.SemanticModel));
    });

    var responseDtoDeclarations = methodDeclarations
      .Combine(taskTypes)
      .Select((o, ct) =>
      {
        var returnType = o.Left.methodDeclaration.ReturnType;
        var typeInfo = o.Left.SemanticModel.GetTypeInfo(returnType, ct);
        
        var typeParams = (returnType as GenericNameSyntax)?.TypeArgumentList.Arguments;
        var name = (typeInfo.Type as INamedTypeSymbol)?.Name;

        if ( $"{typeInfo.Type?.ContainingNamespace}.{typeInfo.Type?.Name}" == "System.Threading.Tasks.Task")
        {
          if (typeParams?.FirstOrDefault() is TypeSyntax dtoTypeSyntax)
          {
            var ti = o.Left.SemanticModel.GetTypeInfo(dtoTypeSyntax);
            return $"{ti.Type?.ContainingNamespace}.{ti.Type?.Name}";
          }
        }
        return null;
      })
      .Where(s => s is not null)
      .Collect();

    context.RegisterSourceOutput(responseDtoDeclarations,
      (ctx, dtoTypeNames) =>
      {
        ctx.AddSource($"DtosGenerationSpec.cs", $@"
using TypeGen.Core.SpecGeneration;

namespace ApiGeneratrion.Generated;

public partial class DtosGenerationSpec : GenerationSpec
{{
  public DtosGenerationSpec()
  {{
    { string.Join( "\n", dtoTypeNames.Select(n => $"AddInterface<{ n }>();")) }
  }}
}}");
      });
  }
}
