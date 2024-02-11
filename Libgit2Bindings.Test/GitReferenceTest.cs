using Libgit2Bindings.Test.TestData;
using Libgit2Bindings.Util;

namespace Libgit2Bindings.Test;

public sealed class GitReferenceTest
{
  [Fact]
  public void CanCreateReference()
  {
    using var repo = new RepoWithOneCommit();
    using var reference = repo.Repo.CreateReference("refs/heads/test", repo.CommitOid, false, "reflog entry");
    Assert.NotNull(reference);
    Assert.Equal("refs/heads/test", reference.Name);
    Assert.Equal(GitReferenceType.Direct, reference.Type);
    Assert.True(reference.IsBranch);
    Assert.False(reference.IsNote);
    Assert.False(reference.IsRemote);
    Assert.False(reference.IsTag);
    Assert.Equal(repo.CommitOid, reference.GetTarget());
  }

  [Fact]
  public void CanCreateMatchingReference()
  {
    using var repo = new RepoWithOneCommit();
    using var reference = repo.Repo.CreateMatchingReference(
      "refs/heads/test", repo.CommitOid, false, GitOid.Zero, "reflog entry");
    Assert.NotNull(reference);
    Assert.Equal("refs/heads/test", reference.Name);
    using var reference2 = repo.Repo.CreateMatchingReference(
      "refs/heads/test", repo.CommitOid, true, repo.CommitOid, "reflog entry");
    Assert.Throws<Libgit2Exception>(() => repo.Repo.CreateMatchingReference(
      "refs/heads/test", repo.CommitOid, true, GitOid.Zero, "reflog entry"));
  }

  [Fact]
  public void CanCreateSymbolicReference()
  {
    using var repo = new RepoWithOneCommit();
    using var reference = repo.Repo.CreateSymbolicReference(
      "refs/heads/test", "refs/heads/master", false, "reflog entry");
    Assert.NotNull(reference);
    Assert.Equal("refs/heads/test", reference.Name);
    Assert.Equal(GitReferenceType.Symbolic, reference.Type);
    Assert.Equal("refs/heads/master", reference.GetSymbolicTarget());
  }

  [Fact]
  public void CanCreateMatchingSymbolicReference()
  {
    using var repo = new RepoWithOneCommit();
    using var reference = repo.Repo.CreateMatchingSymbolicReference(
      "refs/heads/test", "refs/heads/master", true, null, "reflog entry");
    Assert.NotNull(reference);
    Assert.Equal("refs/heads/test", reference.Name);
    using var reference2 = repo.Repo.CreateMatchingSymbolicReference(
      "refs/heads/test", "refs/heads/master", true, "refs/heads/master", null);
    Assert.Throws<Libgit2Exception>(() => repo.Repo.CreateMatchingSymbolicReference(
      "refs/heads/test", "refs/heads/master", true, GitOid.Zero.Sha, null));
  }

  [Fact]
  public void CanLookupReferenceByHumanReadableName()
  {
    using var repo = new RepoWithOneCommit();
    using var reference = repo.Repo.LookupReference("refs/heads/master");
    Assert.NotNull(reference);
    Assert.Equal("refs/heads/master", reference.Name);
    using var reference2 = repo.Repo.LookupReferenceDwim("master");
    Assert.NotNull(reference2);
    Assert.Equal("refs/heads/master", reference2.Name);
    Assert.True(reference.EqualsTo(reference2));
  }

  [Fact]
  public void CanGetOidFromReferenceName()
  {
    using var repo = new RepoWithOneCommit();
    var oid = repo.Repo.ReferenceNameToOid("HEAD");
    Assert.Equal(repo.CommitOid, oid);
  }

  [Fact]
  public void CanEnsureReferenceHasRefLog()
  {
    using var repo = new RepoWithOneCommit();
    repo.Repo.EnsureReferenceHasLog("HEAD");
    Assert.True(repo.Repo.ReferenceHasLog("HEAD"));
  }

  [Fact]
  public void CanIterateReferences()
  {
    using var repo = new RepoWithOneCommit();
    int count = 0;
    repo.Repo.ForEachReference(reference =>
    {
      count++;
      Assert.NotNull(reference);
      Assert.Equal("refs/heads/master", reference.Name);
      return GitOperationContinuation.Continue;
    });
    Assert.Equal(1, count);
  }

  [Fact]
  public void CanIterateReferencesWithIEnumerable()
  {
    using var repo = new RepoWithOneCommit();
    using var disposables = new DisposableCollection();
    var references = repo.Repo.EnumerateReferences().Select(r => r.DisposeWith(disposables)).ToList();
    Assert.Single(references);
    Assert.Equal("refs/heads/master", references[0].Name);
  }

  [Fact]
  public void CanIterateReferencesWithIEnumerableAndGlob()
  {
    using var repo = new RepoWithOneCommit();
    using var disposables = new DisposableCollection();
    var references = repo.Repo.EnumerateReferences("*ster").Select(r => r.DisposeWith(disposables)).ToList();
    Assert.Single(references);
    Assert.Equal("refs/heads/master", references[0].Name);
  }

  [Fact]
  public void CanIterateReferenceNames()
  {
    using var repo = new RepoWithOneCommit();
    var names = repo.Repo.EnumerateReferenceNames().ToList();
    Assert.Single(names);
    Assert.Equal("refs/heads/master", names[0]);
  }

