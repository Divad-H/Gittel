using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class CloneOptionsMapper
{
  public static libgit2.GitCloneOptions ToNative(this CloneOptions managedOptions,
    DisposableCollection disposables)
  {
    var nativeOptions = new libgit2.GitCloneOptions();
    try
    {
      libgit2.clone.GitCloneOptionsInit(nativeOptions, 
        (uint)libgit2.GitCloneOptionsVersion.GIT_CLONE_OPTIONS_VERSION);

      nativeOptions.CheckoutOpts = managedOptions.CheckoutOptions.ToNative(disposables).DisposeWith(disposables);
      nativeOptions.FetchOpts = managedOptions.FetchOptions.ToNative(disposables).DisposeWith(disposables);
      nativeOptions.Bare = managedOptions.Bare ? 1 : 0;
      nativeOptions.Local = managedOptions.CloneLocal.ToNative();
      nativeOptions.CheckoutBranch = managedOptions.CheckoutBranch;
      
      if (managedOptions.RepositoryCreateCallback is not null || managedOptions.RemoteCreateCallback is not null)
      {
        var callbacksImpl = new CloneCallbacksImpl(
          managedOptions.RepositoryCreateCallback, 
          managedOptions.RemoteCreateCallback)
          .DisposeWith(disposables);

        if (managedOptions.RepositoryCreateCallback is not null)
        {
          nativeOptions.RepositoryCb = CloneCallbacksImpl.GitRepositoryCreateCb;
          nativeOptions.RepositoryCbPayload = callbacksImpl.Payload;
        }
        if (managedOptions.RemoteCreateCallback is not null)
        {
          nativeOptions.RemoteCb = CloneCallbacksImpl.GitRemoteCreateCb;
          nativeOptions.RemoteCbPayload = callbacksImpl.Payload;
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
