using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class CheckoutOptionsMapper
{
  public static libgit2.GitCheckoutOptions ToNative(this CheckoutOptions? managedOptions,
    DisposableCollection disposables)
  {
    var nativeOptions = new libgit2.GitCheckoutOptions();
    try
    {
      libgit2.checkout.GitCheckoutOptionsInit(nativeOptions, 
               (uint)libgit2.GitCheckoutOptionsVersion.GIT_CHECKOUT_OPTIONS_VERSION);

      if (managedOptions is null)
      {
        return nativeOptions;
      }

      var callbacks = managedOptions.NotifyCallback is not null 
        || managedOptions.ProgressCallback is not null
        || managedOptions.PerformanceDataCallback is not null
        ? new CheckoutCallbacksImpl(
            managedOptions.NotifyCallback, 
            managedOptions.ProgressCallback,
            managedOptions.PerformanceDataCallback
          ).DisposeWith(disposables)
        : null;

      nativeOptions.CheckoutStrategy = (uint)CheckoutStrategyMapper.ToNative(managedOptions.Strategy);
      nativeOptions.DisableFilters = managedOptions.DisableFilters ? 1 : 0;
      nativeOptions.NotifyFlags = (uint)CheckoutNotifyFlagsMapper.ToNative(managedOptions.NotifyFlags);

      if (callbacks is not null)
      {
        if (managedOptions.NotifyCallback is not null)
        {
          nativeOptions.NotifyCb = CheckoutCallbacksImpl.GitCheckoutNotifyCb;
          nativeOptions.NotifyPayload = callbacks.Payload;
        }
        if (managedOptions.ProgressCallback is not null)
        {
          nativeOptions.ProgressCb = CheckoutCallbacksImpl.GitCheckoutProgressCb;
          nativeOptions.ProgressPayload = callbacks.Payload;
        }
        if (managedOptions.PerformanceDataCallback is not null)
        {
          nativeOptions.PerfdataCb = CheckoutCallbacksImpl.GitPerformanceDataCb;
          nativeOptions.PerfdataPayload = callbacks.Payload;
        }
      }

      if (managedOptions.Paths is not null)
      {
        var gitStrArray = new GitStrArrayImpl(managedOptions.Paths).DisposeWith(disposables);
        nativeOptions.Paths = gitStrArray.NativeStrArray;
      }

      nativeOptions.Baseline = (managedOptions.Baseline as GitTree)?.NativeGitTree;
      nativeOptions.BaselineIndex = (managedOptions.BaselineIndex as GitIndex)?.NativeGitIndex;
      nativeOptions.TargetDirectory = managedOptions.TargetDirectory;
      nativeOptions.AncestorLabel = managedOptions.AncestorLabel;
      nativeOptions.OurLabel = managedOptions.OurLabel;
      nativeOptions.TheirLabel = managedOptions.TheirLabel;

      return nativeOptions;
    }
    catch (Exception)
    {
      nativeOptions.Dispose();
      throw;
    }
  }
}
