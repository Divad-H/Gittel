﻿using Libgit2Bindings.Callbacks;
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

  public void ForEachOid(Func<GitOid, GitOperationContinuation> callback)
  {
    using var callbackImpl = new ForEachOidCallbackImpl(callback);
    var res = libgit2.odb.GitOdbForeach(
      NativeGitOdb, ForEachOidCallbackImpl.GitForEachOidCb, callbackImpl.Payload);
    CheckLibgit2.Check(res, "Unable to iterate over ODB");
  }

  public UIntPtr GetNumBackends()
  {
    return (UIntPtr)libgit2.odb.__Internal.GitOdbNumBackends(NativeGitOdb?.__Instance ?? IntPtr.Zero);
  }

  public IGitOdbBackend GetBackend(UIntPtr index)
  {
    var res = libgit2.odb.GitOdbGetBackend(out var backend, NativeGitOdb, index);
    CheckLibgit2.Check(res, "Unable to get ODB backend");
    return new GitOdbBackend(backend, true);
  }

  public void AddBackend(IGitOdbBackend backend, int priority)
  {
    var nativeBackend = GittelObjects.DowncastNonNull<GitOdbBackend>(backend);
    var res = libgit2.odb.GitOdbAddBackend(NativeGitOdb, nativeBackend.NativeInstance, priority);
    CheckLibgit2.Check(res, "Unable to add ODB backend");
  }

  public void AddAlternativeOnDisk(string path)
  {
    var res = libgit2.odb.GitOdbAddDiskAlternate(NativeGitOdb, path);
    CheckLibgit2.Check(res, "Unable to add ODB disk alternate");
  }

  public (nuint size, GitObjectType type) ReadHeader(GitOid oid)
  {
    using var nativeOid = GitOidMapper.ToNative(oid);
    UInt64 size = 0;
    libgit2.GitObjectT type = 0;
    var res = libgit2.odb.GitOdbReadHeader(out size, out type, NativeGitOdb, nativeOid);
    CheckLibgit2.Check(res, "Unable to read ODB object header");
    return ((nuint)size, type.ToManaged());
  }

  public IGitOdbObject Read(GitOid oid)
  {
    using var nativeOid = GitOidMapper.ToNative(oid);
    var res = libgit2.odb.GitOdbRead(out var nativeGitOdbObject, NativeGitOdb, nativeOid);
    CheckLibgit2.Check(res, "Unable to read ODB object");
    return new GitOdbObject(nativeGitOdbObject);
  }

  public IGitOdbObject ReadPrefix(byte[] shortId, nuint length)
  {
    GitOid gitOid = GitOidMapper.FromShortId(shortId);
    using var nativeOid = GitOidMapper.ToNative(gitOid);
    var res = libgit2.odb.GitOdbReadPrefix(
      out var nativeGitOdbObject, NativeGitOdb, nativeOid, (UIntPtr)length);
    CheckLibgit2.Check(res, "Unable to read ODB object");
    return new GitOdbObject(nativeGitOdbObject);
  }

  public IGitOdbObject ReadPrefix(string shortSha)
  {
    UInt16 shortShaLength = (UInt16)shortSha.Length;
    if (shortSha.Length % 2 != 0)
    {
      shortSha += "0";
    }
    var shortId = Convert.FromHexString(shortSha);
    return ReadPrefix(shortId, shortShaLength);
  }

  public void Refresh()
  {
    var res = libgit2.odb.GitOdbRefresh(NativeGitOdb);
    CheckLibgit2.Check(res, "Unable to refresh ODB");
  }

  public IOdbStream OpenWriteStream(UIntPtr size, GitObjectType type)
  {
    var res = libgit2.odb.GitOdbOpenWstream(
      out var nativeGitOdbStream, NativeGitOdb, size, type.ToNative());
    CheckLibgit2.Check(res, "Unable to open ODB stream");
    return new OdbStream(nativeGitOdbStream);
  }

  public (IOdbStream stream, UIntPtr size, GitObjectType type) OpenReadStream(GitOid oid)
  {
    using var nativeOid = GitOidMapper.ToNative(oid);
    UInt64 size = 0;
    libgit2.GitObjectT type = 0;
    var res = libgit2.odb.GitOdbOpenRstream(
      out var nativeGitOdbStream, ref size, ref type, NativeGitOdb, nativeOid);
    CheckLibgit2.Check(res, "Unable to open ODB stream");
    return (new OdbStream(nativeGitOdbStream), (UIntPtr)size, type.ToManaged());
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.odb.GitOdbFree(NativeGitOdb);
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
