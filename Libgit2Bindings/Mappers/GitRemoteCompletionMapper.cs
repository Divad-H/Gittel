namespace Libgit2Bindings.Mappers;

internal static class GitRemoteCompletionMapper
{
  public static libgit2.GitRemoteCompletionT ToNative(this RemoteCompletionType managedCompletion)
  {
    return managedCompletion switch
    {
      RemoteCompletionType.Download => libgit2.GitRemoteCompletionT.GIT_REMOTE_COMPLETION_DOWNLOAD,
      RemoteCompletionType.Indexing => libgit2.GitRemoteCompletionT.GIT_REMOTE_COMPLETION_INDEXING,
      RemoteCompletionType.Error => libgit2.GitRemoteCompletionT.GIT_REMOTE_COMPLETION_ERROR,
      _ => throw new ArgumentOutOfRangeException(nameof(managedCompletion), managedCompletion, null)
    };
  }

  public static RemoteCompletionType ToManaged(this libgit2.GitRemoteCompletionT nativeCompletion)
  {
    return nativeCompletion switch
    {
      libgit2.GitRemoteCompletionT.GIT_REMOTE_COMPLETION_DOWNLOAD => RemoteCompletionType.Download,
      libgit2.GitRemoteCompletionT.GIT_REMOTE_COMPLETION_INDEXING => RemoteCompletionType.Indexing,
      libgit2.GitRemoteCompletionT.GIT_REMOTE_COMPLETION_ERROR => RemoteCompletionType.Error,
      _ => throw new ArgumentOutOfRangeException(nameof(nativeCompletion), nativeCompletion, null)
    };
  }
}
