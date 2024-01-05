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

  public string Path
  {
    get
    {
      return libgit2.index.GitIndexPath(NativeGitIndex);
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

  public GitIndexEntry GetEntry(UInt64 index)
  {
    using var nativeIndexEntry = libgit2.index.GitIndexGetByindex(NativeGitIndex, (UIntPtr)index);
    return nativeIndexEntry.ToManaged();
  }

  public GitIndexEntry GetEntryByPath(string path, int stage)
  {
    using var nativeIndexEntry = libgit2.index.GitIndexGetBypath(NativeGitIndex, path, stage);
    return nativeIndexEntry.ToManaged();
  }

  public UInt64 FindEntryIndex(string path)
  {
    UInt64 pos = 0;
    var res = libgit2.index.GitIndexFind(ref pos, NativeGitIndex, path);
    CheckLibgit2.Check(res, "Unable to find path '{0}' in index", path);
    return pos;
  }

  public UInt64 FindEntryIndexByPrefix(string pathPrefix)
  {
    UInt64 pos = 0;
    var res = libgit2.index.GitIndexFindPrefix(ref pos, NativeGitIndex, pathPrefix);
    CheckLibgit2.Check(res, "Unable to find path prefix '{0}' in index", pathPrefix);
    return pos;
  }

  public IEnumerable<GitIndexEntry> GetEntries()
  {
    var res = libgit2.index.GitIndexIteratorNew(out var iterator, NativeGitIndex);
    CheckLibgit2.Check(res, "Unable to create index iterator");
    try
    {
      while (true)
      {
        res = libgit2.index.GitIndexIteratorNext(out var indexEntry, iterator);
        if (res == (int)libgit2.GitErrorCode.GIT_ITEROVER)
        {
          break;
        }
        CheckLibgit2.Check(res, "Unable to get next index entry from iterator");
        using (indexEntry)
        {
          yield return indexEntry.ToManaged();
        }
      }
    }
    finally
    {
      libgit2.index.GitIndexIteratorFree(iterator);
    }
  }

  public void Add(GitIndexEntry entry)
  {
    using var nativeEntry = entry.ToNative();
    var res = libgit2.index.GitIndexAdd(NativeGitIndex, nativeEntry);
    CheckLibgit2.Check(res, "Unable to add entry to index");
  }

  public void UpdateAll(IReadOnlyCollection<string> pathspecs, 
    GitIndexMatchedPathCallback? callback = null)
  {
    using GitIndexMatchedPathCallbackImpl? callbackImpl = callback is null ? null : new(callback);
    using var nativePathspecs = new GitStrArrayImpl(pathspecs);
    var res = libgit2.index.GitIndexUpdateAll(
      NativeGitIndex, 
      nativePathspecs.NativeStrArray,
      callback is null ? null : GitIndexMatchedPathCallbackImpl.GitIndexMatchedPathCb,
      callbackImpl?.Payload ?? IntPtr.Zero);
    CheckLibgit2.Check(res, "Unable to update all paths in index");
  }

  public void Remove(string path, int stage)
  {
    var res = libgit2.index.GitIndexRemove(NativeGitIndex, path, stage);
    CheckLibgit2.Check(res, "Unable to remove entry from index");
  }

  public void RemoveByPath(string path)
  {
    var res = libgit2.index.GitIndexRemoveBypath(NativeGitIndex, path);
    CheckLibgit2.Check(res, "Unable to remove entry from index");
  }

  public void RemoveDirectory(string directory, int stage)
  {
    var res = libgit2.index.GitIndexRemoveDirectory(NativeGitIndex, directory, stage);
    CheckLibgit2.Check(res, "Unable to remove directory from index");
  }

  public void RemoveAll(IReadOnlyCollection<string> pathspecs,
    GitIndexMatchedPathCallback? callback = null)
  {
    using GitIndexMatchedPathCallbackImpl? callbackImpl = callback is null ? null : new(callback);
    using var nativePathspecs = new GitStrArrayImpl(pathspecs);
    var res = libgit2.index.GitIndexRemoveAll(
      NativeGitIndex, 
      nativePathspecs.NativeStrArray,
      callback is null ? null : GitIndexMatchedPathCallbackImpl.GitIndexMatchedPathCb,
      callbackImpl?.Payload ?? IntPtr.Zero);
    CheckLibgit2.Check(res, "Unable to remove all paths from index");
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

  public void CleanupConflicts()
  {
    var res = libgit2.index.GitIndexConflictCleanup(NativeGitIndex);
    CheckLibgit2.Check(res, "Unable to cleanup conflicts in index");
  }

  public void RemoveConflict(string path)
  {
    var res = libgit2.index.GitIndexConflictRemove(NativeGitIndex, path);
    CheckLibgit2.Check(res, "Unable to remove conflict from index");
  }

  public ConflictEntries GetConflict(string path)
  {
    var res = libgit2.index.GitIndexConflictGet(
      out var ancestorEntry, out var ourEntry, out var theirEntry, NativeGitIndex, path);
    CheckLibgit2.Check(res, "Unable to get conflict from index");
    using (ancestorEntry)
    using (ourEntry)
    using (theirEntry)
    {
      return new ConflictEntries()
      {
        Ancestor = ancestorEntry?.ToManaged(),
        Our = ourEntry?.ToManaged(),
        Their = theirEntry?.ToManaged()
      };
    }
  }

  public IEnumerable<ConflictEntries> GetAllConflicts()
  {
    var res = libgit2.index.GitIndexConflictIteratorNew(out var iterator, NativeGitIndex);
    CheckLibgit2.Check(res, "Unable to create conflict iterator");
    try
    {
      while (true)
      {
        res = libgit2.index.GitIndexConflictNext(
          out var ancestorEntry, out var ourEntry, out var theirEntry, iterator);
        if (res == (int)libgit2.GitErrorCode.GIT_ITEROVER)
        {
          break;
        }
        CheckLibgit2.Check(res, "Unable to get next conflict from iterator");
        using (ancestorEntry)
        using (ourEntry)
        using (theirEntry)
        {
          yield return new ConflictEntries()
          {
            Ancestor = ancestorEntry?.ToManaged(),
            Our = ourEntry?.ToManaged(),
            Their = theirEntry?.ToManaged()
          };
        }
      }
    }
    finally
    {
      libgit2.index.GitIndexConflictIteratorFree(iterator);
    }
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

  public void Read(bool force)
  {
    var res = libgit2.index.GitIndexRead(NativeGitIndex, force ? 1 : 0);
    CheckLibgit2.Check(res, "Unable to read index");
  }

  public void ReadTree(IGitTree tree)
  {
    var managedTree = GittelObjects.DowncastNonNull<GitTree>(tree);
    var res = libgit2.index.GitIndexReadTree(NativeGitIndex, managedTree.NativeGitTree);
    CheckLibgit2.Check(res, "Unable to read tree into index");
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
