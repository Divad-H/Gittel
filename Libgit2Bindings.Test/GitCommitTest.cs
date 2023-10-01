using Libgit2Bindings.Test.Helpers;

namespace Libgit2Bindings.Test;

public class GitCommitTest
{
  private class RepoWithOneCommit : IDisposable
  {
    public const string Filename = "test.txt";
    public const string AuthorName = "John Doe";
    public const string AuthorEmail = "john@doe.de";
    public const string CommitMessage = "Initial commit";

    public Libgit2 Libgit2 { get; }
    public TemporaryDirectory TempDirectory { get; }
    public IGitRepository Repo { get; }
    public IGitSignature Signature { get; }
    public GitOid CommitOid { get; }

    public RepoWithOneCommit()
    {
      const string filename = Filename;

      Libgit2 = new Libgit2();
      TempDirectory = new TemporaryDirectory();

      Repo = Libgit2.InitRepository(TempDirectory.DirectoryPath, false);

      Signature = Repo.GitSignatureNow(AuthorName, AuthorEmail);

      var fileFullPath = Path.Combine(TempDirectory.DirectoryPath, filename);
      File.WriteAllLines(fileFullPath, Array.Empty<string>());

      using var index = Repo.GetIndex();

      index.AddByPath(filename);
      var treeOid = index.WriteTree();
      index.Write();

      using var tree = Repo.LookupTree(treeOid);

      CommitOid = Repo.CreateCommit("HEAD", Signature, Signature, null, CommitMessage, tree, null);
    }

    public void Dispose()
    {
      Signature.Dispose();
      Repo.Dispose();
      TempDirectory.Dispose();
      Libgit2.Dispose();
    }
  }

  [Fact]
  public void CanCreateCommit()
  {
    using var repoWithOneCommit = new RepoWithOneCommit();
    
    Assert.NotNull(repoWithOneCommit.CommitOid);
  }

  [Fact]
  public void CanLookupCommit()
  {
    using var repoWithOneCommit = new RepoWithOneCommit();

    using var commit = repoWithOneCommit.Repo.LookupCommit(repoWithOneCommit.CommitOid);
    Assert.NotNull(commit);
  }

  [Fact]
  public void CanAmendCommit()
  {
    using var repoWithOneCommit = new RepoWithOneCommit();

    using var commit = repoWithOneCommit.Repo.LookupCommit(repoWithOneCommit.CommitOid);

    var amendedCommitOid = commit.Amend("HEAD", null, null, null, "Amended commit", null);

    Assert.NotNull(amendedCommitOid);
  }

  [Fact]
  public void CanReadAuthor()
  {
    using var repoWithOneCommit = new RepoWithOneCommit();

    using var commit = repoWithOneCommit.Repo.LookupCommit(repoWithOneCommit.CommitOid);

    var author = commit.GetAuthor();

    Assert.NotNull(author);
    Assert.Equal(RepoWithOneCommit.AuthorName, author.Name);
    Assert.Equal(RepoWithOneCommit.AuthorEmail, author.Email);
  }

  [Fact]
  public void CanReadAuthorWithMailmap()
  {
    const string realEmail = "Jim@Doe.de";

    using var repoWithOneCommit = new RepoWithOneCommit();

    using var commit = repoWithOneCommit.Repo.LookupCommit(repoWithOneCommit.CommitOid);

    using var mailmap = repoWithOneCommit.Repo.GetMailmap();
    mailmap.AddEntry(null, realEmail, null, RepoWithOneCommit.AuthorEmail);

    var author = commit.GetAuthor(mailmap);

    Assert.NotNull(author);
    Assert.Equal(RepoWithOneCommit.AuthorName, author.Name);
    Assert.Equal(realEmail, author.Email);
  }
}
