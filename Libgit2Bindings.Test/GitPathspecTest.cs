using Libgit2Bindings.Test.TestData;

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

  private void RunTest(
    Func<IGitPathspec, RepoWithOneCommit, GitPathspecFlags, IGitPathspecMathList> matchesFunc)
  {
    using var repo = new RepoWithOneCommit();
    using var pathspec = repo.Libgit2.NewGitPathspec(["test.*"]);

    using var matches = matchesFunc(pathspec, repo, GitPathspecFlags.Default);
    Assert.Equal((nuint)1, matches.Entrycount);
    Assert.Equal("test.txt", matches.Entry(0));

    using var failedPathspec = repo.Libgit2.NewGitPathspec(["foo"]);
    using var failedMatches = matchesFunc(failedPathspec, repo, GitPathspecFlags.FindFailures);
    Assert.Equal((nuint)0, failedMatches.Entrycount);
    Assert.Equal((nuint)1, failedMatches.FailedEntrycount);
    Assert.Equal("foo", failedMatches.FailedEntry(0));
  }

  [Fact]
  public void CanMatchWorkdir()
  {
    RunTest((pathspec, repo, flags) => pathspec.MatchWorkdir(repo.Repo, flags));
  }

  [Fact]
  public void CanMatchIndex()
  {
    RunTest(
      (pathspec, repo, flags) =>
      {
        using var index = repo.Repo.GetIndex();
        return pathspec.MatchIndex(index, flags);
      });
  }

  [Fact]
  public void CanMatchTree()
  {
    RunTest((pathspec, repo, flags) => pathspec.MatchTree(repo.Tree, flags));
  }
}
