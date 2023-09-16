using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal class Libgit2 : ILibgit2, IDisposable
{
  public Libgit2()
  {
    libgit2.global.GitLibgit2Init();
  }

  public IGitRepository GitRepositoryOpen(string path)
  {
    var res = libgit2.repository.GitRepositoryOpen(out var repo, path);
    CheckLibgit2.Check(res, "Unable to open repository '{0}'", path);
    return new GitRepository(repo);
  }

  public string DiscoverRepository(string startPath, bool acrossFilesystem, string[] ceilingDirectories)
  {
    var res = libgit2.repository.GitRepositoryDiscover(
      out var repoPath, 
      startPath, 
      acrossFilesystem ? 1 : 0, 
      ceilingDirectories.Any() ? string.Join((char)libgit2.PathListSeparator.GIT_PATH_LIST_SEPARATOR, ceilingDirectories) : null);

    using (repoPath)
    {
      CheckLibgit2.Check(res, "Unable to discover repository '{0}'", startPath);
      return StringUtil.ToString(repoPath);
    }
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
