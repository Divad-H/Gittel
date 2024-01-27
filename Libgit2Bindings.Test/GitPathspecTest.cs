namespace Libgit2Bindings.Test;

public sealed class GitPathspecTest
{
  [Fact]
  public void CanMatchPath()
  {
    using var libgit2 = new Libgit2();
    using var pathspec = libgit2.NewGitPathspec(["foo/*/bar"]);
    Assert.True(pathspec.MatchesPath(GitPathspecFlags.Default, "foo/abc/bar"));
    Assert.False(pathspec.MatchesPath(GitPathspecFlags.Default, "foo/abc/baz"));
  }
}
