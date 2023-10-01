namespace Libgit2Bindings;

internal sealed class GitCommit : IGitCommit
{
  private readonly libgit2.GitCommit _nativeGitCommit;
  public libgit2.GitCommit NativeGitCommit => _nativeGitCommit;

  public GitCommit(libgit2.GitCommit nativeGitCommit)
  {
    _nativeGitCommit = nativeGitCommit;
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.commit.GitCommitFree(_nativeGitCommit);
      _disposedValue = true;
    }
  }

  ~GitCommit()
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
