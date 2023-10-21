namespace Libgit2Bindings;

internal class GitCertificate : IGitCertificate
{
  public libgit2.GitCert NativeGitCertificate { get; }

  public GitCertificate(libgit2.GitCert nativeGitCertificate)
  {
    NativeGitCertificate = nativeGitCertificate;
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      NativeGitCertificate.Dispose();
    }
  }

  ~GitCertificate()
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
