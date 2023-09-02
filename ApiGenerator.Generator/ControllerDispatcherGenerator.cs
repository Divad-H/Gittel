using Microsoft.CodeAnalysis;
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
      Debugger.Launch();
    }
#endif 

    Debug.WriteLine("Execute code generator");

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
              return classDeclarationSyntax;
            }
          }
        }
        return null;
      })
      .Where(static c => c is not null)!;

    context.RegisterSourceOutput(controllerDeclarations,
      (ctx, controllerDeclaration) =>
      {
        ctx.AddSource($"MySource.cs", $@"
    public static partial class TestClass
    {{
        public const string Test = ""MoreTest"";
    }}");
      });
  }
}
