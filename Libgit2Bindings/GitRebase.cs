using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal sealed class GitRebase : IGitRebase
{
  public libgit2.GitRebase NativeGitRebase { get; }
  private readonly DisposableCollection _disposables;

  public GitRebase(libgit2.GitRebase nativeGitRebase, DisposableCollection disposables)
  {
    NativeGitRebase = nativeGitRebase;
    _disposables = disposables;
  }

  public GitRebaseOperation? Next()
  {
    var res = libgit2.rebase.GitRebaseNext(out var operation, NativeGitRebase);
    if (res == (int)libgit2.GitErrorCode.GIT_ITEROVER)
    {
      return null;
    }
    CheckLibgit2.Check(res, "Unable to get next rebase operation");
    return operation is null ? null : GitRebaseOperationMapper.FromNative(operation);
  }

  public GitOid Commit(IGitSignature? author, IGitSignature committer, string? message)
  {
    var managedAuthor = GittelObjects.Downcast<GitSignature>(author);
    var managedCommitter = GittelObjects.Downcast<GitSignature>(committer);
    var res = libgit2.rebase.GitRebaseCommit(
      out var commitOid, NativeGitRebase, managedAuthor?.NativeGitSignature,
      managedCommitter?.NativeGitSignature, null, message);
    CheckLibgit2.Check(res, "Unable to commit rebase");
    using (commitOid)
    {
      return GitOidMapper.FromNative(commitOid);
    }
  }

  public void Finish(IGitSignature? signature)
  {
    var managedSignature = GittelObjects.Downcast<GitSignature>(signature);
    var res = libgit2.rebase.GitRebaseFinish(NativeGitRebase, managedSignature?.NativeGitSignature);
    CheckLibgit2.Check(res, "Unable to finish rebase");
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      _disposables.Dispose();
      libgit2.rebase.GitRebaseFree(NativeGitRebase);
      _disposedValue = true;
    }
  }

  ~GitRebase()
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
