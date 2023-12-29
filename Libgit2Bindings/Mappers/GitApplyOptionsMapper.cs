using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class GitApplyOptionsMapper
{
  public static libgit2.GitApplyOptions ToNative(this GitApplyOptions options, 
    DisposableCollection disposables)
  {
    var nativeOptions = new libgit2.GitApplyOptions();

    try
    {
      libgit2.apply.GitApplyOptionsInit(nativeOptions,
               (UInt32)libgit2.GitApplyOptionsVersion.GIT_APPLY_OPTIONS_VERSION);

      nativeOptions.Flags = (UInt32)options.Flags;
      
      if (options.DeltaCallback is not null || options.HunkCallback is not null)
      {
        var callbacksImpl = new ApplyCallbacksImpl(options.DeltaCallback, options.HunkCallback)
          .DisposeWith(disposables);

        if (options.DeltaCallback is not null)
        {
          nativeOptions.DeltaCb = ApplyCallbacksImpl.GitApplyDeltaCb;
        }
        if (options.HunkCallback is not null)
        {
          nativeOptions.HunkCb = ApplyCallbacksImpl.GitApplyHunkCb;
        }
        nativeOptions.Payload = callbacksImpl.Payload;
      }
    }
    catch (Exception)
    {
      nativeOptions.Dispose();
      throw;
    }

    return nativeOptions;
  }
}
