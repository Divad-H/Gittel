using CppSharp;
using CppSharp.AST;
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

    if (text.Contains("@out", StringComparison.OrdinalIgnoreCase) && text.Contains("____arg0", StringComparison.OrdinalIgnoreCase))
    {
      var lines = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      var res = new List<string>();
      string? outLine = null;

      for (int i = 0; i < lines.Length; i++)
      {
        if (lines[i].Trim().StartsWith("@out"))
        {
          outLine = lines[i];
        }
        else
        {
          var assignArgIndex = lines[i].IndexOf("var ____arg0 = ");
          if (assignArgIndex >= 0)
          {
            lines[i] = lines[i].Substring(0, assignArgIndex) + "var ____arg0 = __IntPtr.Zero;";
          }

          if (lines[i].Trim().StartsWith("return"))
          {
            if (outLine is not null)
            {
              outLine = outLine.Replace(" new ", " ");
              var lastBracketsIndex = outLine.LastIndexOf("()");
              if (lastBracketsIndex > 0)
              {
                outLine = outLine.Insert(lastBracketsIndex + 1, "____arg0");
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
      if (parameter.Name.Equals("out"))
      {
        parameter.Usage = ParameterUsage.Out;
      }
    }

    return base.VisitFunctionDecl(function);
  }
}
