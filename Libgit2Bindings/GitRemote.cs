namespace Libgit2Bindings;

internal sealed class GitRemote : IGitRemote
{
  public libgit2.GitRemote NativeGitRemote { get; }
  public GitRemote(libgit2.GitRemote nativeGitRemote)
  {
    NativeGitRemote = nativeGitRemote;
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.remote.GitRemoteFree(NativeGitRemote);
      _disposedValue = true;
    }
  }

  ~GitRemote()
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
