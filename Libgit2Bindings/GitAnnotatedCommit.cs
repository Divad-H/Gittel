using Libgit2Bindings.Mappers;

namespace Libgit2Bindings;

internal sealed class GitAnnotatedCommit : IGitAnnotatedCommit
{
  private readonly libgit2.GitAnnotatedCommit _nativeAnnotatedCommit;

  public GitAnnotatedCommit(libgit2.GitAnnotatedCommit nativeAnnotatedCommit)
  {
    _nativeAnnotatedCommit = nativeAnnotatedCommit;
  }

  public GitOid GetId()
  {
    using var nativeId = libgit2.annotated_commit.GitAnnotatedCommitId(_nativeAnnotatedCommit);
    return GitOidMapper.FromNative(nativeId);
  }

  public string GetRefName()
  {
    return libgit2.annotated_commit.GitAnnotatedCommitRef(_nativeAnnotatedCommit);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.annotated_commit.GitAnnotatedCommitFree(_nativeAnnotatedCommit);
      _disposedValue = true;
    }
  }

  ~GitAnnotatedCommit()
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
