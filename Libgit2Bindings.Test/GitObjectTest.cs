using Libgit2Bindings.Test.TestData;
using System.Text;

namespace Libgit2Bindings.Test;

public class GitObjectTest
{
  [Fact]
  public void CanLookupObject()
  {
    using var repo = new EmptyRepo();
    var oid = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("my content"));
    using var blob = repo.Repo.LookupObject(oid, GitObjectType.Blob);
    Assert.Equal(oid, blob.Id);
  }

  [Fact]
  public void CanDuplicateObject()
  {
    using var repo = new EmptyRepo();
    var oid = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("my content"));
    using var blob = repo.Repo.LookupObject(oid, GitObjectType.Blob);
    using var duplicate = blob.Duplicate();
    Assert.Equal(oid, duplicate.Id);
  }

  [Fact]
  public void CanLookupObjectByPath()
  {
    using var repo = new RepoWithOneCommit();
    using var treeObject = repo.Repo.LookupObject(repo.TreeOid, GitObjectType.Tree);
    using var blobObject = treeObject.LookupByPath(RepoWithOneCommit.Filename, GitObjectType.Blob);
    var type = blobObject.Type;
    Assert.Equal(GitObjectType.Blob, type);
  }

  [Fact]
  public void CanDetermineLooseObjectTypes()
  {
    using var libgit2 = new Libgit2();
    Assert.True(libgit2.GitObjectTypeIsLoose(GitObjectType.Tag));
    Assert.True(libgit2.GitObjectTypeIsLoose(GitObjectType.Blob));
    Assert.True(libgit2.GitObjectTypeIsLoose(GitObjectType.Tree));
    Assert.True(libgit2.GitObjectTypeIsLoose(GitObjectType.Commit));
    Assert.False(libgit2.GitObjectTypeIsLoose(GitObjectType.OffsetDelta));
    Assert.False(libgit2.GitObjectTypeIsLoose(GitObjectType.RefDelta));
    Assert.False(libgit2.GitObjectTypeIsLoose(GitObjectType.Any));
    Assert.False(libgit2.GitObjectTypeIsLoose(GitObjectType.Invalid));
  }

  [Fact]
  public void CanGetOwner()
  {
    using var repo = new EmptyRepo();
    var oid = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("my content"));
    using var blob = repo.Repo.LookupObject(oid, GitObjectType.Blob);
    Assert.Equal(repo.Repo.GetPath(), blob.Owner.GetPath());
  }

  [Fact]
  public void CanTestIfRawContentIsValid()
  {
    using var libgit2 = new Libgit2();

    const string commit = "tree 4b825dc642cb6eb9a060e54bf8d69288fbee4904\n" +
      "author David Hübscher <huebschersdavid@gmail.com> 1696283371 +0200\n" +
      "committer David Hübscher <huebschersdavid@gmail.com> 1696283371 +0200\n" +
      "\n" +
      "an empty but signed commit\n";

    byte[] rawCommit = Encoding.UTF8.GetBytes(commit);
    var valid = libgit2.GitObjectRawContentIsValid(rawCommit, GitObjectType.Commit);
    Assert.True(valid);

    const string invalidCommit = "rubbish";
    byte[] rawInvalidCommit = Encoding.UTF8.GetBytes(invalidCommit);
    valid = libgit2.GitObjectRawContentIsValid(rawInvalidCommit, GitObjectType.Commit);
    Assert.False(valid);
  }

  [Fact]
  public void CanGetShortId()
  {
    using var repo = new EmptyRepo();
    var oid = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("my content"));
    using var blob = repo.Repo.LookupObject(oid, GitObjectType.Blob);
    var shortId = blob.ShortId;
    Assert.StartsWith(shortId, oid.Sha);
  }
}
