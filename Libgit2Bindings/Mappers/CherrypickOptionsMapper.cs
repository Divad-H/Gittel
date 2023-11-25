using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class CherrypickOptionsMapper
{
  public static libgit2.GitCherrypickOptions ToNative(
    this CherrypickOptions managedOptions,
    DisposableCollection disposables)
  {
    var nativeOptions = new libgit2.GitCherrypickOptions();
    try
    {
      libgit2.cherrypick.GitCherrypickOptionsInit(nativeOptions,
        (uint)libgit2.GitCherrypickOptionsVersion.GIT_CHERRYPICK_OPTIONS_VERSION);

      nativeOptions.Mainline = managedOptions.Mainline;
      nativeOptions.MergeOpts = managedOptions.MergeOptions.ToNative(disposables);
      nativeOptions.CheckoutOpts = managedOptions.CheckoutOptions.ToNative(disposables);

      return nativeOptions;
    }
    catch (Exception)
    {
      nativeOptions.Dispose();
      throw;
    }
  }
}
