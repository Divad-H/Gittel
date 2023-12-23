using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;
using System.Runtime.InteropServices;

namespace Libgit2Bindings.Callbacks;

internal class GitDiffCallbacks : IDisposable
{
  private readonly IGitDiff.FileCallback? _fileCallback;
  private readonly IGitDiff.BinaryCallback? _binaryCallback;
  private readonly IGitDiff.HunkCallback? _hunkCallback;
  private readonly IGitDiff.LineCallback? _lineCallback;

  private readonly GCHandle _gcHandle;

  public GitDiffCallbacks(
    IGitDiff.FileCallback? fileCallback,
    IGitDiff.BinaryCallback? binaryCallback,
    IGitDiff.HunkCallback? hunkCallback,
    IGitDiff.LineCallback? lineCallback)
  {
    _fileCallback = fileCallback;
    _binaryCallback = binaryCallback;
    _hunkCallback = hunkCallback;
    _lineCallback = lineCallback;

    _gcHandle = GCHandle.Alloc(this);
  }

  public IntPtr Payload => GCHandle.ToIntPtr(_gcHandle);

  public static int GitDiffFileCb(IntPtr delta, float progress, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (GitDiffCallbacks)gcHandle.Target!;

      using var gitDiffDelta = libgit2.GitDiffDelta.__CreateInstance(delta);
      var deltaToAdd = GitDiffDeltaMapper.FromNative(gitDiffDelta);

      return callbacks._fileCallback?.Invoke(deltaToAdd, progress) ?? GitOperationContinuation.Continue;
    };

    return func.ExecuteInTryCatch(nameof(GitDiffFileCb));
  }

  public static int GitDiffBinaryCb(IntPtr delta, IntPtr binary, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (GitDiffCallbacks)gcHandle.Target!;

      using var gitDiffDelta = libgit2.GitDiffDelta.__CreateInstance(delta);
      var deltaToAdd = GitDiffDeltaMapper.FromNative(gitDiffDelta);

      using var gitDiffBinary = libgit2.GitDiffBinary.__CreateInstance(binary);
      var binaryToAdd = GitDiffBinaryMapper.FromNative(gitDiffBinary);

      return callbacks._binaryCallback?.Invoke(deltaToAdd, binaryToAdd) ?? GitOperationContinuation.Continue;
    };

    return func.ExecuteInTryCatch(nameof(GitDiffBinaryCb));
  }

  public static int GitDiffHunkCb(IntPtr delta, IntPtr hunk, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (GitDiffCallbacks)gcHandle.Target!;

      using var gitDiffDelta = libgit2.GitDiffDelta.__CreateInstance(delta);
      var deltaToAdd = GitDiffDeltaMapper.FromNative(gitDiffDelta);

      using var gitDiffHunk = libgit2.GitDiffHunk.__CreateInstance(hunk);
      var hunkToAdd = GitDiffHunkMapper.FromNative(gitDiffHunk);

      return callbacks._hunkCallback?.Invoke(deltaToAdd, hunkToAdd) ?? GitOperationContinuation.Continue;
    };

    return func.ExecuteInTryCatch(nameof(GitDiffHunkCb));
  }

  public static int GitDiffLineCb(IntPtr delta, IntPtr hunk, IntPtr line, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (GitDiffCallbacks)gcHandle.Target!;

      using var gitDiffDelta = libgit2.GitDiffDelta.__CreateInstance(delta);
      var deltaToAdd = GitDiffDeltaMapper.FromNative(gitDiffDelta);

      using var gitDiffHunk = libgit2.GitDiffHunk.__CreateInstance(hunk);
      var hunkToAdd = GitDiffHunkMapper.FromNative(gitDiffHunk);

      using var gitDiffLine = libgit2.GitDiffLine.__CreateInstance(line);
      var lineToAdd = GitDiffLineMapper.FromNative(gitDiffLine);

      return callbacks._lineCallback?.Invoke(deltaToAdd, hunkToAdd, lineToAdd) ?? GitOperationContinuation.Continue;
    };

    return func.ExecuteInTryCatch(nameof(GitDiffLineCb));
  }
  public void Dispose()
  {
    _gcHandle.Free();
  }
}
