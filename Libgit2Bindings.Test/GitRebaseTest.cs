using Libgit2Bindings.Test.TestData;

namespace Libgit2Bindings.Test;

public sealed class GitRebaseTest
{
  [Fact]
  public void CanRebaseCommit()
  {
    using var repoWithTwoBranches = new RepoWithTwoBranches();
    var repo = repoWithTwoBranches.Repo;

    using var branch = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondBranchCommitOid);
    using var onto = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondCommitOid);

    using var rebase = repo.StartRebase(branch, null, onto, null);
    var operation = rebase.Next();
    Assert.NotNull(operation);
    Assert.Equal(repoWithTwoBranches.SecondBranchCommitOid, operation.Id);
    Assert.Equal(GitRebaseOperationType.Pick, operation.Type);

    var commitId = rebase.Commit(null, repoWithTwoBranches.Signature);

    operation = rebase.Next();
    Assert.Null(operation);

    rebase.Finish(repoWithTwoBranches.Signature);
  }

  [Fact]
  public void CanRebaseCommitWithCreateCommitCallback()
  {
    using var repoWithTwoBranches = new RepoWithTwoBranches();
    var repo = repoWithTwoBranches.Repo;

    using var branch = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondBranchCommitOid);
    using var onto = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondCommitOid);

    GitOid? createdId = null;
    using var rebase = repo.StartRebase(branch, null, onto, new()
    {
      CommitCreateCallback = (out GitOid? createdCommitId,
        IGitSignature author, IGitSignature committer, string message,
        IGitTree tree, IReadOnlyCollection<IGitCommit> parents) =>
      {
        createdId = repo.CreateCommit(
          "HEAD",
          repoWithTwoBranches.Signature, repoWithTwoBranches.Signature,
          message,
          tree,
          parents);
        createdCommitId = createdId;
        return GitOperationContinuationWithPassthrough.Continue;
      }
    });
    var operation = rebase.Next();
    Assert.NotNull(operation);
    Assert.Equal(repoWithTwoBranches.SecondBranchCommitOid, operation.Id);
    Assert.Equal(GitRebaseOperationType.Pick, operation.Type);

    var commitId = rebase.Commit(null, repoWithTwoBranches.Signature);
    Assert.NotNull(commitId);
    Assert.Equal(createdId, commitId);

    operation = rebase.Next();
    Assert.Null(operation);

    rebase.Finish(repoWithTwoBranches.Signature);
  }

  [Fact]
  public void CanOpenRebaseWithCreateCommitCallback()
  {
    using var repoWithTwoBranches = new RepoWithTwoBranches();
    var repo = repoWithTwoBranches.Repo;

    using var branch = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondBranchCommitOid);
    using var onto = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondCommitOid);

    using var rebaseOrig = repo.StartRebase(branch, null, onto, null);

    GitOid? createdId = null;
    using var rebase = repo.OpenRebase(new()
    {
      CommitCreateCallback = (out GitOid? createdCommitId,
        IGitSignature author, IGitSignature committer, string message,
        IGitTree tree, IReadOnlyCollection<IGitCommit> parents) =>
      {
        createdId = repo.CreateCommit(
          "HEAD",
          repoWithTwoBranches.Signature, repoWithTwoBranches.Signature,
          message,
          tree,
          parents);
        createdCommitId = createdId;
        return GitOperationContinuationWithPassthrough.Continue;
      }
    });

    Assert.NotNull(rebaseOrig.OriginalHeadId);
    Assert.Equal(rebaseOrig.OriginalHeadId, rebase.OriginalHeadId);
    Assert.Equal(rebaseOrig.OriginalHeadName, rebase.OriginalHeadName);
    Assert.NotNull(rebaseOrig.OntoId);
    Assert.Equal(rebaseOrig.OntoId, rebase.OntoId);
    Assert.NotNull(rebaseOrig.OntoName);
    Assert.Equal(rebaseOrig.OntoName, rebase.OntoName);

    var operation = rebase.Next();
    Assert.NotNull(operation);
    Assert.Equal(repoWithTwoBranches.SecondBranchCommitOid, operation.Id);
    Assert.Equal(GitRebaseOperationType.Pick, operation.Type);

    var commitId = rebase.Commit(null, repoWithTwoBranches.Signature);
    Assert.NotNull(commitId);
    Assert.Equal(createdId, commitId);

    operation = rebase.Next();
    Assert.Null(operation);

    rebase.Finish(repoWithTwoBranches.Signature);
  }

  [Fact]
  public void CanAbortRebase()
  {
    using var repoWithTwoBranches = new RepoWithTwoBranches();
    var repo = repoWithTwoBranches.Repo;

    using var branch = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondBranchCommitOid);
    using var onto = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondCommitOid);

    using var rebase = repo.StartRebase(branch, null, onto, null);
    var operation = rebase.Next();
    Assert.NotNull(operation);
    Assert.Equal(repoWithTwoBranches.SecondBranchCommitOid, operation.Id);
    Assert.Equal(GitRebaseOperationType.Pick, operation.Type);

    var commitId = rebase.Commit(null, repoWithTwoBranches.Signature);

    operation = rebase.Next();
    Assert.Null(operation);

    rebase.Abort();
  }

  [Fact]
  public void CanGetInMemoryIndex()
  {
    using var repoWithTwoBranches = new RepoWithTwoBranches();
    var repo = repoWithTwoBranches.Repo;

    using var branch = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondBranchCommitOid);
    using var onto = repo.AnnotatedCommitLookup(repoWithTwoBranches.SecondCommitOid);

    using var rebase = repo.StartRebase(branch, null, onto, new()
    {
      InMemory = true
    });
    var operation = rebase.Next();
    Assert.NotNull(operation);
    Assert.Equal(repoWithTwoBranches.SecondBranchCommitOid, operation.Id);
    Assert.Equal(GitRebaseOperationType.Pick, operation.Type);

    var commitId = rebase.Commit(null, repoWithTwoBranches.Signature);

    using var index = rebase.GetInMemoryIndex();
    Assert.NotNull(index);
    Assert.False(index.HasConflicts());

    operation = rebase.Next();
    Assert.Null(operation);

    rebase.Abort();
  }
}
