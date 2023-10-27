namespace Libgit2Bindings;

internal sealed class GitRemote : IGitRemote
{
  private bool _ownsNativeInstance;
  public libgit2.GitRemote NativeGitRemote { get; }
  public GitRemote(libgit2.GitRemote nativeGitRemote, bool ownsNativeInstance)
  {
    _ownsNativeInstance = ownsNativeInstance;
    NativeGitRemote = nativeGitRemote;
  }

  public void ReleaseNativeInstance()
  {
    _ownsNativeInstance = false;
  }

  public int SetUrl(string url)
  {
    throw new NotImplementedException();
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (_ownsNativeInstance)
      {
        libgit2.remote.GitRemoteFree(NativeGitRemote);
      }
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
