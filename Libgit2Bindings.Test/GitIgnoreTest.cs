using Libgit2Bindings.Test.TestData;

namespace Libgit2Bindings.Test;

public sealed class GitIgnoreTest
{
  [Fact]
  public void CanAddIgnoreRuleAndClearRules()
  {
    using var repo = new EmptyRepo();
    Assert.False(repo.Repo.IgnorePathIsIgnored("test.txt"));
    repo.Repo.AddIgnoreRule("*.txt");
    Assert.True(repo.Repo.IgnorePathIsIgnored("test.txt"));
    repo.Repo.ClearInternalIgnoreRules();
    Assert.False(repo.Repo.IgnorePathIsIgnored("test.txt"));
  }
}
