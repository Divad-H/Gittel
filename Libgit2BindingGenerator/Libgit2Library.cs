using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;

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
  }
}
