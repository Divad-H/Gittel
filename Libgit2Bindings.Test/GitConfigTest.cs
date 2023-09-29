namespace Libgit2Bindings.Test;

public class GitConfigTest
{
  [Fact]
  public void CanFindGlobalGitConfig()
  {
    using var libgit2 = new Libgit2();

    var config = libgit2.FindGlobalConfig();

    Assert.NotNull(config);
    Assert.NotEmpty(config);
  }
}
