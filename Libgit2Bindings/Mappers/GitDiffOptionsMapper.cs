using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class GitDiffOptionsMapper
{
  public static libgit2.GitDiffOptions ToNative(this GitDiffOptions managedOptions, 
    DisposableCollection disposables)
  {
    var pathspec = new GitStrArrayImpl(managedOptions.Pathspec ?? Array.Empty<string>()).DisposeWith(disposables);
    
    var callbacks = (managedOptions.Notify is not null || managedOptions.Progress is not null)
      ? new DiffCallbacks(managedOptions.Notify, managedOptions.Progress).DisposeWith(disposables)
      : null;

    return new libgit2.GitDiffOptions()
    {
      Version = (UInt32)libgit2.GitDiffOptionsVersion.GIT_DIFF_OPTIONS_VERSION,
      Flags = (UInt32)GitDiffOptionFlagsMapper.ToNative(managedOptions.Flags),
      IgnoreSubmodules = IgnoreSubmodulesModeMapper.ToNative(managedOptions.IgnoreSubmodules),
      Pathspec = pathspec?.NativeStrArray,
      NotifyCb = managedOptions.Notify is null ? null : DiffCallbacks.GitDiffNotifyCb,
      ProgressCb = managedOptions.Progress is null ? null : DiffCallbacks.GitDiffProgressCb,
      Payload = callbacks?.Payload ?? IntPtr.Zero,
      ContextLines = managedOptions.ContextLines,
      InterhunkLines = managedOptions.InterhunkLines,
      OidType = GitOidTypeMapper.ToNative(managedOptions.OidType),
      IdAbbrev = managedOptions.IdAbbrev,
      MaxSize = managedOptions.MaxSize,
      OldPrefix = managedOptions.OldPrefix,
      NewPrefix = managedOptions.NewPrefix,
    };
  }
}
