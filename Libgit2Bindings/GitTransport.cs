namespace Libgit2Bindings;

internal class GitTransport : IGitTransport
{
  private bool _ownsNativeInstance = true;
  public libgit2.GitTransport NativeGitTransport { get; }

  public GitTransport(libgit2.GitTransport nativeGitTransport)
  {
    NativeGitTransport = nativeGitTransport;
  }

  public void Release()
  {
    _ownsNativeInstance = false;
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (_ownsNativeInstance)
      {
        NativeGitTransport.Free?.Invoke(NativeGitTransport.__Instance);
      }
      NativeGitTransport.Dispose();
    }
  }

  ~GitTransport()
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
