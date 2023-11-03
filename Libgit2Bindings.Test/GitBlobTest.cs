using Libgit2Bindings.Test.Helpers;
using Libgit2Bindings.Test.TestData;
using System.Text;

namespace Libgit2Bindings.Test;

public class GitBlobTest
{
  [Fact]
  public void CanCreateBlobFromBuffer()
  {
    using var repo = new EmptyRepo();
    var content = Encoding.ASCII.GetBytes("some content");
    var blobId = repo.Repo.CreateBlob(content);
    Assert.NotNull(blobId);
    var blob = repo.Repo.LookupBlob(blobId);
    Assert.NotNull(blob);
    Assert.Equal(content, blob.RawContent());
  }

  class RepoWithBlob : EmptyRepo
  {
    public GitOid BlobId { get; }
    public IGitBlob Blob { get; }

    public RepoWithBlob()
    {
      var content = Encoding.ASCII.GetBytes("some content");
      BlobId = Repo.CreateBlob(content);
      Blob = Repo.LookupBlob(BlobId);
    }
  }

  [Fact]
  public void CanGetBlobId()
  {
    using var repo = new RepoWithBlob();
    Assert.NotNull(repo.BlobId);
    Assert.Equal(repo.BlobId, repo.Blob.Id());
  }

  [Fact]
  public void CanGetBlobOwner()
  {
    using var repo = new RepoWithBlob();
    using var owner = repo.Blob.Owner();
    Assert.NotNull(owner);
    Assert.Equal(repo.Repo.GetPath(), owner.GetPath());
  }

  [Fact]
  public void CanDuplicateBlob()
  {
    using var repo = new RepoWithBlob();
    using var duplicate = repo.Blob.Duplicate();
    Assert.NotNull(duplicate);
    Assert.Equal(repo.Blob.Id(), duplicate.Id());
  }

  [Fact]
  public void CanTestIfBlobContentIsBinary()
  {
    using var repo = new RepoWithBlob();
    Assert.False(repo.Blob.IsBinary());
  }

  [Fact]
  public void CanApplyCheckoutFilter()
  {
    using var repo = new EmptyRepo();
    var content = Encoding.ASCII.GetBytes("some content\n");
    var blobId = repo.Repo.CreateBlob(content);
    Assert.NotNull(blobId);
    var blob = repo.Repo.LookupBlob(blobId);
    Assert.NotNull(blob);
    File.WriteAllText(Path.Combine(repo.Repo.GetWorkdir()!, ".gitattributes"), "*.txt eol=lf");
    var filteredContent = blob.Filter("foo.txt", null);
    var filtertText = Encoding.ASCII.GetString(filteredContent);
    Assert.Equal("some content\n", filtertText);
    File.WriteAllText(Path.Combine(repo.Repo.GetWorkdir()!, ".gitattributes"), "*.txt eol=crlf");
    filteredContent = blob.Filter("foo.txt", new GitBlobFilterOptions());
    filtertText = Encoding.ASCII.GetString(filteredContent);
    Assert.Equal("some content\r\n", filtertText);
  }

