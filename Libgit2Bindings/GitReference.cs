using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal class GitReference : IGitReference
{
  public libgit2.GitReference NativeGitReference { get; private set; }

  public bool IsBranch => libgit2.refs.GitReferenceIsBranch(NativeGitReference) == 1;

  public bool IsNote => libgit2.refs.GitReferenceIsNote(NativeGitReference) == 1;

  public bool IsRemote => libgit2.refs.GitReferenceIsRemote(NativeGitReference) == 1;

  public bool IsTag => libgit2.refs.GitReferenceIsTag(NativeGitReference) == 1;

  public string Name => libgit2.refs.GitReferenceName(NativeGitReference);

  public GitReferenceType Type => (GitReferenceType)libgit2.refs.GitReferenceType(NativeGitReference);

  public GitReference(libgit2.GitReference nativeGitReference)
  {
    NativeGitReference = nativeGitReference;
  }

  public string BranchName()
  {
    var res = libgit2.branch.GitBranchName(out var name, NativeGitReference);
    CheckLibgit2.Check(res, "Unable to get branch name");
    return name;
  }

  public void DeleteBranch()
  {
    var res = libgit2.branch.GitBranchDelete(NativeGitReference);
    CheckLibgit2.Check(res, "Unable to delete branch");
  }

  public void MoveBranch(string newBranchName, bool force)
  {
    var res = libgit2.branch.GitBranchMove(
      out var newReference, NativeGitReference, newBranchName, force ? 1 : 0);
    CheckLibgit2.Check(res, "Unable to move branch");
    libgit2.refs.GitReferenceFree(NativeGitReference);
    NativeGitReference = newReference;
  }

  public bool IsBranchCheckedOut()
  {
    var res = libgit2.branch.GitBranchIsCheckedOut(NativeGitReference);
    if (res < 0)
    {
      CheckLibgit2.Check(res, "Unable to check if branch is checked out");
    }
    return res == 1;
  }

  public bool IsBranchHead()
  {
    var res = libgit2.branch.GitBranchIsHead(NativeGitReference);
    if (res < 0)
    {
      CheckLibgit2.Check(res, "Unable to check if branch is head");
    }
    return res == 1;
  }

  public IGitReference GetUpstream()
  {
    var res = libgit2.branch.GitBranchUpstream(out var upstream, NativeGitReference);
    CheckLibgit2.Check(res, "Unable to get upstream branch");
    return new GitReference(upstream);
  }

  public void SetUpstream(string? branchName)
  {
    var res = libgit2.branch.GitBranchSetUpstream(NativeGitReference, branchName);
    CheckLibgit2.Check(res, "Unable to set upstream branch");
  }

  public string? GetSymbolicTarget()
  {
    return libgit2.refs.GitReferenceSymbolicTarget(NativeGitReference);
  }

  public GitOid? GetTarget()
  {
    using var target = libgit2.refs.GitReferenceTarget(NativeGitReference);
    return target is null ? null : GitOidMapper.FromNative(target);
  }

  public IGitReference SetTarget(GitOid targetId, string logMessage)
  {
    using var nativeOid = targetId.ToNative();
    var res = libgit2.refs.GitReferenceSetTarget(
      out var newRef, NativeGitReference, nativeOid, logMessage);
    CheckLibgit2.Check(res, "Unable to set target");
    return new GitReference(newRef);
  }

  public IGitReference SetSymbolicTarget(string target, string logMessage)
  {
    var res = libgit2.refs.GitReferenceSymbolicSetTarget(
      out var newRef, NativeGitReference, target, logMessage);
    CheckLibgit2.Check(res, "Unable to set symbolic target");
    return new GitReference(newRef);
  }

  public IGitReference Resolve()
  {
    var res = libgit2.refs.GitReferenceResolve(out var resolved, NativeGitReference);
    CheckLibgit2.Check(res, "Unable to resolve reference");
    return new GitReference(resolved);
  }

  public IGitObject Peel(GitObjectType type)
  {
    var res = libgit2.refs.GitReferencePeel(out var obj, NativeGitReference, type.ToNative());
    CheckLibgit2.Check(res, "Unable to peel reference");
    return new GitObject(obj);
  }

  public GitOid? PeelTarget()
  {
    using var oid = libgit2.refs.GitReferenceTargetPeel(NativeGitReference);
    return oid is null ? null : GitOidMapper.FromNative(oid);
  }

  public bool EqualsTo(IGitReference reference)
  {
    var other = GittelObjects.DowncastNonNull<GitReference>(reference);
    return libgit2.refs.GitReferenceCmp(NativeGitReference, other.NativeGitReference) == 0;
  }

  public void DeleteFromDisk()
  {
    var res = libgit2.refs.GitReferenceDelete(NativeGitReference);
    CheckLibgit2.Check(res, "Unable to delete reference from disk");
  }

  public IGitReference Duplicate()
  {
    var res = libgit2.refs.GitReferenceDup(out var reference, NativeGitReference);
    CheckLibgit2.Check(res, "Unable to duplicate reference");
    return new GitReference(reference);
  }

  public IGitRepository GetOwner()
  {
    var repository = libgit2.refs.GitReferenceOwner(NativeGitReference);
    return new GitRepository(repository, false);
  }

  public IGitReference Rename(string newName, bool force, string logMessage)
  {
    var res = libgit2.refs.GitReferenceRename(out var newRef, 
      NativeGitReference, newName, force ? 1 : 0, logMessage);
    CheckLibgit2.Check(res, "Unable to rename reference");
    return new GitReference(newRef);
  }

  public string GetShorthand()
  {
    return libgit2.refs.GitReferenceShorthand(NativeGitReference);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.refs.GitReferenceFree(NativeGitReference);
      _disposedValue = true;
    }
  }

  ~GitReference()
  {
    Dispose(disposing: false);
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
  #endregion
}
