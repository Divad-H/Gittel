using Libgit2Bindings.Mappers;

namespace Libgit2Bindings;

internal class GitObject(libgit2.GitObject nativeGitObject) : IGitObject
{
  public libgit2.GitObject NativeGitObject { get; } = nativeGitObject;

  public GitOid Id
  {
    get
    {
      using var id = libgit2.@object.GitObjectId(NativeGitObject);
      return GitOidMapper.FromNative(id);
    }
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.@object.GitObjectFree(NativeGitObject);
      _disposedValue = true;
    }
  }

  ~GitObject()
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
