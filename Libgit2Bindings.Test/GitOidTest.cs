using Libgit2Bindings.Test.TestData;

namespace Libgit2Bindings.Test;

public class GitOidTest
{
  [Fact]
  public void CanDetermineIfObjectExsists()
  {
    using var repo = new RepoWithOneCommit();
    var oid = repo.CommitOid;
    using var odb = repo.Repo.GetOdb();
    Assert.True(odb.Exists(oid));
    Assert.False(odb.Exists(new(Enumerable.Repeat((byte)0, 20).ToArray())));
  }
}
