using Libgit2Bindings.Test.TestData;

namespace Libgit2Bindings.Test;

public class GitCherrypickTest
{
  [Fact]
  public void CanCherrypick()
  {
    using var repo = new RepoWithOneCommit();
    using var commit = repo.Repo.LookupCommit(repo.CommitOid);

    var fileFullPath = Path.Combine(repo.TempDirectory.DirectoryPath, RepoWithOneCommit.Filename);
    File.WriteAllLines(fileFullPath, ["my content", "more content"]);

    using var index = repo.Repo.GetIndex();

    index.AddByPath(RepoWithOneCommit.Filename);
    var treeOid = index.WriteTree();
    index.Write();

    using var tree = repo.Repo.LookupTree(treeOid);
    var commitOid = repo.Repo.CreateCommit(null, repo.Signature, repo.Signature, "msg", tree, [commit]);
    using var secondCommit = repo.Repo.LookupCommit(commitOid);

    repo.Repo.CheckoutHead(new() { Strategy = CheckoutStrategy.Force });

    var cherrypickOptions = new CherrypickOptions();
    repo.Repo.Cherrypick(secondCommit, cherrypickOptions);

    using var newIndex = repo.Repo.GetIndex();
    var cherrypickedTreeOid = index.WriteTree();

    Assert.Equal(treeOid, cherrypickedTreeOid);
    Assert.NotEqual(cherrypickedTreeOid, repo.Tree.GetId());
  }

  [Fact]
  public void CanCherrypickCommit()
  {
    using var repo = new RepoWithOneCommit();
    using var commit = repo.Repo.LookupCommit(repo.CommitOid);

    var fileFullPath = Path.Combine(repo.TempDirectory.DirectoryPath, RepoWithOneCommit.Filename);
    File.WriteAllLines(fileFullPath, ["my content", "more content"]);

    using var index = repo.Repo.GetIndex();

    index.AddByPath(RepoWithOneCommit.Filename);
    var treeOid = index.WriteTree();
    index.Write();

    using var tree = repo.Repo.LookupTree(treeOid);
    var commitOid = repo.Repo.CreateCommit(null, repo.Signature, repo.Signature, "msg", tree, [commit]);
    using var secondCommit = repo.Repo.LookupCommit(commitOid);

    repo.Repo.CheckoutHead(new() { Strategy = CheckoutStrategy.Force });

    using var newIndex = repo.Repo.CherrypickCommit(secondCommit, commit, 0);
    
    var cherrypickedTreeOid = newIndex.WriteTreeTo(repo.Repo);

    Assert.Equal(treeOid, cherrypickedTreeOid);
    Assert.NotEqual(cherrypickedTreeOid, repo.Tree.GetId());
  }
}
