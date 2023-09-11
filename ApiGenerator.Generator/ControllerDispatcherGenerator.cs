using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

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

    var tgconfig = context
      .AdditionalTextsProvider
      .Where(text => text.Path.EndsWith("tgconfig.json",
                      StringComparison.OrdinalIgnoreCase))
      .Select((text, token) => text.GetText(token)?.ToString())
      .Where(text => text is not null)
      .Select((jsonString, ct) => 
      {
        var outputPathIndex = jsonString!.IndexOf("outputPath") + "outputPath".Length;
        if (outputPathIndex < 0)
        {
          return null;
        }
        jsonString = jsonString.Substring(outputPathIndex + 1);
        var colonIndex = jsonString.IndexOf(":");
        if (colonIndex < 0)
        {
          return null;
        }
        jsonString = jsonString.Substring(colonIndex + 1);
        var singleQuoteIndex = jsonString.IndexOf('\'');
        var doubleQuotesIndex = jsonString.IndexOf('"');
        var useSingleQuote = singleQuoteIndex >= 0 && (doubleQuotesIndex < 0 || doubleQuotesIndex > singleQuoteIndex);
        var quotesIndex = useSingleQuote ? singleQuoteIndex : doubleQuotesIndex;
        if (quotesIndex < 0)
        {
          return null;
        }
        jsonString = jsonString.Substring(quotesIndex + 1);
        quotesIndex = jsonString.IndexOf(useSingleQuote ? '\'' : '"');
        if (quotesIndex < 0)
        {
          return null;
        }
        return jsonString.Substring(0, quotesIndex);
      })
      .Where(config => config is not null)
      .Collect()
      .Select((cs, ct) => cs.FirstOrDefault());

    var tsOutDir = context
      .AdditionalTextsProvider
      .Where(text => text.Path.EndsWith("project-dir.txt",
                      StringComparison.OrdinalIgnoreCase))
      .Select((text, token) => text.GetText(token)?.ToString().Split('\r', '\n').FirstOrDefault())
      .Where(text => text is not null)
      .Collect()
      .Select((cs, ct) => cs.FirstOrDefault())
      .Combine(tgconfig)
      .Select((o, ct) => Path.Combine(o.Left, o.Right));

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

        if ( $"{typeInfo.Type?.ContainingNamespace}.{typeInfo.Type?.Name}" == "System.Threading.Tasks.Task"
          || $"{typeInfo.Type?.ContainingNamespace}.{typeInfo.Type?.Name}" == "System.IObservable")
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
      .Combine(tsOutDir)
      .Select((o, ct) => (o.Left.methodDeclaration, o.Left.classDeclarationSyntax, o.Left.SemanticModel, tsOutDir: o.Right))
      .Select(static (o, ct) =>
      {
        var controllerSymbol = o.SemanticModel.GetDeclaredSymbol(o.classDeclarationSyntax, ct);
        var controllerName = controllerSymbol?.Name;
        var controllerFullName = controllerSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var methodName = o.SemanticModel.GetDeclaredSymbol(o.methodDeclaration, ct)?.Name;
        var returnType = o.methodDeclaration.ReturnType;

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

        var hasReturnType = (returnType as GenericNameSyntax)?.TypeArgumentList.Arguments.Any() ?? false;
        var typeParams = (returnType as GenericNameSyntax)?.TypeArgumentList.Arguments;
        string? returnTypeDto = null;
        if (typeParams?.FirstOrDefault() is TypeSyntax dtoTypeSyntax)
        {
          var ti = o.SemanticModel.GetTypeInfo(dtoTypeSyntax, ct);
          returnTypeDto = ti.Type?.Name;
        }

        var typeInfo = o.SemanticModel.GetTypeInfo(returnType, ct);
        var name = (typeInfo.Type as INamedTypeSymbol)?.Name;
        var isFunction = $"{typeInfo.Type?.ContainingNamespace}.{typeInfo.Type?.Name}" == "System.Threading.Tasks.Task";

        return (controllerName, controllerFullName, methodName, parameters, hasReturnType, returnTypeDto, o.tsOutDir, isFunction);
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

        string toCamelCase(string pascalCase)
        {
          return char.ToLowerInvariant(pascalCase[0]) + pascalCase.Substring(1);
        }

        string toHyphenCase(string camelCase)
        {
          return Regex.Replace(camelCase, "(?<!^)([A-Z])", "-$1").ToLower();
        }

        foreach (var g in eps)
        {
          var fileName = Path.Combine(g.First().tsOutDir, $@"{toHyphenCase(getControllerWireName(g.Key!))}-client.ts");

          var code = $@"import {{ Injectable }} from ""@angular/core"";
import {{ Observable }} from 'rxjs';
import {{ MessageService }} from '../services/message.service';

{ string.Join("\n", g.Select(e => e.returnTypeDto).Where(t => t is not null).Distinct().Select(t => @$"import {{ { t } }} from ""./{toHyphenCase(t!) }""")) }
{ string.Join("\n", g.SelectMany(e => e.parameters).Where(p => p is not null && p != "System.Threading.CancellationToken").Distinct().Select(p => @$"import {{ { p!.Split('.').Last() } }} from ""./{toHyphenCase(p!.Split('.').Last())}""")) }

@Injectable()
export class {getControllerWireName(g.Key!)}Client {{

  constructor(private readonly messageService: MessageService) {{
  }}

  { string.Join("\n\n  ", g.Select(o => $@"public { toCamelCase(o.methodName!) }({ 
    string.Join(", ", o.parameters.Where(p => p != "System.Threading.CancellationToken").Select((p, i) => $"data{i}: {p!.Split('.').Last()}"))
  }) : Observable<{ (o.hasReturnType ? o.returnTypeDto : "null") }> {{
    return this.messageService.{(o.isFunction ? "callNativeFunction" : "callNativeSubscribe")}(""{ getControllerWireName(g.Key!) }"", ""{ o.methodName }"", [{ string.Join(", ", o.parameters.Where(p => p != "System.Threading.CancellationToken").Select((p, i) => $"data{i}")) }]);
  }}")) }
}}";
#pragma warning disable RS1035 // Do not use APIs banned for analyzers
          try
          {
            File.WriteAllText(fileName, code);
          }
          catch { }
#pragma warning restore RS1035 // Do not use APIs banned for analyzers
        }

        ctx.AddSource("RequestDispatchImpl.cs", $@"namespace ApiGeneration.Generated;

public partial class RequestDispatcherImpl : global::ApiGenerator.IRequestDispatcherImpl
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
    return (string?)null;
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
      { string.Join("\n      ", eps.Select(g => @$"""{getControllerWireName(g.Key!)}"" => request.Function switch
      {{
        { string.Join("\n        ", g.Where(o => o.isFunction).Select(o => @$"""{ o.methodName }"" => await ExecuteOn{ g.Key }(async controller => { (o.hasReturnType 
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

  global::System.IObservable<string?> ApiGenerator.IRequestDispatcherImpl.DispatchObservableRequest(ApiGenerator.RequestDto request, global::System.Text.Json.JsonSerializerOptions jsonSerializerOptions, global::System.IServiceProvider serviceProvider)
  {{
    return request.Controller switch
    {{
      {string.Join("\n      ", eps.Select(g => @$"""{getControllerWireName(g.Key!)}"" => request.Function switch
      {{
        {string.Join("\n        ", g.Where(o => !o.isFunction).Select(o => @$"""{o.methodName}"" => global::System.Reactive.Linq.Observable.Select(global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<{o.controllerFullName}>(serviceProvider).{o.methodName}({
          string.Join(", ", o.parameters.Select((p, i) => $"global::System.Text.Json.JsonSerializer.Deserialize<{p}>(request.Data[{i}] ?? throw new global::System.ArgumentNullException(), jsonSerializerOptions) ?? throw new global::System.InvalidOperationException()"))
        }), eventRes => {$"global::System.Text.Json.JsonSerializer.Serialize(eventRes, jsonSerializerOptions)"}) ,"))}
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
