using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;
using static Libgit2Bindings.IGitDiff;

namespace Libgit2Bindings;

internal sealed class GitPatch : IGitPatch
{
  public GitPatch(libgit2.GitPatch nativeGitPatch)
  {
    NativeGitPatch = nativeGitPatch;
  }

  public libgit2.GitPatch NativeGitPatch { get; private set; }

  public GitDiffDelta Delta 
    => GitDiffDeltaMapper.FromNative(libgit2.patch.GitPatchGetDelta(NativeGitPatch));

  public UIntPtr NumHunks => (UIntPtr)libgit2.patch.GitPatchNumHunks(NativeGitPatch);

  public byte[] GetContent()
  {
    var res = libgit2.patch.GitPatchToBuf(out var buf, NativeGitPatch);
    CheckLibgit2.Check(res, "Unable to get patch content");
    using (buf)
    {
      return StringUtil.ToArray(buf);
    }
  }

  public UIntPtr RawSize(bool includeContext, bool includeHunkHeaders, bool includeFileHeaders)
  {
    var res = libgit2.patch.GitPatchSize(
      NativeGitPatch, includeContext ? 1 : 0, includeHunkHeaders ? 1 : 0, includeFileHeaders ? 1 : 0);
    return (UIntPtr)res;
  }

  public int GetNumLinesInHunk(UIntPtr hunkIdx)
  {
    var res = libgit2.patch.GitPatchNumLinesInHunk(NativeGitPatch, hunkIdx);
    if (res < 0)
    {
      CheckLibgit2.Check(res, "Unable to get number of lines in hunk");
    }
    return res;
  }

  public (nuint contextLines, nuint addedLines, nuint deletedLines) GetLineStats()
  {
    UInt64 contextLines = 0;
    UInt64 addedLines = 0;
    UInt64 deletedLines = 0;
    var res = libgit2.patch.GitPatchLineStats(ref contextLines, ref addedLines, ref deletedLines, NativeGitPatch);
    CheckLibgit2.Check(res, "Unable to get line stats");
    return ((UIntPtr)contextLines, (UIntPtr)addedLines, (UIntPtr)deletedLines);
  }

  public GitDiffLine GetLineInHunk(UIntPtr hunkIdx, UIntPtr lineOfHunk)
  {
    var res = libgit2.patch.GitPatchGetLineInHunk(out var diffLine, NativeGitPatch, hunkIdx, lineOfHunk);
    CheckLibgit2.Check(res, "Unable to get line in hunk");
    using (diffLine)
    {
      return GitDiffLineMapper.FromNative(diffLine);
    }
  }

  public GitDiffHunk GetHunk(UIntPtr hunkIdx, out UIntPtr linesInHunk)
  {
    UInt64 lines = 0;
    var res = libgit2.patch.GitPatchGetHunk(out var diffHunk, ref lines, NativeGitPatch, hunkIdx);
    CheckLibgit2.Check(res, "Unable to get hunk");
    using (diffHunk)
    {
      linesInHunk = (UIntPtr)lines;
      return GitDiffHunkMapper.FromNative(diffHunk);
    }
  }

  public void Print(LineCallback printCallback)
  {
    using var callbacks = new GitDiffCallbacks(lineCallback: printCallback);

    var res = libgit2.patch.GitPatchPrint(NativeGitPatch, GitDiffCallbacks.GitDiffLineCb, callbacks.Payload);
    CheckLibgit2.Check(res, "Unable to print patch");
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.patch.GitPatchFree(NativeGitPatch);
      _disposedValue = true;
    }
  }

  ~GitPatch()
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