  [Fact]
  public void CanIterateReferenceNamesWithGlob()
  {
    using var repo = new RepoWithOneCommit();
    var names = repo.Repo.EnumerateReferenceNames("*ster").ToList();
    Assert.Single(names);
    Assert.Equal("refs/heads/master", names[0]);
  }

  [Fact]
  public void CanGetReferenceList()
  {
    using var repo = new RepoWithOneCommit();
    var list = repo.Repo.ReferenceList();
    Assert.Single(list);
    Assert.Equal("refs/heads/master", list.First());
  }

  [Fact]
  public void CanIterateReferenceNamesWithCallback()
  {
    using var repo = new RepoWithOneCommit();
    int count = 0;
    repo.Repo.ForEachReferenceName(reference =>
    {
      count++;
      Assert.NotNull(reference);
      Assert.Equal("refs/heads/master", reference);
      return GitOperationContinuation.Continue;
    });
    Assert.Equal(1, count);
  }

  [Fact]
  public void CanIterateReferenceNamesWithGlobAndCallback()
  {
    using var repo = new RepoWithOneCommit();
    int count = 0;
    repo.Repo.ForEachReferenceName("*ster", reference =>
    {
      count++;
      Assert.NotNull(reference);
      Assert.Equal("refs/heads/master", reference);
      return GitOperationContinuation.Continue;
    });
    Assert.Equal(1, count);
  }

  [Fact]
  public void CanRemoveReference()
  {
    using var repo = new RepoWithOneCommit();
    repo.Repo.RemoveReference("refs/heads/master");
    Assert.Throws<Libgit2Exception>(() => repo.Repo.LookupReference("refs/heads/master"));
  }

  [Fact]
  public void CanSetReferenceTarget()
  {
    using var repo = new RepoWithTwoCommits();
    using var reference = repo.Repo.LookupReference("refs/heads/master");
    Assert.Equal(repo.SecondCommitOid, reference.GetTarget());
    using var movedReference = reference.SetTarget(repo.FirstCommitOid, "reflog entry");
    Assert.Equal(repo.FirstCommitOid, movedReference.GetTarget());
  }

  [Fact]
  public void CanSetSymbolicReferenceTarget()
  {
    using var repo = new RepoWithTwoCommits();
    using var reference = repo.Repo.CreateReference(
      "refs/heads/test", repo.FirstCommitOid, false, "reflog entry");
    using var symbolicReference = repo.Repo.CreateSymbolicReference(
      "refs/heads/test2", "refs/heads/master", false, "reflog entry");
    using var movedReference = symbolicReference.SetSymbolicTarget("refs/heads/test", "reflog entry");
    Assert.Equal("refs/heads/test", movedReference.GetSymbolicTarget());
    using var resolvedReference = movedReference.Resolve();
    Assert.Equal(repo.FirstCommitOid, resolvedReference.GetTarget());
    Assert.True(resolvedReference.EqualsTo(reference));

    using var peeledReferenceTree = movedReference.Peel(GitObjectType.Tree);
    Assert.Equal(repo.FirstTreeOid, peeledReferenceTree.Id);
  }
  [Fact]
  public void CanDeleteReference()
  {
    using var repo = new RepoWithOneCommit();
    using var reference = repo.Repo.LookupReference("refs/heads/master");
    reference.DeleteFromDisk();
    Assert.Throws<Libgit2Exception>(() => repo.Repo.LookupReference("refs/heads/master"));
  }

  [Fact]
  public void CanDuplicateReference()
  {
    using var repo = new RepoWithOneCommit();
    using var reference = repo.Repo.LookupReference("refs/heads/master");
    using var duplicate = reference.Duplicate();
    Assert.True(reference.EqualsTo(duplicate));
    using var owner = reference.GetOwner();
    Assert.Equal(owner.GetPath(), repo.Repo.GetPath());
  }

  [Fact]
  public void CanRenameReference()
  {
    using var repo = new RepoWithOneCommit();
    using var reference = repo.Repo.LookupReference("refs/heads/master");
    reference.Rename("refs/heads/test", true, "reflog message");
    Assert.Throws<Libgit2Exception>(() => repo.Repo.LookupReference("refs/heads/master"));
    using var renamedReference = repo.Repo.LookupReference("refs/heads/test");
    Assert.NotNull(renamedReference);
    Assert.Equal("refs/heads/test", renamedReference.Name);
    Assert.Equal("test", renamedReference.GetShorthand());
  }

  [Fact]
  public void CanVerifyReferenceName()
  {
    using var libgit2 = new Libgit2();
    Assert.True(libgit2.ReferenceNameIsValid("refs/heads/master"));
    Assert.False(libgit2.ReferenceNameIsValid("*"));
  }

  [Fact]
  public void CanNormalizeReferenceName()
  {
    using var libgit2 = new Libgit2();
    Assert.Equal("refs/heads/master", 
      libgit2.NormalizeReferenceName("refs/heads/master", GitReferenceFormat.Normal));
    Assert.Equal("refs/heads/master", 
      libgit2.NormalizeReferenceName("refs//heads/master", GitReferenceFormat.Normal));
  }
}
