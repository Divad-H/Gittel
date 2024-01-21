using Libgit2Bindings.Test.TestData;
using System.Text;

namespace Libgit2Bindings.Test;

public class GitOdbTest
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

  [Fact]
  public void CanExpandIds()
  {
    using var repo = new RepoWithOneCommit();
    var oid = repo.CommitOid;
    using var odb = repo.Repo.GetOdb();
    (string shortId, GitObjectType type)[] ids = [
      (oid.Sha.Substring(0, 7), GitObjectType.Any),
      ("1234567", GitObjectType.Commit),
    ];
    var expandedIds = odb.ExpandIds(ids);
    Assert.Equal(2, expandedIds.Count);
    Assert.Equal(oid, expandedIds[0].oid);
    Assert.Equal(GitObjectType.Commit, expandedIds[0].type);
    Assert.Equal(new GitOid(Enumerable.Repeat((byte)0, 20).ToArray()), expandedIds[1].oid);
  }

  [Fact]
  public void CanIterateObjects()
  {
    using var repo = new RepoWithOneCommit();
    using var odb = repo.Repo.GetOdb();
    var oids = new List<GitOid>();
    odb.ForEachOid(oid =>
    {
      oids.Add(oid);
      return GitOperationContinuation.Continue;
    });
    Assert.Equal(3, oids.Count);
    Assert.Contains(repo.CommitOid, oids);
    Assert.Contains(repo.TreeOid, oids);
  }

  [Fact]
  public void CanGetNumBackends()
  {
    using var repo = new RepoWithOneCommit();
    using var odb = repo.Repo.GetOdb();
    Assert.Equal(2u, odb.GetNumBackends());
  }

  [Fact]
  public void CanGetBackend()
  {
    using var repo = new RepoWithOneCommit();
    using var odb = repo.Repo.GetOdb();
    using var backend = odb.GetBackend(0);
    Assert.NotNull(backend);
  }

  [Fact]
  public void CanAddAlternativeOnDisk()
  {
    using var repo = new EmptyRepo();
    using var otherRepoWithObjects = new RepoWithOneCommit();

    using var odb = repo.Repo.GetOdb();
    odb.AddAlternativeOnDisk(Path.Combine(otherRepoWithObjects.TempDirectory.DirectoryPath, ".git", "objects"));

    Assert.Equal(4u, odb.GetNumBackends());
    odb.Exists(otherRepoWithObjects.CommitOid);
  }

  [Fact]
  public void CanReadOdbObject()
  {
    using var repo = new RepoWithOneCommit();
    using var odb = repo.Repo.GetOdb();
    using var obj = odb.Read(repo.CommitOid);
    Assert.Equal(repo.CommitOid, obj.Id);
    Assert.Equal(GitObjectType.Commit, obj.Type);
    var data = System.Text.Encoding.UTF8.GetString(obj.Data);
    Assert.Contains("Initial commit", data);
  }

  [Fact]
  public void CanReadOdbObjectByPrefix()
  {
    using var repo = new RepoWithOneCommit();
    using var odb = repo.Repo.GetOdb();
    using var obj = odb.ReadPrefix(repo.CommitOid.Sha.Substring(0, 7));
    Assert.Equal(repo.CommitOid, obj.Id);
    Assert.Equal(GitObjectType.Commit, obj.Type);
    var data = Encoding.UTF8.GetString(obj.Data);
    Assert.Contains("Initial commit", data);
  }

  [Fact]
  public void CanCallRefresh()
  {
    using var repo = new RepoWithOneCommit();
    using var odb = repo.Repo.GetOdb();
    odb.Refresh();
  }

  [Fact]
  public void CanWriteOdbObjectByStream()
  {
    using var repo = new EmptyRepo();
    using var odb = repo.Repo.GetOdb();
    var data = Encoding.UTF8.GetBytes("Hello, World!");
    using var writeStream = odb.OpenWriteStream((UIntPtr)data.Length, GitObjectType.Blob);
    writeStream.Write(data, data.Length);
    var oid = writeStream.FinalizeWrite();
    using var readObject = odb.Read(oid);
    Assert.Equal(data, readObject.Data);
    var (size, type) = odb.ReadHeader(oid);
    Assert.Equal((UIntPtr)data.Length, size);
    Assert.Equal(GitObjectType.Blob, type);
  }
}
