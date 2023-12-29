using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal class GitDiff : IGitDiff
{
  public libgit2.GitDiff NativeGitDiff { get; }
  private bool _ownsNativeInstance;
  public GitDiff(libgit2.GitDiff nativeGitDiff, bool ownsNativeInstance = true)
  {
    NativeGitDiff = nativeGitDiff;
    _ownsNativeInstance = ownsNativeInstance;
  }

  public GitDiffDelta? GetDelta(UInt64 index)
  {
    var nativeDelta = libgit2.diff.GitDiffGetDelta(NativeGitDiff, (UIntPtr)index);
    if (nativeDelta is null)
    {
      return null;
    }
    return GitDiffDeltaMapper.FromNative(nativeDelta);
  }

  public UInt64 GetNumDeltas()
  {
    return libgit2.diff.GitDiffNumDeltas(NativeGitDiff);
  }

  public UInt64 GetNumDeltasOfType(GitDeltaType type)
  {
    return libgit2.diff.GitDiffNumDeltasOfType(NativeGitDiff, type.ToNative());
  }

  public GitDiffStats GetStats()
  {
    var res = libgit2.diff.GitDiffGetStats(out var stats, NativeGitDiff);
    CheckLibgit2.Check(res, "Get stats failed");

    try
    {
      return new()
      {
        Deletions = libgit2.diff.GitDiffStatsDeletions(stats),
        FilesChanged = libgit2.diff.GitDiffStatsFilesChanged(stats),
        Insertions = libgit2.diff.GitDiffStatsInsertions(stats)
      };
    }
    finally
    {
      libgit2.diff.GitDiffStatsFree(stats);
    }
  }

  public string GetStatsFormatted(GitDiffStatsFormatOptions format, UInt32 width)
  {
    var res = libgit2.diff.GitDiffGetStats(out var stats, NativeGitDiff);
    CheckLibgit2.Check(res, "Get stats failed");
    
    try
    {
      res = libgit2.diff.GitDiffStatsToBuf(out var buf, stats, format.ToNative(), width);
      CheckLibgit2.Check(res, "Get stats formatted failed");
      using (buf)
      {
        return StringUtil.ToString(buf);
      }
    }
    finally
    {
      libgit2.diff.GitDiffStatsFree(stats);
    }
  }

  public bool IsSortedCaseInsensitively()
  {
    return libgit2.diff.GitDiffIsSortedIcase(NativeGitDiff) != 0;
  }

  public void FindSimilar(GitDiffFindOptions? options = null)
  {
    using DisposableCollection disposables = new();

    var nativeOptions = options?.ToNative(disposables);
    var res = libgit2.diff.GitDiffFindSimilar(NativeGitDiff, nativeOptions);

    CheckLibgit2.Check(res, "Find similar failed");
  }

  public void Merge(IGitDiff from)
  {
    var fromManaged = GittelObjects.DowncastNonNull<GitDiff>(from);
    var res = libgit2.diff.GitDiffMerge(NativeGitDiff, fromManaged.NativeGitDiff);
    CheckLibgit2.Check(res, "Merge failed");
  }

  public void ForEach(
    IGitDiff.FileCallback? fileCallback = null, 
    IGitDiff.BinaryCallback? binaryCallback = null,
    IGitDiff.HunkCallback? hunkCallback = null,
    IGitDiff.LineCallback? lineCallback = null)
  {
    using DisposableCollection disposables = new();

    using var callbacks = new GitDiffCallbacks(
      fileCallback, binaryCallback, hunkCallback, lineCallback);

    var res = libgit2.diff.GitDiffForeach(
      NativeGitDiff, 
      GitDiffCallbacks.GitDiffFileCb,
      GitDiffCallbacks.GitDiffBinaryCb,
      GitDiffCallbacks.GitDiffHunkCb,
      GitDiffCallbacks.GitDiffLineCb,
      callbacks.Payload);

    CheckLibgit2.Check(res, "ForEach failed");
  }

  public byte[] ToBuffer(GitDiffFormatOptions format)
  {
    var res = libgit2.diff.GitDiffToBuf(out var buf, NativeGitDiff, format.ToNative());
    CheckLibgit2.Check(res, "To buffer failed");
    using (buf)
    {
      return StringUtil.ToArray(buf);
    }
  }

  public void Print(GitDiffFormatOptions format, IGitDiff.LineCallback printCallback)
  {
    using var callbacks = new GitDiffCallbacks(lineCallback: printCallback);
    var res = libgit2.diff.GitDiffPrint(
      NativeGitDiff, format.ToNative(), GitDiffCallbacks.GitDiffLineCb, callbacks.Payload);
    CheckLibgit2.Check(res, "Print failed");
  }

  public GitOid PatchId(GitDiffPatchIdOptions? options = null)
  {
    var res = libgit2.diff.GitDiffPatchid(out var patchId, NativeGitDiff, null);
    CheckLibgit2.Check(res, "Patch ID failed");
    using (patchId)
    {
      return GitOidMapper.FromNative(patchId);
    }
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (_ownsNativeInstance)
      {
        libgit2.diff.GitDiffFree(NativeGitDiff);
      }
      _disposedValue = true;
    }
  }

  ~GitDiff()
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
