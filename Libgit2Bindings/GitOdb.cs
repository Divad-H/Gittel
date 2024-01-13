using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal class GitOdb(libgit2.GitOdb nativeGitOdb) : IGitOdb
{
  public libgit2.GitOdb NativeGitOdb { get; private set; } = nativeGitOdb;

  public GitOid Write(byte[] data, GitObjectType type)
  {
    using var pinnedData = new PinnedBuffer(data);
    var res = libgit2.odb.GitOdbWrite(
      out var oid, NativeGitOdb, pinnedData.Pointer, (UIntPtr)pinnedData.Length, type.ToNative());
    CheckLibgit2.Check(res, "Unable to write to ODB");
    using (oid)
    {
      return GitOidMapper.FromNative(oid);
    }
  }

  public bool Exists(GitOid oid)
  {
    using var nativeOid = GitOidMapper.ToNative(oid);
    var res = libgit2.odb.GitOdbExists(NativeGitOdb, nativeOid);
    return res == 1;
  }

  public bool Exists(GitOid oid, GitOdbLookupFlags flags)
  {
    using var nativeOid = GitOidMapper.ToNative(oid);
    var nativeFlags = flags.ToNative();
    var res = libgit2.odb.GitOdbExistsExt(NativeGitOdb, nativeOid, (uint)nativeFlags);
    return res == 1;
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.odb.GitOdbFree(nativeGitOdb);
      _disposedValue = true;
    }
  }

  ~GitOdb()
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
