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

      using var callbacks = managedOptions.NotifyCallback is not null || managedOptions.ProgressCallback is not null
        ? new CheckoutCallbacksImpl(managedOptions.NotifyCallback, managedOptions.ProgressCallback).DisposeWith(disposables)
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
      }

      return nativeOptions;
    }
    catch (Exception)
    {
      nativeOptions.Dispose();
      throw;
    }
  }
}
