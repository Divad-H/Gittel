using Libgit2Bindings.Test.Helpers;
using System.Text;

namespace Libgit2Bindings.Test;

public class GitCommitTest
{
  private class RepoWithOneCommit : IDisposable
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

      Tree = Repo.LookupTree(treeOid);

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

  [Fact]
  public void CanCreateCommit()
  {
    using var repoWithOneCommit = new RepoWithOneCommit();
    
    Assert.NotNull(repoWithOneCommit.CommitOid);
  }

  [Fact]
  public void CanCreateCommitObject()
  {
    using var repoWithOneCommit = new RepoWithOneCommit();

    using var commit = repoWithOneCommit.Repo.LookupCommit(repoWithOneCommit.CommitOid);

    var commitObject = repoWithOneCommit.Repo.CreateCommitObject(repoWithOneCommit.Signature, 
      repoWithOneCommit.Signature, "second commit", repoWithOneCommit.Tree, new IGitCommit[] { commit });
    Assert.NotNull(commitObject);

    var commitObjectString = Encoding.UTF8.GetString(commitObject);

    Assert.Contains("second commit", commitObjectString);
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

    var amendedCommitOid = commit.Amend("HEAD", null, null, "Amended commit", null);

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

  [Fact]
  public void CanReadCommitter()
  {
    using var repoWithOneCommit = new RepoWithOneCommit();

    using var commit = repoWithOneCommit.Repo.LookupCommit(repoWithOneCommit.CommitOid);

    var committer = commit.GetCommitter();

    Assert.NotNull(committer);
    Assert.Equal(RepoWithOneCommit.AuthorName, committer.Name);
    Assert.Equal(RepoWithOneCommit.AuthorEmail, committer.Email);
  }

  [Fact]
  public void CanReadCommitterWithMailmap()
  {
    const string realEmail = "Jim@Doe.de";

    using var repoWithOneCommit = new RepoWithOneCommit();

    using var commit = repoWithOneCommit.Repo.LookupCommit(repoWithOneCommit.CommitOid);

    using var mailmap = repoWithOneCommit.Repo.GetMailmap();
    mailmap.AddEntry(null, realEmail, null, RepoWithOneCommit.AuthorEmail);

    var committer = commit.GetCommitter(mailmap);

    Assert.NotNull(committer);
    Assert.Equal(RepoWithOneCommit.AuthorName, committer.Name);
    Assert.Equal(realEmail, committer.Email);
  }

  [Fact]
  public void CanReadBody()
  {
    using var repoWithOneCommit = new RepoWithOneCommit();

    using var commit = repoWithOneCommit.Repo.LookupCommit(repoWithOneCommit.CommitOid);

    var body = commit.GetBody();

    Assert.NotNull(body);
    Assert.Equal(RepoWithOneCommit.CommitBody, body);
  }

  [Fact]
  public void CanCreateSignedCommitAndExtractSignature()
  {
    using var libgit2 = new Libgit2();
    using var tempDirectory = new TemporaryDirectory();

    using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

    const string commit = "tree 4b825dc642cb6eb9a060e54bf8d69288fbee4904\n" +
      "author David Hübscher <huebschersdavid@gmail.com> 1696283371 +0200\n" +
      "committer David Hübscher <huebschersdavid@gmail.com> 1696283371 +0200\n" +
      "\n" +
      "an empty but signed commit\n";

    const string gpgsig = "-----BEGIN PGP SIGNATURE-----\n" +
      "\n" +
      "iQIzBAABCAAdFiEEiQg9Wo4C+bIB/zfgrSSj3zCxtdsFAmUbOusACgkQrSSj3zCx\n" +
      "tdsvOxAAmJ/mxTN4McVpbhqTij/oTmzy7ff4X8YpJ5K0qfVeeUv7lufd6pPvxbdG\n" +
      "iaBS7Ui/QRQ+/SXPub+swzaOg4XsOPWN40+38BS35IIBwLoROP0gsx40NRL29qRd\n" +
      "rtdgy8r5FnQKr4mXd1lFnScOrppDAytpL4S0KG26yPWtNgG7ixEd0jAhAisQ+721\n" +
      "9PO9YshySCcjdmPh1XNNLupHHz7lqg7FvsVcgD5lf9vB6AweLa2KAoL1pOPIFw/n\n" +
      "i3C7G/STOcVLk9z9eBVaPkwStO/m2Js7I9Y2jxPNg2ZhpROJb47tsR5qSj62V2CI\n" +
      "OtKTQBWzfl+6WVcIA0N5wyyuG3UIQYtF8c1sHm2cbAR59L28hY7NvxfHEhChU2Qo\n" +
      "81ZX5dtwQnsZyPOSMxlkuYsT4TU3drjSkZ3KIlhmL3mxfigUNMX/4S1WV4ZqLsQs\n" +
      "2VZvcuOfDod3MlbvHLFvs4dPAv8uWv6/wBQRQnFsJvT9i3+cYugAg+ZZ37XYrX4P\n" +
      "4iVSXIjeFEJVW+n/BnzwxriQlDlOdoETEvJuH/a/KlkA+pU+nbEtu8cduPjhRyb2\n" +
      "+qHVoWprwT6WUZeAAmx9z6UoGNo3fg5is3PPrf3ddUrvpm2fueLyZjhzaMGQNXoU\n" +
      "tmnvBtzNBY3ccqhTo1nKU2Pgwtg8B+I2M7gGuZ4uhHMXqOvX9TA=\n" +
      "=q0RV\n" +
      "-----END PGP SIGNATURE-----";

    var oid = repo.CreateCommitWithSignature(commit, gpgsig, "gpgsig");

    Assert.NotNull(oid);

    var (resSignature, signedData) = repo.ExtractCommitSignature(oid, null);

    Assert.NotNull(resSignature);
    Assert.NotNull(signedData);

    var resSignatureString = Encoding.UTF8.GetString(resSignature);
    var signedDataString = Encoding.UTF8.GetString(signedData);

    Assert.Equal(gpgsig, resSignatureString);
    Assert.Equal(commit, signedDataString);
  }

  [Fact]
  public void CanGetHeaderField()
  {
    using var repoWithOneCommit = new RepoWithOneCommit();
    using var commit = repoWithOneCommit.Repo.LookupCommit(repoWithOneCommit.CommitOid);

    var headerField = commit.GetHeaderField("tree");

    Assert.NotNull(headerField);

    var headerFieldString = Encoding.UTF8.GetString(headerField);

    var treeOid = repoWithOneCommit.Tree.GetId();
    Assert.NotNull(treeOid);

    Assert.Equal(treeOid.Sha, headerFieldString);
  }

  [Fact]
  public void CanGetId()
  {
    using var repoWithOneCommit = new RepoWithOneCommit();
    using var commit = repoWithOneCommit.Repo.LookupCommit(repoWithOneCommit.CommitOid);

    var commitOid = commit.GetId();
    Assert.NotNull(commitOid);
    Assert.Equal(repoWithOneCommit.CommitOid, commitOid);
  }

  [Fact]
  public void CanLookupCommitByPrefix()
  {
    using var repoWithOneCommit = new RepoWithOneCommit();
    using var commit = repoWithOneCommit.Repo.LookupCommitPrefix(
      repoWithOneCommit.CommitOid.Sha.Substring(0, 7));

    var commitOid = commit.GetId();
    Assert.NotNull(commitOid);
    Assert.Equal(repoWithOneCommit.CommitOid, commitOid);
  }
}
