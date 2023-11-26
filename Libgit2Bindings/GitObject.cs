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

  public IGitObject Duplicate()
  {
    var res = libgit2.@object.GitObjectDup(out var nativeGitObject, NativeGitObject);
    CheckLibgit2.Check(res, "Unable to duplicate object");
    return new GitObject(nativeGitObject);
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
