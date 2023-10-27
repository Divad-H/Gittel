using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;
using System.Runtime.InteropServices;

namespace Libgit2Bindings.Callbacks;

internal sealed class CheckoutCallbacksImpl : IDisposable
{
  private readonly GCHandle _gcHandle;

  private readonly CheckoutNotifyHandler? _notify;
  private readonly CheckoutProgressHandler? _progress;
  private readonly PerformanceDataHandler? _performanceData;

  public CheckoutCallbacksImpl(
    CheckoutNotifyHandler? notify, 
    CheckoutProgressHandler? progress,
    PerformanceDataHandler? performanceData)
  {
    _notify = notify;
    _progress = progress;
    _performanceData = performanceData;

    _gcHandle = GCHandle.Alloc(this);
  }

  public IntPtr Payload => GCHandle.ToIntPtr(_gcHandle);

  public static int GitCheckoutNotifyCb(global::libgit2.GitCheckoutNotifyT why, string path, IntPtr baseline, IntPtr target, IntPtr workdir, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (CheckoutCallbacksImpl)gcHandle.Target!;
      var managedWhy = Mappers.CheckoutNotifyFlagsMapper.FromNative(why);
      var baselineDiffFile = Mappers.DiffFileMapper.FromNativePtr(baseline);
      var targetDiffFile = Mappers.DiffFileMapper.FromNativePtr(target);
      var workdirDiffFile = Mappers.DiffFileMapper.FromNativePtr(workdir);
      var res = callbacks._notify?.Invoke(managedWhy, path, baselineDiffFile, targetDiffFile, workdirDiffFile);
      return res ?? GitOperationContinuation.Continue;
    };
    
    return func.ExecuteInTryCatch(nameof(GitCheckoutNotifyCb));
  }

  public static void GitCheckoutProgressCb(string path, ulong completed_steps, ulong total_steps, IntPtr payload)
  {
    try
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (CheckoutCallbacksImpl)gcHandle.Target!;
      callbacks._progress?.Invoke(path, completed_steps, total_steps);
    }
    catch (Exception)
    {
      // ignored
    }
  }

  public static void GitPerformanceDataCb(IntPtr perfdata, IntPtr payload)
  {
    try
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (CheckoutCallbacksImpl)gcHandle.Target!;
      var managedPerfdata = PerfromanceDataMapper.FromNativePtr(perfdata);
      callbacks._performanceData?.Invoke(managedPerfdata);
    }
    catch (Exception)
    {
      // ignored
    }
  }

  public void Dispose()
  {
    _gcHandle.Free();
  }
}

