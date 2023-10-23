using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class FetchOptionsMapper
{
  public static libgit2.GitFetchOptions ToNative(this FetchOptions managedOptions,
    DisposableCollection disposables)
  {
    var nativeOptions = new libgit2.GitFetchOptions();
    try
    {
      libgit2.remote.GitFetchOptionsInit(nativeOptions, 
               (uint)libgit2.GitFetchOptionsVersion.GIT_FETCH_OPTIONS_VERSION);

      nativeOptions.Callbacks = managedOptions.Callbacks.ToNative(disposables).DisposeWith(disposables);
      nativeOptions.Prune = managedOptions.Prune.ToNative();
      nativeOptions.UpdateFetchhead = managedOptions.UpdateFetchHead ? 1 : 0;
      nativeOptions.DownloadTags = managedOptions.DownloadTags.ToNative();
      if (managedOptions.ProxyOptions is not null)
      {
        nativeOptions.ProxyOpts = managedOptions.ProxyOptions.ToNative(disposables).DisposeWith(disposables);
      }
      nativeOptions.Depth = managedOptions.Depth;
      nativeOptions.FollowRedirects = managedOptions.FollowRedirects.ToNative();

      if (managedOptions.CustomHeaders is not null)
      {
        var customHeadersWrapper = new GitStrArrayImpl(managedOptions.CustomHeaders)
          .DisposeWith(disposables);
        nativeOptions.CustomHeaders = customHeadersWrapper.NativeStrArray;
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
