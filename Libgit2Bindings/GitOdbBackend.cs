namespace Libgit2Bindings;

internal sealed class GitOdbBackend : IGitOdbBackend
{
  public libgit2.GitOdbBackend NativeInstance { get; }

  private bool _ownsNativeInstance;

  public GitOdbBackend(
    libgit2.GitOdbBackend nativeInstance,
    bool ownsNativeInstance)
  {
    NativeInstance = nativeInstance ?? throw new ArgumentNullException(nameof(nativeInstance));
    _ownsNativeInstance = ownsNativeInstance;
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
        NativeInstance.Dispose();
      }
      _disposedValue = true;
    }
  }

  ~GitOdbBackend()
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
