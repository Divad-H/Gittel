using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;
using System.Runtime.InteropServices;

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

  public GitOid? Exists(string shortSha)
  {
    UInt16 shortShaLength = (UInt16)shortSha.Length;
    if (shortSha.Length % 2 != 0)
    {
      shortSha += "0";
    }
    var shortId = Convert.FromHexString(shortSha);
    return Exists(shortId, shortShaLength);
  }

  public GitOid? Exists(byte[] shortId, UInt16 shortIdLength)
  {
    GitOid gitOid = GitOidMapper.FromShortId(shortId);
    using var nativeOid = GitOidMapper.ToNative(gitOid);
    var res = libgit2.odb.GitOdbExistsPrefix(
      out var fullOid, NativeGitOdb, nativeOid, (UIntPtr)shortIdLength);
    using (fullOid)
    {
      if (res == 0)
      {
        return GitOidMapper.FromNative(fullOid);
      }  
      if (res == (int)libgit2.GitErrorCode.GIT_ENOTFOUND)
      {
        return null;
      }
      CheckLibgit2.Check(res, "Unable to check if object exists");
      return null;
    }
  }

  public IReadOnlyList<(GitOid oid, GitObjectType type)> ExpandIds(
    IEnumerable<(string shortId, GitObjectType type)> shortIds)
  {
    return ExpandIds(shortIds.Select(x => 
    {
      var shortSha = x.shortId;
      UInt16 shortShaLength = (UInt16)shortSha.Length;
      if (shortSha.Length % 2 != 0)
      {
        shortSha += "0";
      }
      return (Convert.FromHexString(shortSha), shortShaLength, x.type);
    }));
  }

  public IReadOnlyList<(GitOid oid, GitObjectType type)> ExpandIds(
    IEnumerable<(byte[] shortId, UInt16 shortIdLength, GitObjectType type)> shortIds)
  {
    var nativeShortIds = shortIds.Select(x =>
    {
      var res = new libgit2.GitOdbExpandId.__Internal()
      {
        id = new libgit2.GitOid.__Internal(),
        length = x.shortIdLength,
        type = x.type.ToNative()
      };
      unsafe
      {
        Marshal.Copy(x.shortId, 0, new IntPtr(res.id.id), Math.Min(GitOid.Size, x.shortId.Length));
      }
      return res;
    }).ToArray();

    var handle = GCHandle.Alloc(nativeShortIds, GCHandleType.Pinned);
    try
    {
      var res = libgit2.odb.__Internal.GitOdbExpandIds(
        NativeGitOdb.__Instance, handle.AddrOfPinnedObject(), (UIntPtr)nativeShortIds.Length);
      CheckLibgit2.Check(res, "Unable to expand ids");

      unsafe
      {
        return nativeShortIds
        .Select(x => (GitOidMapper.FromNativePtr(new IntPtr(&x.id)), x.type.ToManaged()))
        .ToList();
      }
    }
    finally
    {
      handle.Free();
    }
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
