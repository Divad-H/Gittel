namespace Libgit2Bindings.Mappers;

internal static class GitRemoteTagDownloadOptionsMapper
{
  public static libgit2.GitRemoteAutotagOptionT ToNative(this GitRemoteTagDownloadOptions managedOptions)
  {
    return managedOptions switch
    {
      GitRemoteTagDownloadOptions.Unspecified => libgit2.GitRemoteAutotagOptionT.GIT_REMOTE_DOWNLOAD_TAGS_UNSPECIFIED,
      GitRemoteTagDownloadOptions.Auto => libgit2.GitRemoteAutotagOptionT.GIT_REMOTE_DOWNLOAD_TAGS_AUTO,
      GitRemoteTagDownloadOptions.None => libgit2.GitRemoteAutotagOptionT.GIT_REMOTE_DOWNLOAD_TAGS_NONE,
      GitRemoteTagDownloadOptions.All => libgit2.GitRemoteAutotagOptionT.GIT_REMOTE_DOWNLOAD_TAGS_ALL,
      _ => throw new ArgumentOutOfRangeException(nameof(managedOptions), managedOptions, null)
    };
  }
}
