using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class GitBlameOptionsMapper
{
  public static libgit2.GitBlameOptions ToNative(this GitBlameOptions options,
    DisposableCollection disposables)
  {
    libgit2.GitBlameOptions res = new();
    res.Version = (Int32)libgit2.GitBlameOptionsVersion.GIT_BLAME_OPTIONS_VERSION;

    if (options.Flags.HasFlag(GitBlameFlags.TrackCopiesSameFile))
      res.Flags |= (uint)libgit2.GitBlameFlagT.GIT_BLAME_TRACK_COPIES_SAME_FILE;
    if (options.Flags.HasFlag(GitBlameFlags.TrackCopiesSameCommitMoves))
      res.Flags |= (uint)libgit2.GitBlameFlagT.GIT_BLAME_TRACK_COPIES_SAME_COMMIT_MOVES;
    if (options.Flags.HasFlag(GitBlameFlags.TrackCopiesSameCommitCopies))
      res.Flags |= (uint)libgit2.GitBlameFlagT.GIT_BLAME_TRACK_COPIES_SAME_COMMIT_COPIES;
    if (options.Flags.HasFlag(GitBlameFlags.TrackCopiesAnyCommitCopies))
      res.Flags |= (uint)libgit2.GitBlameFlagT.GIT_BLAME_TRACK_COPIES_ANY_COMMIT_COPIES;
    if (options.Flags.HasFlag(GitBlameFlags.FirstParent))
      res.Flags |= (uint)libgit2.GitBlameFlagT.GIT_BLAME_FIRST_PARENT;
    if (options.Flags.HasFlag(GitBlameFlags.UseMailmap))
      res.Flags |= (uint)libgit2.GitBlameFlagT.GIT_BLAME_USE_MAILMAP;
    if (options.Flags.HasFlag(GitBlameFlags.IgnoreWhitespace))
      res.Flags |= (uint)libgit2.GitBlameFlagT.GIT_BLAME_IGNORE_WHITESPACE;

    res.MinMatchCharacters = options.MinMatchCharacters;
    if (options.NewestCommit is not null)
      res.NewestCommit = GitOidMapper.ToNative(options.NewestCommit).DisposeWith(disposables);
    if (options.OldestCommit is not null)
      res.OldestCommit = GitOidMapper.ToNative(options.OldestCommit).DisposeWith(disposables);
    res.MinLine = options.MinLine;
    res.MaxLine = options.MaxLine;

    return res;
  }
}
