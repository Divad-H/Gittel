using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal sealed class GitOdbObject(libgit2.GitOdbObject nativeGitOdbObject) : IGitOdbObject
{
  public libgit2.GitOdbObject NativeGitOdbObject { get; } = nativeGitOdbObject;

  public GitOid Id
  {
    get
    {
      using var id = libgit2.odb.GitOdbObjectId(NativeGitOdbObject);
      return GitOidMapper.FromNative(id);
    }
  }

  public GitObjectType Type
  {
    get
    {
      var res = libgit2.odb.GitOdbObjectType(NativeGitOdbObject);
      return res.ToManaged();
    }
  }

  public byte[] Data
  {
    get
    {
      var res = libgit2.odb.GitOdbObjectData(NativeGitOdbObject);
      if (res == IntPtr.Zero)
      {
        return [];
      }
      return StringUtil.ToArray(res, Size);
    }
  }

  public UIntPtr Size
  {
    get
    {
      var res = libgit2.odb.GitOdbObjectSize(NativeGitOdbObject);
      return (UIntPtr)res;
    }
  }

  public IGitOdbObject Duplicate()
  {
    var res = libgit2.odb.GitOdbObjectDup(out var nativeGitOdbObject, NativeGitOdbObject);
    CheckLibgit2.Check(res, "Unable to duplicate ODB object");
    return new GitOdbObject(nativeGitOdbObject);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.odb.GitOdbObjectFree(NativeGitOdbObject);
      _disposedValue = true;
    }
  }

  ~GitOdbObject()
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
