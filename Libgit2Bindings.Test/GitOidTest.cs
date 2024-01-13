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
    Assert.False(odb.Exists(new GitOid(Enumerable.Repeat((byte)0, 20).ToArray())));
  }

  [Fact]
  public void CanDetermineIfObjectExsistsWithOptionFlags()
  {
    using var repo = new RepoWithOneCommit();
    var oid = repo.CommitOid;
    using var odb = repo.Repo.GetOdb();
    Assert.True(odb.Exists(oid, GitOdbLookupFlags.NoRefresh));
    Assert.False(odb.Exists(new GitOid(Enumerable.Repeat((byte)0, 20).ToArray()), GitOdbLookupFlags.NoRefresh));
  }

  [Fact]
  public void CanDetermineIfObjectExsistsWithShortSha()
  {
    using var repo = new RepoWithOneCommit();
    var oid = repo.CommitOid;
    using var odb = repo.Repo.GetOdb();
    Assert.Equal(oid, odb.Exists(oid.Sha.Substring(0, 7)));
    Assert.Null(odb.Exists("0000000"));
  }
}
