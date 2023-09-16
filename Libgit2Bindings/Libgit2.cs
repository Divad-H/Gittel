namespace Libgit2Bindings;

internal class Libgit2 : ILibgit2Internal, IDisposable
{
  public Libgit2()
  {
    libgit2.global.GitLibgit2Init();
  }

  public IGitRepository GitRepositoryOpen(string path)
  {
    var res = libgit2.repository.GitRepositoryOpen(out var repo, path);
    CheckLibgit2.Check(res, "Unable to open repository '{0}'", path);
    return new GitRepository(repo, this);
  }

  public void GitRepositoryFree(libgit2.GitRepository repo)
  {
    libgit2.repository.GitRepositoryFree(repo);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.global.GitLibgit2Shutdown();
      _disposedValue = true;
    }
  }

  ~Libgit2()
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
