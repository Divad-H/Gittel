using Libgit2Bindings.Test.TestData;

namespace Libgit2Bindings.Test;

public sealed class GitMergeTest
{
  [Fact]
  public void CanMerge()
  {
    using RepoWithTwoBranches repoWithTwoBranches = new();
    var repo = repoWithTwoBranches.Repo;

    using var commit = repo.LookupCommit(repoWithTwoBranches.SecondBranchCommitOid);

    using var annotatedCommit = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondCommitOid);

    repo.Merge([annotatedCommit], null, null);

    using var index = repo.GetIndex();
    Assert.False(index.HasConflicts());

    var mergeCommitOid = repo.CreateCommit(
      "HEAD",
      repoWithTwoBranches.Signature, repoWithTwoBranches.Signature, 
      "Merge commit",
      repoWithTwoBranches.SecondBranchTree, 
      [commit, repo.LookupCommit(repoWithTwoBranches.SecondCommitOid)]);

    using var mergeCommit = repo.LookupCommit(mergeCommitOid);
    Assert.Equal(2u, mergeCommit.GetParentCount());

    repo.CleanupState();
  }

  [Fact]
  public void CanAnalyzeMerge()
  {
    using RepoWithTwoBranches repoWithTwoBranches = new();
    var repo = repoWithTwoBranches.Repo;

    using var commit = repo.LookupCommit(repoWithTwoBranches.SecondBranchCommitOid);

    using var annotatedCommit = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondCommitOid);

    var (analysis, preferences) = repo.MergeAnalysis([annotatedCommit]);

    Assert.Equal(GitMergeAnalysisResult.Normal, analysis);
    Assert.True(preferences == GitMergePreference.None 
      || preferences == GitMergePreference.FastForwardOnly
      || preferences == GitMergePreference.NoFastForward);
  }
}
