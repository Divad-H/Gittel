namespace Libgit2Bindings;

internal class GitReference : IGitReference
{
  private readonly libgit2.GitReference _nativeGitReference;

  public GitReference(libgit2.GitReference nativeGitReference)
  {
    _nativeGitReference = nativeGitReference;
  }

  public string BranchName()
  {
    var res = libgit2.branch.GitBranchName(out var name, _nativeGitReference);
    CheckLibgit2.Check(res, "Unable to get branch name");
    return name;
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
