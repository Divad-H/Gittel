namespace Libgit2Bindings;

internal sealed class GitRepository : IGitRepository
{
  private readonly libgit2.GitRepository _nativeGitRepository;

  public GitRepository(libgit2.GitRepository nativeGitRepository)
  {
    _nativeGitRepository = nativeGitRepository;
  }

  public IGitReference GetHead()
  {
    var res = libgit2.repository.GitRepositoryHead(out var head, _nativeGitRepository);
    CheckLibgit2.Check(res, "Unable to get HEAD");
    return new GitReference(head);
  }

  public string GetPath()
  {
    return libgit2.repository.GitRepositoryPath(_nativeGitRepository);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.repository.GitRepositoryFree(_nativeGitRepository);
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
