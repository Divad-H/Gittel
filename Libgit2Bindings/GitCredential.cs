namespace Libgit2Bindings;

internal sealed class GitCredential : IGitCredential
{
  private bool _ownsNativeInstance = true;
  public libgit2.GitCredential NativeGitCredential { get; }

  public GitCredential(libgit2.GitCredential nativeGitCredential)
  {
    NativeGitCredential = nativeGitCredential;
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
        libgit2.credential.GitCredentialFree(NativeGitCredential);
      }
      NativeGitCredential.Dispose();
    }
  }

  ~GitCredential()
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
