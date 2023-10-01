using Libgit2Bindings.Mappers;

namespace Libgit2Bindings;

internal sealed class GitIndex : IGitIndex
{
  private readonly libgit2.GitIndex _nativeGitIndex;

  public GitIndex(libgit2.GitIndex nativeGitIndex)
  {
    _nativeGitIndex = nativeGitIndex;
  }

  public void AddByPath(string path)
  {
    var res = libgit2.index.GitIndexAddBypath(_nativeGitIndex, path);
    CheckLibgit2.Check(res, "Unable to add path '{0}' to index", path);
  }

  public GitOid WriteTree()
  {
    var res = libgit2.index.GitIndexWriteTree(out var treeOid, _nativeGitIndex);
    using (treeOid)
    {
      CheckLibgit2.Check(res, "Unable to write tree");
      return GitOidMapper.FromNative(treeOid);
    }
  }

  public void Write()
  {
    var res = libgit2.index.GitIndexWrite(_nativeGitIndex);
    CheckLibgit2.Check(res, "Unable to write index");
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.index.GitIndexFree(_nativeGitIndex);
      _disposedValue = true;
    }
  }

  ~GitIndex()
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
