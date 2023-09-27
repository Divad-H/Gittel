using System.Runtime.InteropServices;

namespace Libgit2Bindings.Callbacks;

internal sealed class CheckoutCallbacks : IDisposable
{
  private readonly GCHandle _gcHandle;

  private readonly CheckoutNotifyHandler? _notify;
  private readonly CheckoutProgressHandler? _progress;

  public CheckoutCallbacks(CheckoutNotifyHandler? notify, CheckoutProgressHandler? progress)
  {
    _notify = notify;
    _progress = progress;

    _gcHandle = GCHandle.Alloc(this);
  }

  public IntPtr Payload => GCHandle.ToIntPtr(_gcHandle);

  public static int GitCheckoutNotifyCb(global::libgit2.GitCheckoutNotifyT why, string path, IntPtr baseline, IntPtr target, IntPtr workdir, IntPtr payload)
  {
    GCHandle gcHandle = GCHandle.FromIntPtr(payload);
    var callbacks = (CheckoutCallbacks)gcHandle.Target!;
    var managedWhy = Mappers.CheckoutNotifyFlagsMapper.FromNative(why);
    var baselineDiffFile = Mappers.DiffFileMapper.FromNativePtr(baseline);
    var targetDiffFile = Mappers.DiffFileMapper.FromNativePtr(target);
    var workdirDiffFile = Mappers.DiffFileMapper.FromNativePtr(workdir);
    var res = callbacks._notify?.Invoke(managedWhy, path, baselineDiffFile, targetDiffFile, workdirDiffFile);
    if (res.HasValue)
      return res.Value == CheckoutNotifyAction.Continue ? 0 : -1;
    return 0;
  }

  public static void GitCheckoutProgressCb(string path, ulong completed_steps, ulong total_steps, IntPtr payload)
  {
    GCHandle gcHandle = GCHandle.FromIntPtr(payload);
    var callbacks = (CheckoutCallbacks)gcHandle.Target!;
    callbacks._progress?.Invoke(path, completed_steps, total_steps);
  }

  public void Dispose()
  {
    _gcHandle.Free();
  }
}

