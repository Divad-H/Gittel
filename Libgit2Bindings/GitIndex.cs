using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal sealed class GitIndex(libgit2.GitIndex nativeGitIndex) : IGitIndex
{
  public libgit2.GitIndex NativeGitIndex { get; } = nativeGitIndex;

  public ulong EntryCount
  {
    get
    {
      return libgit2.index.GitIndexEntrycount(NativeGitIndex);
    }
  }

  public GitIndexCapability Capabilities
  {
    get
    {
      return (GitIndexCapability)libgit2.index.GitIndexCaps(NativeGitIndex);
    }
  }

  public uint Version
  {
    get
    {
      return libgit2.index.GitIndexVersion(NativeGitIndex);
    }
  }

  public void SetCapabilities(GitIndexCapability capabilities)
  {
    var res = libgit2.index.GitIndexSetCaps(NativeGitIndex, (int)capabilities);
    CheckLibgit2.Check(res, "Unable to set index capabilities");
  }

  public void SetVersion(uint version)
  {
    var res = libgit2.index.GitIndexSetVersion(NativeGitIndex, version);
    CheckLibgit2.Check(res, "Unable to set index version");
  }

  public bool HasConflicts()
  {
    return libgit2.index.GitIndexHasConflicts(NativeGitIndex) == 1;
  }


  public void AddByPath(string path)
  {
    var res = libgit2.index.GitIndexAddBypath(NativeGitIndex, path);
    CheckLibgit2.Check(res, "Unable to add path '{0}' to index", path);
  }

  public void AddAll(IReadOnlyCollection<string> pathspecs, GitIndexAddOption flags, 
    GitIndexMatchedPathCallback? callback = null)
  {
    using GitIndexMatchedPathCallbackImpl? callbackImpl = callback is null ? null : new(callback);
    using var nativePathspecs = new GitStrArrayImpl(pathspecs);
    var res = libgit2.index.GitIndexAddAll(
      NativeGitIndex, nativePathspecs.NativeStrArray, (uint)flags,
      callback is null ? null : GitIndexMatchedPathCallbackImpl.GitIndexMatchedPathCb, 
      callbackImpl?.Payload ?? IntPtr.Zero);
    CheckLibgit2.Check(res, "Unable to add all paths to index");
  }

  public void Clear()
  {
    var res = libgit2.index.GitIndexClear(NativeGitIndex);
    CheckLibgit2.Check(res, "Unable to clear index");
  }

  public GitIndexEntry GetEntry(ulong index)
  {
    using var nativeIndexEntry = libgit2.index.GitIndexGetByindex(NativeGitIndex, (UIntPtr)index);
    return nativeIndexEntry.ToManaged();
  }

  public void Add(GitIndexEntry entry)
  {
    using var nativeEntry = entry.ToNative();
    var res = libgit2.index.GitIndexAdd(NativeGitIndex, nativeEntry);
    CheckLibgit2.Check(res, "Unable to add entry to index");
  }

  public void AddFromBuffer(GitIndexEntry entry, byte[] buffer)
  {
    using var nativeEntry = entry.ToNative();
    using var pinnedBuffer = new PinnedBuffer(buffer);
    var res = libgit2.index.GitIndexAddFromBuffer(
      NativeGitIndex, nativeEntry, pinnedBuffer.Pointer, (UIntPtr)pinnedBuffer.Length);
    CheckLibgit2.Check(res, "Unable to add entry from buffer to index");
  }

  public void AddConflict(
    GitIndexEntry? ancestorEntry, GitIndexEntry? ourEntry, GitIndexEntry? theirEntry)
  {
    using var nativeAncestorEntry = ancestorEntry?.ToNative();
    using var nativeOurEntry = ourEntry?.ToNative();
    using var nativeTheirEntry = theirEntry?.ToNative();
    var res = libgit2.index.GitIndexConflictAdd(
      NativeGitIndex, nativeAncestorEntry, nativeOurEntry, nativeTheirEntry);
    CheckLibgit2.Check(res, "Unable to add conflict to index");
  }

  public GitOid GetChecksum()
  {
    using var checksum = libgit2.index.GitIndexChecksum(NativeGitIndex);
    return GitOidMapper.FromNative(checksum);
  }

  public GitOid WriteTree()
  {
    var res = libgit2.index.GitIndexWriteTree(out var treeOid, NativeGitIndex);
    using (treeOid)
    {
      CheckLibgit2.Check(res, "Unable to write tree");
      return GitOidMapper.FromNative(treeOid);
    }
  }

  public GitOid WriteTreeTo(IGitRepository repository)
  {
    var managedRepository = GittelObjects.DowncastNonNull<GitRepository>(repository);
    var res = libgit2.index.GitIndexWriteTreeTo(
      out var treeOid, NativeGitIndex, managedRepository.NativeGitRepository);
    using (treeOid)
    {
      CheckLibgit2.Check(res, "Unable to write tree");
      return GitOidMapper.FromNative(treeOid);
    }
  }

  public void Write()
  {
    var res = libgit2.index.GitIndexWrite(NativeGitIndex);
    CheckLibgit2.Check(res, "Unable to write index");
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.index.GitIndexFree(NativeGitIndex);
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
