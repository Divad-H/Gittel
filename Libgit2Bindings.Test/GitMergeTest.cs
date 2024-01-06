using libgit2;
using Libgit2Bindings.Test.TestData;

namespace Libgit2Bindings.Test;

public sealed class GitMergeTest
{
  [Fact]
  public void CanMerge()
  {
    const string secondFileName = "secondFile.txt";

    using var repoWithTwoCommits = new RepoWithTwoCommits();
    var repo = repoWithTwoCommits.Repo;

    using var firstCommit = repo.LookupCommit(repoWithTwoCommits.FirstCommitOid);

    using var branch = repo.CreateBranch("test-branch", firstCommit, false);

    repo.SetHead($"{IGitReference.LocalBranchPrefix}{branch.BranchName()}");
    repo.CheckoutHead(new()
    {
      Strategy = CheckoutStrategy.Force
    });

    var fileFullPath = Path.Combine(repoWithTwoCommits.TempDirectory.DirectoryPath, secondFileName);
    File.WriteAllLines(fileFullPath, ["second file content"]);

    using var index = repo.GetIndex();

    index.AddByPath(secondFileName);
    var treeOid = index.WriteTree();
    index.Write();

    using var tree = repo.LookupTree(treeOid);
    var commitOid = repo.CreateCommit(
      "HEAD", 
      repoWithTwoCommits.Signature, repoWithTwoCommits.Signature, 
      "test-branch commit", 
      tree, 
      [firstCommit]);
    using var commit = repo.LookupCommit(commitOid);

    using var annotatedCommit = repo.AnnotatedCommitLookup(repoWithTwoCommits.SecondCommitOid);

    repo.Merge([annotatedCommit], null, null);

    Assert.False(index.HasConflicts());

    var mergeCommitOid = repo.CreateCommit(
      "HEAD", 
      repoWithTwoCommits.Signature, repoWithTwoCommits.Signature, 
      "Merge commit",
      tree, 
      [commit, repo.LookupCommit(repoWithTwoCommits.SecondCommitOid)]);

    using var mergeCommit = repo.LookupCommit(mergeCommitOid);
    Assert.Equal(2u, mergeCommit.GetParentCount());

    repo.CleanupState();
  }
}
