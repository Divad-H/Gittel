namespace Libgit2Bindings;

internal class GitReference : IGitReference
{
  private readonly libgit2.GitReference _nativeGitReference;

  public GitReference(libgit2.GitReference nativeGitReference)
  {
    _nativeGitReference = nativeGitReference;
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.refs.GitReferenceFree(_nativeGitReference);
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
