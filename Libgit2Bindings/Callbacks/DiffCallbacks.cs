using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;
using System.Runtime.InteropServices;

namespace Libgit2Bindings.Callbacks;

internal sealed class DiffCallbacks : IDisposable
{
  private readonly GCHandle _gcHandle;

  private readonly DiffNotifyHandler? _notify;
  private readonly DiffProgressHandler? _progress;

  public DiffCallbacks(
    DiffNotifyHandler? notify,
    DiffProgressHandler? progress)
  {
    _notify = notify;
    _progress = progress;

    _gcHandle = GCHandle.Alloc(this);
  }

  public IntPtr Payload => GCHandle.ToIntPtr(_gcHandle);

  public static int GitDiffNotifyCb(IntPtr diff_so_far, IntPtr delta_to_add, string? matched_pathspec, IntPtr payload)
  {
    Func<int> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (DiffCallbacks)gcHandle.Target!;

      using var diffSoFar = new GitDiff(libgit2.GitDiff.__CreateInstance(diff_so_far), false);

      using var gitDiffDelta = libgit2.GitDiffDelta.__CreateInstance(delta_to_add);
      var deltaToAdd = GitDiffDeltaMapper.FromNative(gitDiffDelta);

      return callbacks._notify?.Invoke(diffSoFar, deltaToAdd, matched_pathspec) ?? (int)libgit2.GitErrorCode.GIT_EUSER;
    };

    return func.ExecuteInTryCatch(nameof(GitDiffNotifyCb));
  }

  public static int GitDiffProgressCb(IntPtr diff_so_far, string old_path, string new_path, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (DiffCallbacks)gcHandle.Target!;

      using var diffSoFar = new GitDiff(libgit2.GitDiff.__CreateInstance(diff_so_far), false);

      return callbacks._progress?.Invoke(diffSoFar, old_path, new_path) ?? GitOperationContinuation.Continue;
    };

    return func.ExecuteInTryCatch(nameof(GitDiffProgressCb));
  }

  public void Dispose()
  {
    _gcHandle.Free();
  }
}
