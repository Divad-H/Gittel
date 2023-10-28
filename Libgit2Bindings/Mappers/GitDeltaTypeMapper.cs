namespace Libgit2Bindings.Mappers;

internal static class GitDeltaTypeMapper
{
  public static GitDeltaType FromNative(libgit2.GitDeltaT nativeDelta)
  {
    return nativeDelta switch
    {
      libgit2.GitDeltaT.GIT_DELTA_UNMODIFIED => GitDeltaType.Unmodified,
      libgit2.GitDeltaT.GIT_DELTA_ADDED => GitDeltaType.Added,
      libgit2.GitDeltaT.GIT_DELTA_DELETED => GitDeltaType.Deleted,
      libgit2.GitDeltaT.GIT_DELTA_MODIFIED => GitDeltaType.Modified,
      libgit2.GitDeltaT.GIT_DELTA_RENAMED => GitDeltaType.Renamed,
      libgit2.GitDeltaT.GIT_DELTA_COPIED => GitDeltaType.Copied,
      libgit2.GitDeltaT.GIT_DELTA_IGNORED => GitDeltaType.Ignored,
      libgit2.GitDeltaT.GIT_DELTA_UNTRACKED => GitDeltaType.Untracked,
      libgit2.GitDeltaT.GIT_DELTA_TYPECHANGE => GitDeltaType.Typechange,
      libgit2.GitDeltaT.GIT_DELTA_UNREADABLE => GitDeltaType.Unreadable,
      libgit2.GitDeltaT.GIT_DELTA_CONFLICTED => GitDeltaType.Conflicted,
      _ => throw new ArgumentOutOfRangeException(nameof(nativeDelta), nativeDelta, null)
    };
  }
}
