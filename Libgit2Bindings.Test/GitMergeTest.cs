using Libgit2Bindings.Test.TestData;
using System.Text;

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
  public void CanMergeCommitsIntoIndex()
  {
    using RepoWithTwoBranches repoWithTwoBranches = new();
    var repo = repoWithTwoBranches.Repo;

    using var secondBranchCommit = repo.LookupCommit(repoWithTwoBranches.SecondBranchCommitOid);
    using var firstBranchCommit = repo.LookupCommit(repoWithTwoBranches.SecondCommitOid);

    using var index = repo.MergeCommits(secondBranchCommit, firstBranchCommit);

    Assert.False(index.HasConflicts());
    Assert.Equal(2u, index.EntryCount);
  }

  [Fact]
  public void CanMergeTreesIntoIndex()
  {
    using RepoWithTwoBranches repoWithTwoBranches = new();
    var repo = repoWithTwoBranches.Repo;

    using var index = repo.MergeTrees(
      repoWithTwoBranches.FirstTree, repoWithTwoBranches.SecondBranchTree, repoWithTwoBranches.SecondTree );

    Assert.False(index.HasConflicts());
    Assert.Equal(2u, index.EntryCount);
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

  [Fact]
  public void CanAnalyzeMergeWithRef()
  {
    using RepoWithTwoBranches repoWithTwoBranches = new();
    var repo = repoWithTwoBranches.Repo;

    using var commit = repo.LookupCommit(repoWithTwoBranches.SecondBranchCommitOid);

    using var annotatedCommit = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondCommitOid);

    using var head = repo.GetHead();

    var (analysis, preferences) = repo.MergeAnalysisForRef(head,[annotatedCommit]);

    Assert.Equal(GitMergeAnalysisResult.Normal, analysis);
    Assert.True(preferences == GitMergePreference.None 
      || preferences == GitMergePreference.FastForwardOnly
      || preferences == GitMergePreference.NoFastForward);
  }

  [Fact]
  public void CanGetMergeBase()
  {
    using RepoWithTwoBranches repoWithTwoBranches = new();
    var repo = repoWithTwoBranches.Repo;

    var mergeBaseOid = repo.GetMergeBase(
      repoWithTwoBranches.SecondBranchCommitOid, repoWithTwoBranches.SecondCommitOid);

    Assert.Equal(repoWithTwoBranches.FirstCommitOid, mergeBaseOid);
  }

  [Fact]
  public void CanGetMergeBaseOfManyCommits()
  {
    using RepoWithTwoBranches repoWithTwoBranches = new();
    var repo = repoWithTwoBranches.Repo;

    var mergeBaseOid = repo.GetMergeBase(
      [repoWithTwoBranches.SecondBranchCommitOid, repoWithTwoBranches.SecondCommitOid]);

    Assert.Equal(repoWithTwoBranches.FirstCommitOid, mergeBaseOid);
  }

  [Fact]
  public void CanGetMergeBaseOctopus()
  {
    using RepoWithTwoBranches repoWithTwoBranches = new();
    var repo = repoWithTwoBranches.Repo;

    var mergeBaseOid = repo.GetMergeBaseOctopus(
      [repoWithTwoBranches.SecondBranchCommitOid, repoWithTwoBranches.SecondCommitOid]);

    Assert.Equal(repoWithTwoBranches.FirstCommitOid, mergeBaseOid);
  }

  [Fact]
  public void CanGetMergeBases()
  {
    using RepoWithTwoBranches repoWithTwoBranches = new();
    var repo = repoWithTwoBranches.Repo;

    using var commit = repo.LookupCommit(repoWithTwoBranches.SecondBranchCommitOid);
    using var annotatedCommit = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondCommitOid);

    repo.Merge([annotatedCommit], null, null);

    using var thirdBranch = repo.CreateBranch("third-branch", commit, false);

    var mergeCommit1Oid = repo.CreateCommit(
      $"{IGitReference.LocalBranchPrefix}{thirdBranch.BranchName()}",
      repoWithTwoBranches.Signature, repoWithTwoBranches.Signature,
      "Merge commit 1",
      repoWithTwoBranches.SecondBranchTree,
      [commit, repo.LookupCommit(repoWithTwoBranches.SecondCommitOid)]);
    
    var mergeCommit2Oid = repo.CreateCommit(
      "HEAD",
      repoWithTwoBranches.Signature, repoWithTwoBranches.Signature,
      "Merge commit 2",
      repoWithTwoBranches.SecondBranchTree,
      [commit, repo.LookupCommit(repoWithTwoBranches.SecondCommitOid)]);

    repo.CleanupState();

    var mergeBases = repo.GetMergeBases(mergeCommit2Oid, mergeCommit1Oid);

    Assert.Equal(2, mergeBases.Count);
    Assert.Equal(repoWithTwoBranches.SecondBranchCommitOid, mergeBases[0]);
    Assert.Equal(repoWithTwoBranches.SecondCommitOid, mergeBases[1]);
  }

  [Fact]
  public void CanGetMergeBasesOfManyCommits()
  {
    using RepoWithTwoBranches repoWithTwoBranches = new();
    var repo = repoWithTwoBranches.Repo;

    using var commit = repo.LookupCommit(repoWithTwoBranches.SecondBranchCommitOid);
    using var annotatedCommit = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondCommitOid);

    repo.Merge([annotatedCommit], null, null);

    using var thirdBranch = repo.CreateBranch("third-branch", commit, false);

    var mergeCommit1Oid = repo.CreateCommit(
      $"{IGitReference.LocalBranchPrefix}{thirdBranch.BranchName()}",
      repoWithTwoBranches.Signature, repoWithTwoBranches.Signature,
      "Merge commit 1",
      repoWithTwoBranches.SecondBranchTree,
      [commit, repo.LookupCommit(repoWithTwoBranches.SecondCommitOid)]);
    
    var mergeCommit2Oid = repo.CreateCommit(
      "HEAD",
      repoWithTwoBranches.Signature, repoWithTwoBranches.Signature,
      "Merge commit 2",
      repoWithTwoBranches.SecondBranchTree,
      [commit, repo.LookupCommit(repoWithTwoBranches.SecondCommitOid)]);

    repo.CleanupState();

    var mergeBases = repo.GetMergeBases(
           [mergeCommit2Oid, mergeCommit1Oid]);

    Assert.Equal(2, mergeBases.Count);
    Assert.Equal(repoWithTwoBranches.SecondBranchCommitOid, mergeBases[0]);
    Assert.Equal(repoWithTwoBranches.SecondCommitOid, mergeBases[1]);
  }


  [Fact]
  public void CanMergeFiles()
  {
    GitMergeFileInput ancestor = new()
    {
      Path = "test.txt",
      Mode = 0,
      FileContent = Encoding.UTF8.GetBytes("line1\nline2\nline3\nline4\n"),
    };
    GitMergeFileInput ours = new()
    {
      Path = "test.txt",
      Mode = 0,
      FileContent = Encoding.UTF8.GetBytes("line1\nline2 extended\nline3\nline4\n"),
    };
    GitMergeFileInput theirs = new()
    {
      Path = "test.txt",
      Mode = 0,
      FileContent = Encoding.UTF8.GetBytes("line1\nline2\nline3\nline4 extended\n"),
    };

    using var libgit2 = new Libgit2();
    var result = libgit2.MergeFiles(ancestor, ours, theirs);

    Assert.Equal("line1\nline2 extended\nline3\nline4 extended\n", Encoding.UTF8.GetString(result.Content));
    Assert.True(result.Automergeable);
    Assert.Equal("test.txt", result.Path);
  }
}
