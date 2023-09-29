using CppSharp;
using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Passes;

namespace Libgit2BindingGenerator;

internal class Libgit2Library : ILibrary
{
  public void Postprocess(Driver driver, ASTContext ctx)
  {
  }

  public void Preprocess(Driver driver, ASTContext ctx)
  {
    ctx.GenerateEnumFromMacros("StructVersion", "GIT_STATUS_OPTIONS_VERSION");
    ctx.GenerateEnumFromMacros("PathListSeparator", "GIT_PATH_LIST_SEPARATOR");
    ctx.GenerateEnumFromMacros("GitCheckoutOptionsVersion", "GIT_CHECKOUT_OPTIONS_VERSION");
  }

  public void Setup(Driver driver)
  {
    var options = driver.Options;
    options.GeneratorKind = GeneratorKind.CSharp;
    var module = options.AddModule("libgit2");
    module.IncludeDirs.Add(@"..\..\..\..\libgit2\include\");
    module.Headers.Add(@"git2.h");

    module.LibraryDirs.Add(@"..\..\..\..\libgit2\build\Debug");
    module.Libraries.Add("git2.lib");

    options.OutputDir = @"..\..\..\..\Libgit2Bindings\Generated";
  }

  public void SetupPasses(Driver driver)
  {
    driver.Context.GeneratorOutputPasses.AddPass(new FixOutVariableUsagePass());
    driver.Context.TranslationUnitPasses.AddPass(new FixOutVariablePass());
  }
}

internal class FixOutVariableUsagePass : GeneratorOutputPass
{
  public override void HandleBlock(Block block)
  {
    var text = block.Text.ToString();

    if (IsFunctionWithOutVariable(text))
    {
      List<string> res = FixOutFuncionImplementation(text);
      ReplaceBlock(block, res);
    }

    if (IsFunctionWithAllocHGlobal(text))
    {
      List<string> res = FixFunctionWithAllocHGlobal(text);
      ReplaceBlock(block, res);
    }

    base.HandleBlock(block);
  }

  private List<string> FixFunctionWithAllocHGlobal(string text)
  {
    var lines = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
    List<string> res = new();

    for (int i = 0; i < lines.Length; i++)
    {
      res.Add(lines[i]);

      if (lines[i].Contains("Marshal.AllocHGlobal"))
      {
        if (i + 1 < lines.Length && lines[i + 1].Contains("ownsNativeInstance"))
        {
          var varName = lines[i].Trim().Split(' ').First();
          var sizeExpression = lines[i].Substring(lines[i].IndexOf('(') + 1, lines[i].IndexOf(')') - lines[i].IndexOf('('));
          res.Add($"            System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned((void*){varName}, 0, (uint){sizeExpression});");
        }
      }
    }
    return res;
  }

  private bool IsFunctionWithAllocHGlobal(string text)
  {
    return text.Contains("Marshal.AllocHGlobal");
  }

  private static List<string> FixOutFuncionImplementation(string text)
  {
    var lines = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
    List<string> res = new();
    List<string> outLines = new();
    List<int> outArgNumbers = new();

    for (int i = 0; i < lines.Length; i++)
    {
      if (IsConstructOutObjectLine(lines[i]))
      {
        outLines.Add(lines[i]);
      }
      else
      {
        lines[i] = FixOutPointerCreation(lines[i], outArgNumbers);

        if (IsReturnStatementLine(lines[i]))
        {
          InsertOutObjectConstructions(res, outLines, outArgNumbers);
        }

        res.Add(lines[i]);
      }
    }

    return res;
  }

  private static void ReplaceBlock(Block block, List<string> lines)
  {
    block.Text.StringBuilder.Clear();
    block.Text.Unindent();
    block.Text.Unindent();
    foreach (var line in lines)
    {
      block.Text.WriteLine(line);
    }
  }

  private static void InsertOutObjectConstructions(List<string> res, List<string> outLines, List<int> outArgNumbers)
  {
    int argNum = 0;
    foreach (var ol in outLines)
    {
      var outLine = ol.Replace(" new ", " ");
      var lastBracketsIndex = outLine.LastIndexOf("()");
      if (lastBracketsIndex > 0)
      {
        outLine = outLine.Insert(lastBracketsIndex + 1, $"____arg{outArgNumbers[argNum++]}");
        outLine = outLine.Insert(lastBracketsIndex, ".__CreateInstance");
      }
      res.Add(outLine!);
    }
  }

  private static bool IsReturnStatementLine(string line)
  {
    return line.Trim().StartsWith("return");
  }

  private static bool IsConstructOutObjectLine(string line)
  {
    return line.Contains(" = new ") && !line.Contains("__IntPtr");
  }

  private static bool IsFunctionWithOutVariable(string text)
  {
    return (text.Contains("(out ", StringComparison.OrdinalIgnoreCase)
         || text.Contains(" out ", StringComparison.OrdinalIgnoreCase))
         && text.Contains("____arg", StringComparison.OrdinalIgnoreCase);
  }

  private static string FixOutPointerCreation(string line, List<int> outArgNumbers)
  {
    var assignArgIndex = line.IndexOf("var ____arg");
    if (assignArgIndex >= 0)
    {
      var argNumIndex = assignArgIndex + "var ____arg".Length;
      outArgNumbers.Add(int.Parse(line.Substring(argNumIndex, 2)));

      return line.Substring(0, argNumIndex + 2) + "= __IntPtr.Zero;";
    }
    return line;
  }
}

internal class FixOutVariablePass : TranslationUnitPass
{
  public override bool VisitFunctionDecl(Function function)
  {
    foreach (var parameter in function.Parameters)
    {
      if (parameter.Type.GetPointee()?.GetPointee() is not null || parameter.Name.Equals("out"))
      {
        parameter.Usage = ParameterUsage.Out;
      }
    }

    return base.VisitFunctionDecl(function);
  }
}
