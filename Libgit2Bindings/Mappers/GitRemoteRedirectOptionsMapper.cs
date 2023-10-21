namespace Libgit2Bindings.Mappers;

internal static class GitRemoteRedirectOptionsMapper
{
  public static libgit2.GitRemoteRedirectT ToNative(this GitRemoteRedirectOptions managedOptions)
  {
    return managedOptions switch
    {
      GitRemoteRedirectOptions.Unspecified => 0,
      GitRemoteRedirectOptions.RedirectNone => libgit2.GitRemoteRedirectT.GIT_REMOTE_REDIRECT_NONE,
      GitRemoteRedirectOptions.RedirectInitial => libgit2.GitRemoteRedirectT.GIT_REMOTE_REDIRECT_INITIAL,
      GitRemoteRedirectOptions.RedirectAll => libgit2.GitRemoteRedirectT.GIT_REMOTE_REDIRECT_ALL,
      _ => throw new ArgumentOutOfRangeException(nameof(managedOptions), managedOptions, null)
    };
  }
}
