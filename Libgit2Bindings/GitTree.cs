using Libgit2Bindings.Mappers;

namespace Libgit2Bindings;

internal class GitTree : IGitTree
{
  private readonly libgit2.GitTree _nativeGitTree;
  public libgit2.GitTree NativeGitTree => _nativeGitTree;

  public GitTree(libgit2.GitTree nativeGitTree)
  {
    _nativeGitTree = nativeGitTree;
  }

  public GitOid GetId()
  {
    var res = libgit2.tree.GitTreeId(_nativeGitTree);
    return GitOidMapper.FromNative(res);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.tree.GitTreeFree(_nativeGitTree);
      _disposedValue = true;
    }
  }

  ~GitTree()
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
