namespace Libgit2Bindings;

internal class GitReference : IGitReference
{
  public libgit2.GitReference NativeGitReference { get; }

  public GitReference(libgit2.GitReference nativeGitReference)
  {
    NativeGitReference = nativeGitReference;
  }

  public string BranchName()
  {
    var res = libgit2.branch.GitBranchName(out var name, NativeGitReference);
    CheckLibgit2.Check(res, "Unable to get branch name");
    return name;
  }

  public void DeleteBranch()
  {
    var res = libgit2.branch.GitBranchDelete(NativeGitReference);
    CheckLibgit2.Check(res, "Unable to delete branch");
  }

  public bool IsBranchCheckedOut()
  {
    var res = libgit2.branch.GitBranchIsCheckedOut(NativeGitReference);
    if (res < 0)
    {
      CheckLibgit2.Check(res, "Unable to check if branch is checked out");
    }
    return res == 1;
  }

  public bool IsBranchHead()
  {
    var res = libgit2.branch.GitBranchIsHead(NativeGitReference);
    if (res < 0)
    {
      CheckLibgit2.Check(res, "Unable to check if branch is head");
    }
    return res == 1;
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.refs.GitReferenceFree(NativeGitReference);
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
