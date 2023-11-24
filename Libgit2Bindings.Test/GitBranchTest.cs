using Libgit2Bindings.Test.Helpers;
using Libgit2Bindings.Test.TestData;

namespace Libgit2Bindings.Test;

public class GitBranchTest
{
  [Fact]
  public void CanDeleteBranch()
  {
    const string branchName = "test-branch";

    using var repo = new RepoWithOneCommit();
    using var commit = repo.Repo.LookupCommit(repo.CommitOid);
    {
      using var branch = repo.Repo.CreateBranch(branchName, commit, false);
      branch.DeleteBranch();
    }
    try
    {
      using var deletedBranch = repo.Repo.LookupBranch(branchName, BranchType.LocalBranch);
    }
    catch (Libgit2Exception ex)
    {
      Assert.Equal(-3, ex.ErrorCode);
      return;
    }
    Assert.Fail("An exception should be thrown, indicating that the branch doesn't exist.");
  }

  [Fact]
  public void CanTestIfBranchIsNotCheckedOutOrHead()
  {
    const string branchName = "test-branch";

    using var repo = new RepoWithOneCommit();
    using var commit = repo.Repo.LookupCommit(repo.CommitOid);
    using var branch = repo.Repo.CreateBranch(branchName, commit, false);

    Assert.False(branch.IsBranchCheckedOut());
    Assert.False(branch.IsBranchHead());
  }

  [Fact]
  public void CanTestIfBranchIsCheckedOut()
  {
    const string branchName = "test-branch";

    using var repo = new RepoWithOneCommit();
    using var commit = repo.Repo.LookupCommit(repo.CommitOid);
    using var head = repo.Repo.GetHead();
    using var annotatedCommit = repo.Repo.GetAnnotatedCommitFromRef(head);

    using var branch = repo.Repo.CreateBranch(branchName, annotatedCommit, false);
    var canonicalName = $"{IGitReference.LocalBranchPrefix}{branch.BranchName()}";
    repo.Repo.SetHead(canonicalName);

    Assert.True(branch.IsBranchCheckedOut());
    Assert.True(branch.IsBranchHead());
  }

  [Fact]
  public void CanIterateBranches()
  {
    const string branchName = "test-branch";

    using var repo = new RepoWithOneCommit();
    using var commit = repo.Repo.LookupCommit(repo.CommitOid);
    using var branch = repo.Repo.CreateBranch(branchName, commit, false);

    foreach (var b in repo.Repo.LookupBranches(BranchType.All).DoNotAutoDisposeAfterIteration())
    {
      using (b)
        Assert.NotNull(b.BranchName());
    }
  }

  [Fact]
  public void CanIterateBranchesWithAutoDispose()
  {
    const string branchName = "test-branch";

    using var repo = new RepoWithOneCommit();
    using var commit = repo.Repo.LookupCommit(repo.CommitOid);
    using var branch = repo.Repo.CreateBranch(branchName, commit, false);

    var branchCount = repo.Repo.LookupBranches(BranchType.All).Count();
    Assert.Equal(2, branchCount);
  }

  [Fact]
  public void CanMoveBranch()
  {
    const string branchName = "test-branch";
    const string newBranchName = "test-branch-renamed";

    using var repo = new RepoWithOneCommit();
    using var commit = repo.Repo.LookupCommit(repo.CommitOid);
    using var branch = repo.Repo.CreateBranch(branchName, commit, false);

    branch.MoveBranch(newBranchName, false);
    Assert.Equal(newBranchName, branch.BranchName());
  }

  [Fact]
  public void CanCheckIfBranchNameIsValid()
  {
    using Libgit2 libgit2 = new();

    Assert.True(libgit2.BranchNameIsValid("foo"));
    Assert.True(libgit2.BranchNameIsValid("refs/heads/main"));
    Assert.False(libgit2.BranchNameIsValid("refs/heads/"));
    Assert.False(libgit2.BranchNameIsValid("-foo"));
  }

  [Fact]
  public void CanGetRemoteNameFromRemoteTrackingBranchName()
  {
    using var sourceRepo = new RepoWithOneCommit();
    using var tempDirectory = new TemporaryDirectory();

    using var clonedRepo = sourceRepo.Libgit2.Clone(
      sourceRepo.TempDirectory.DirectoryPath, tempDirectory.DirectoryPath);

    var remoteName = clonedRepo.GetRemoteNameFromBranch("refs/remotes/origin/main");
    Assert.Equal("origin", remoteName);
  }

  [Fact]
  public void CanSetUpstream()
  {
    const string branchName = "test-branch";
    const string upstreamBranchName = "test-upstream-branch";

    using var repo = new RepoWithOneCommit();
    using var commit = repo.Repo.LookupCommit(repo.CommitOid);
    using var branch = repo.Repo.CreateBranch(branchName, commit, false);
    using var upstreamBranch = repo.Repo.CreateBranch(upstreamBranchName, commit, false);

    branch.SetUpstream(upstreamBranchName);
    using var retrievedUpstreamBranch = branch.GetUpstream();
    Assert.Equal(upstreamBranchName, retrievedUpstreamBranch.BranchName());
  }
}
