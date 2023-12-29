using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;
using System.Runtime.InteropServices;

namespace Libgit2Bindings.Callbacks;

internal sealed class ApplyCallbacksImpl : IDisposable
{
  private readonly GCHandle _gcHandle;

  private readonly ApplyDeltaCallback? _delta;
  private readonly ApplyHunkCallback? _hunk;

  public ApplyCallbacksImpl(
    ApplyDeltaCallback? delta,
    ApplyHunkCallback? hunk)
  {
    _delta = delta;
    _hunk = hunk;

    _gcHandle = GCHandle.Alloc(this);
  }

  public IntPtr Payload => GCHandle.ToIntPtr(_gcHandle);

  public static int GitApplyDeltaCb(IntPtr delta, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (ApplyCallbacksImpl)gcHandle.Target!;
      var deltaData = GitDiffDeltaMapper.FromNativePtr(delta);
      var res = callbacks._delta?.Invoke(deltaData);
      return res ?? GitOperationContinuation.Continue;
    };
    
    return func.ExecuteInTryCatch(nameof(GitApplyDeltaCb));
  }

  public static int GitApplyHunkCb(IntPtr hunk, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (ApplyCallbacksImpl)gcHandle.Target!;
      var hunkData = GitDiffHunkMapper.FromNativePtr(hunk);
      var res = callbacks._hunk?.Invoke(hunkData);
      return res ?? GitOperationContinuation.Continue;
    };
    
    return func.ExecuteInTryCatch(nameof(GitApplyHunkCb));
  }

  public void Dispose()
  {
    _gcHandle.Free();
  }
}
