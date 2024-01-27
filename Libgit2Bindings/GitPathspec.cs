namespace Libgit2Bindings;

internal sealed class GitPathspec : IGitPathspec
{
  public libgit2.GitPathspec NativeGitPathspec { get; }

  public GitPathspec(libgit2.GitPathspec nativeGitPathspec)
  {
    NativeGitPathspec = nativeGitPathspec;
  }

  public bool MatchesPath(GitPathspecFlags flags, string path)
  {
    var res = libgit2.pathspec.GitPathspecMatchesPath(NativeGitPathspec, (UInt32)flags, path);
    if (res < 0)
    {
      CheckLibgit2.Check(res, "Unable to match path '{0}'", path);
    }
    return res == 1;
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.pathspec.GitPathspecFree(NativeGitPathspec);
      _disposedValue = true;
    }
  }

  ~GitPathspec()
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
