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

    if ((text.Contains("(out ", StringComparison.OrdinalIgnoreCase)
      || text.Contains(" out ", StringComparison.OrdinalIgnoreCase))
      && text.Contains("____arg", StringComparison.OrdinalIgnoreCase))
    {
      var lines = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      var res = new List<string>();
      List<string> outLines = new();
      List<int> outArgNumbers = new();

      for (int i = 0; i < lines.Length; i++)
      {
        if (lines[i].Contains(" = new ") && !lines[i].Contains("__IntPtr"))
        {
          outLines.Add(lines[i]);
        }
        else
        {
          var assignArgIndex = lines[i].IndexOf("var ____arg");
          if (assignArgIndex >= 0)
          {
            var argNumIndex = assignArgIndex + "var ____arg".Length;
            outArgNumbers.Add(int.Parse(lines[i].Substring(argNumIndex, 2)));

            lines[i] = lines[i].Substring(0, argNumIndex + 2) + "= __IntPtr.Zero;";
          }

          if (lines[i].Trim().StartsWith("return"))
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
          res.Add(lines[i]);
        }
      }

      block.Text.StringBuilder.Clear();
      block.Text.Unindent();
      block.Text.Unindent();
      foreach (var line in res)
      {

        block.Text.WriteLine(line);
      }
    }

    base.HandleBlock(block);
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
