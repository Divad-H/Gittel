using Libgit2Bindings.Test.TestData;

namespace Libgit2Bindings.Test;

public class GitDescribeTest
{
  [Fact]
  public void CanDescribeCommit()
  {
    using var repo = new RepoWithOneCommit();
    var commit = repo.Repo.LookupCommit(repo.CommitOid);
    var options = new GitDescribeOptions
    {
      DescribeStrategy = GitDescribeStrategy.All,
    };
    using var commitObject = repo.Repo.LookupObject(repo.CommitOid, GitObjectType.Commit);
    using var description = commitObject.DescribeCommit(options);
    var text = description.Format();
    Assert.Contains("master", text);
  }

  [Fact]
  public void CanDescribeCommitFromWorkdir()
  {
    using var repo = new RepoWithOneCommit();
    using var description = repo.Repo.DescribeWorkdir(new()
    {
      DescribeStrategy = GitDescribeStrategy.All,
    });
    var text = description.Format(new()
    {
      AlwaysUseLongFormat = true,
    });
    Assert.Contains("master", text);
  }
}
