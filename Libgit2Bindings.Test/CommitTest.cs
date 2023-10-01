using Libgit2Bindings.Test.Helpers;

namespace Libgit2Bindings.Test;

public class CommitTest
{
  private class RepoWithOneCommit : IDisposable
  {
    public Libgit2 Libgit2 { get; }
    public TemporaryDirectory TempDirectory { get; }
    public IGitRepository Repo { get; }
    public IGitSignature Signature { get; }
    public GitOid CommitOid { get; }

    public RepoWithOneCommit()
    {
      const string filename = "test.txt";

      Libgit2 = new Libgit2();
      TempDirectory = new TemporaryDirectory();

      Repo = Libgit2.InitRepository(TempDirectory.DirectoryPath, false);

      Signature = Repo.GitSignatureNow("John Doe", "john@doe.de");

      var fileFullPath = Path.Combine(TempDirectory.DirectoryPath, filename);
      File.WriteAllLines(fileFullPath, Array.Empty<string>());

      using var index = Repo.GetIndex();

      index.AddByPath(filename);
      var treeOid = index.WriteTree();
      index.Write();

      using var tree = Repo.LookupTree(treeOid);

      CommitOid = Repo.CreateCommit("HEAD", Signature, Signature, null, "Initial commit", tree, null);
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
}