  [Fact]
  public void CanApplyCheckoutFilterFromCommit()
  {
    using var repo = new RepoWithOneCommit();

    File.WriteAllText(Path.Combine(repo.Repo.GetWorkdir()!, ".gitattributes"), "*.txt eol=lf");

    using var index = repo.Repo.GetIndex();

    index.AddByPath(".gitattributes");
    var treeOid = index.WriteTree();
    index.Write();

    using var secondTree = repo.Repo.LookupTree(treeOid);
    using var firstCommit = repo.Repo.LookupCommit(repo.CommitOid);
    var secondCommitOid = repo.Repo.CreateCommit(
      "HEAD", repo.Signature, repo.Signature, "commit", secondTree, new[] { firstCommit });

    File.WriteAllText(Path.Combine(repo.Repo.GetWorkdir()!, ".gitattributes"), "*.txt eol=crlf");
    index.AddByPath(".gitattributes");
    treeOid = index.WriteTree();
    index.Write();

    using var thirdTree = repo.Repo.LookupTree(treeOid);
    using var secondCommit = repo.Repo.LookupCommit(secondCommitOid);
    var thirdCommitOid = repo.Repo.CreateCommit(
      "HEAD", repo.Signature, repo.Signature, "commit", thirdTree, new[] { secondCommit });

    File.Delete(Path.Combine(repo.Repo.GetWorkdir()!, ".gitattributes"));

    using var tempDirectory = new TemporaryDirectory();
    using var clonedRepo = repo.Libgit2.Clone(
      repo.TempDirectory.DirectoryPath, tempDirectory.DirectoryPath, new CloneOptions
      {
        Bare = true,
      });

    var content = Encoding.ASCII.GetBytes("some content\n");
    var blobId = clonedRepo.CreateBlob(content);
    Assert.NotNull(blobId);
    var blob = clonedRepo.LookupBlob(blobId);
    Assert.NotNull(blob);

    var filteredContent = blob.Filter("foo.txt", new GitBlobFilterOptions()
    {
      Flags = GitBlobFilterFlags.AttributesFromCommit | GitBlobFilterFlags.NoSystemAttributes,
      AttributesCommitId = secondCommitOid,
    });
    var filtertText = Encoding.ASCII.GetString(filteredContent);
    Assert.Equal("some content\n", filtertText);
    filteredContent = blob.Filter("foo.txt", new GitBlobFilterOptions()
    {
      Flags = GitBlobFilterFlags.AttributesFromCommit | GitBlobFilterFlags.NoSystemAttributes,
      AttributesCommitId = thirdCommitOid,
    });
    filtertText = Encoding.ASCII.GetString(filteredContent);
    Assert.Equal("some content\r\n", filtertText);
  }

  [Fact]
  public void CanLookupBlobFromPrefix()
  {
    using var repo = new RepoWithBlob();
    var blob = repo.Repo.LookupBlobByPrefix(repo.BlobId.Sha.Substring(0, 7));
    Assert.NotNull(blob);
    Assert.Equal(repo.BlobId, blob.Id());
  }

  [Fact]
  public void CanCreateBlobFromDisk()
  {
    using var repo = new EmptyRepo();
    var path = Path.Combine(repo.TempDirectory.DirectoryPath, "foo.txt");
    File.WriteAllText(path, "some content");
    var blobId = repo.Repo.CreateBlobFromDisk(path);
    Assert.NotNull(blobId);
    var blob = repo.Repo.LookupBlob(blobId);
    Assert.NotNull(blob);
    Assert.Equal(Encoding.UTF8.GetBytes("some content"), blob.RawContent());
  }

  [Fact]
  public void CanCreateBlobFromWorkdir()
  {
    using var repo = new EmptyRepo();
    var path = Path.Combine(repo.TempDirectory.DirectoryPath, "foo.txt");
    File.WriteAllText(path, "some content");
    var blobId = repo.Repo.CreateBlobFromWorkdir("foo.txt");
    Assert.NotNull(blobId);
    var blob = repo.Repo.LookupBlob(blobId);
    Assert.NotNull(blob);
    Assert.Equal(Encoding.UTF8.GetBytes("some content"), blob.RawContent());
  }

  [Fact]
  public void CanCreateBlobFromStream()
  {
    using var repo = new EmptyRepo();

    using var stream = repo.Repo.CreateBlobFromStream("foo.txt");
    using var writer = new StreamWriter(stream);
    writer.Write("some content");
    writer.Flush();

    var blobId = stream.Commit();
    Assert.NotNull(blobId);
    var blob = repo.Repo.LookupBlob(blobId);
    Assert.NotNull(blob);
    Assert.Equal(Encoding.UTF8.GetBytes("some content"), blob.RawContent());
  }
}
