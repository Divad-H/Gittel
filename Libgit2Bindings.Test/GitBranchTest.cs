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
  public void CanTestIfBranchIsNotCheckedOut()
  {
    const string branchName = "test-branch";

    using var repo = new RepoWithOneCommit();
    using var commit = repo.Repo.LookupCommit(repo.CommitOid);
    using var branch = repo.Repo.CreateBranch(branchName, commit, false);

    Assert.False(branch.IsBranchCheckedOut());
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
  }
}
