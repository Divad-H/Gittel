using Libgit2Bindings.Test.Helpers;

namespace Libgit2Bindings.Test.TestData;

internal class RepoWithOneCommit : IDisposable
{
  public const string Filename = "test.txt";
  public const string AuthorName = "John Doe";
  public const string AuthorEmail = "john@doe.de";
  public const string CommitBody = "This is the initial commit.";
  public const string CommitMessage = $"Initial commit\n\n{CommitBody}";

  public Libgit2 Libgit2 { get; }
  public TemporaryDirectory TempDirectory { get; }
  public IGitRepository Repo { get; }
  public IGitSignature Signature { get; }
  public GitOid CommitOid { get; }
  public IGitTree Tree { get; }
  public GitOid TreeOid { get; }

  public RepoWithOneCommit()
  {
    const string filename = Filename;

    Libgit2 = new Libgit2();
    TempDirectory = new TemporaryDirectory();

    Repo = Libgit2.InitRepository(TempDirectory.DirectoryPath, false);

    Signature = Repo.GitSignatureNow(AuthorName, AuthorEmail);

    var fileFullPath = Path.Combine(TempDirectory.DirectoryPath, filename);
    File.WriteAllLines(fileFullPath, ["my content"]);

    using var index = Repo.GetIndex();

    index.AddByPath(filename);
    TreeOid = index.WriteTree();
    index.Write();

    Tree = Repo.LookupTree(TreeOid);

    CommitOid = Repo.CreateCommit("HEAD", Signature, Signature, CommitMessage, Tree, null);
  }

  public void Dispose()
  {
    Tree.Dispose();
    Signature.Dispose();
    Repo.Dispose();
    TempDirectory.Dispose();
    Libgit2.Dispose();
  }
}

internal class EmptyRepo : IDisposable
{
  public Libgit2 Libgit2 { get; }
  public TemporaryDirectory TempDirectory { get; }
  public IGitRepository Repo { get; }

  public EmptyRepo()
  {
    Libgit2 = new Libgit2();
    TempDirectory = new TemporaryDirectory();

    Repo = Libgit2.InitRepository(TempDirectory.DirectoryPath, false);
  }

  public void Dispose()
  {
    Repo.Dispose();
    TempDirectory.Dispose();
    Libgit2.Dispose();
  }
}
