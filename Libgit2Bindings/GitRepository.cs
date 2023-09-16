namespace Libgit2Bindings;

internal sealed class GitRepository : IGitRepository
{
  private readonly ILibgit2Internal _libgit2;
  private readonly libgit2.GitRepository _nativeGitRepository;

  public GitRepository(libgit2.GitRepository nativeGitRepository, ILibgit2Internal libgit2)
  {
    _libgit2 = libgit2;
    _nativeGitRepository = nativeGitRepository;
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      _libgit2.GitRepositoryFree(_nativeGitRepository);
      _disposedValue = true;
    }
  }

  ~GitRepository()
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
