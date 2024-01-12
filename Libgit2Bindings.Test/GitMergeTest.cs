using libgit2;
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

    repo.Merge([annotatedCommit], new(), new());

    using var index = repo.GetIndex();
    Assert.False(index.HasConflicts());

    using var secondCommit = repo.LookupCommit(repoWithTwoBranches.SecondCommitOid);
    var mergeCommitOid = repo.CreateCommit(
      "HEAD",
      repoWithTwoBranches.Signature, repoWithTwoBranches.Signature, 
      "Merge commit",
      repoWithTwoBranches.SecondBranchTree, 
      [commit, secondCommit]);

    using var mergeCommit = repo.LookupCommit(mergeCommitOid);
    Assert.Equal(2u, mergeCommit.GetParentCount());

    repo.CleanupState();
  }

  [Fact]
  public void CanMergeWithOptions()
  {
    using RepoWithTwoBranches repoWithTwoBranches = new();
    var repo = repoWithTwoBranches.Repo;
    using var secondBranchFirstCommit = repo.LookupCommit(repoWithTwoBranches.SecondBranchCommitOid);
    using var secondCommit = repo.LookupCommit(repoWithTwoBranches.SecondCommitOid);

    var fileFullPath = Path.Combine(repoWithTwoBranches.TempDirectory.DirectoryPath, RepoWithOneCommit.Filename);
    File.WriteAllLines(fileFullPath, ["0", " my content"]);

    using var index = repo.GetIndex();

    index.AddByPath(RepoWithOneCommit.Filename);
    var treeOid = index.WriteTree();
    index.Write();

    using var tree = repo.LookupTree(treeOid);
    var commitOid = repo.CreateCommit(
    "HEAD",
    repoWithTwoBranches.Signature, repoWithTwoBranches.Signature,
      "test-branch second commit",
      tree,
      [secondBranchFirstCommit]);

    using var annotatedCommit = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondCommitOid);

    bool notifyCalled = false;
    bool progressCalled = false;
    bool performanceDataCalled = false;

    repo.Merge([annotatedCommit], 
      new()
      { 
        FileFlags = MergeFileFlags.IgnoreWhitespace
      },
      new()
      {
        NotifyCallback = (why, path, baseline, target, workdir) =>
        {
          Assert.Equal(CheckoutNotifyFlags.Updated, why);
          Assert.Equal(RepoWithOneCommit.Filename, path);
          Assert.Equal(RepoWithOneCommit.Filename, baseline?.Path);
          Assert.Equal(RepoWithOneCommit.Filename, target?.Path);
          Assert.Equal(RepoWithOneCommit.Filename, workdir?.Path);
          notifyCalled = true;
          return GitOperationContinuation.Continue;
        },
        NotifyFlags = CheckoutNotifyFlags.All,
        ProgressCallback = (path, completedSteps, totalSteps) =>
        {
          progressCalled = true;
        },
        PerformanceDataCallback = (data) =>
        {
          performanceDataCalled = true;
        }
      });

    Assert.True(notifyCalled);
    Assert.True(progressCalled);
    Assert.True(performanceDataCalled);

    Assert.False(index.HasConflicts());

    using var secondBranchLastCommit = repo.LookupCommit(commitOid);
    var mergeCommitOid = repo.CreateCommit(
      "HEAD",
      repoWithTwoBranches.Signature, repoWithTwoBranches.Signature,
      "Merge commit",
      repoWithTwoBranches.SecondBranchTree,
      [secondBranchLastCommit, secondCommit]);

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

  [Fact]
  public void CanMergeFilesFromIndex()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();
    
    var ancestor = new GitIndexEntry()
    {
      Path = "file.txt",
      CTime = new(),
      MTime = new(),
      Mode = 33188,
      Id = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("line1\nline2\nline3\nline4\n"))
    };
    index.Add(ancestor);

    var ours = new GitIndexEntry()
    {
      Path = "file.txt",
      CTime = new(),
      MTime = new(),
      Mode = 33188,
      Id = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("line1\nline2 extended\nline3\nline4\n"))
    };
    index.Add(ours);

    var theirs = new GitIndexEntry()
    {
      Path = "file.txt",
      CTime = new(),
      MTime = new(),
      Mode = 33188,
      Id = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("line1\nline2\nline3\nline4 extended\n"))
    };
    index.Add(theirs);

    var result = repo.Repo.MergeFilesFromIndex(ancestor, ours, theirs);

    Assert.Equal("line1\nline2 extended\nline3\nline4 extended\n", Encoding.UTF8.GetString(result.Content));
    Assert.True(result.Automergeable);
    Assert.Equal("file.txt", result.Path);
  }
}
