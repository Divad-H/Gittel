using Libgit2Bindings.Mappers;

namespace Libgit2Bindings;

internal class GitTree(libgit2.GitTree nativeGitTree, bool ownsNativeInstance = true) : IGitTree
{
  private readonly libgit2.GitTree _nativeGitTree = nativeGitTree;
  private readonly bool _ownsNativeInstance = ownsNativeInstance;
  public libgit2.GitTree NativeGitTree => _nativeGitTree;

  public GitOid GetId()
  {
    using var res = libgit2.tree.GitTreeId(_nativeGitTree);
    return GitOidMapper.FromNative(res);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (_ownsNativeInstance)
      {
        libgit2.tree.GitTreeFree(_nativeGitTree);
      }
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
