using Libgit2Bindings.Test.TestData;

namespace Libgit2Bindings.Test;

public class GitGraphTest
{
  [Fact]
  public void CanCountCommitsAheadBehind()
  {
    using var repo = new RepoWithTwoCommits();
    
    var aheadBehind = repo.Repo.GraphAheadBehind(repo.FirstCommitOid, repo.SecondCommitOid);
    Assert.Equal(0ul, aheadBehind.Ahead);
    Assert.Equal(1ul, aheadBehind.Behind);
  }
}
