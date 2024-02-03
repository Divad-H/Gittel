using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class GitRebaseOptionsMapper
{
  public static libgit2.GitRebaseOptions ToNative(this GitRebaseOptions options,
    DisposableCollection disposables)
  {
    var nativeOptions = new libgit2.GitRebaseOptions();

    nativeOptions.Version = (int)libgit2.GitRebaseOptionsVersion.GIT_REBASE_OPTIONS_VERSION;
    nativeOptions.Quiet = options.Quiet ? 1 : 0;
    nativeOptions.Inmemory = options.InMemory ? 1 : 0;
    nativeOptions.RewriteNotesRef = options.RewriteNotesRef;
    nativeOptions.MergeOptions = (options.MergeOptions ?? new()).ToNative(disposables)
      .DisposeWith(disposables);
    nativeOptions.CheckoutOptions = (options.CheckoutOptions ?? new()).ToNative(disposables)
      .DisposeWith(disposables);

    if (options.CommitCreateCallback is not null)
    {
      var commitCreateCallbackImpl = new GitRebaseCallbackImpl(options.CommitCreateCallback)
        .DisposeWith(disposables);
      unsafe
      {
        delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, nuint, IntPtr, IntPtr, int> fPtr 
          = (&GitRebaseCallbackImpl.GitRebaseCommitCreateCb);
        ((libgit2.GitRebaseOptions.__Internal*)nativeOptions.__Instance)->commit_create_cb = (IntPtr)fPtr;
      }
      nativeOptions.Payload = commitCreateCallbackImpl.Payload;
    }

    return nativeOptions;
  }
}
