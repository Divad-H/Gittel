namespace Libgit2Bindings.Mappers;

internal static class GitRemoteDirectionMapper
{
  public static GitRemoteDirection FromNative(libgit2.GitDirection nativeGitRemoteDirection)
  {
    return nativeGitRemoteDirection switch
    {
      libgit2.GitDirection.GIT_DIRECTION_FETCH => GitRemoteDirection.Fetch,
      libgit2.GitDirection.GIT_DIRECTION_PUSH => GitRemoteDirection.Push,
      _ => throw new ArgumentOutOfRangeException(nameof(nativeGitRemoteDirection), nativeGitRemoteDirection, null)
    };
  }
}
