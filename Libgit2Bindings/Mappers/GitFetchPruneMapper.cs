namespace Libgit2Bindings.Mappers;

internal static class GitFetchPruneMapper
{
  public static GitFetchPruneOptions FromNative(libgit2.GitFetchPruneT nativePrune)
  {
    return nativePrune switch
    {
      libgit2.GitFetchPruneT.GIT_FETCH_PRUNE_UNSPECIFIED => GitFetchPruneOptions.Unspecified,
      libgit2.GitFetchPruneT.GIT_FETCH_PRUNE => GitFetchPruneOptions.Prune,
      libgit2.GitFetchPruneT.GIT_FETCH_NO_PRUNE => GitFetchPruneOptions.NoPrune,
      _ => throw new ArgumentOutOfRangeException(nameof(nativePrune), nativePrune, null)
    };
  }

  internal static libgit2.GitFetchPruneT ToNative(this GitFetchPruneOptions prune)
  {
    return prune switch
    {
      GitFetchPruneOptions.Unspecified => libgit2.GitFetchPruneT.GIT_FETCH_PRUNE_UNSPECIFIED,
      GitFetchPruneOptions.Prune => libgit2.GitFetchPruneT.GIT_FETCH_PRUNE,
      GitFetchPruneOptions.NoPrune => libgit2.GitFetchPruneT.GIT_FETCH_NO_PRUNE,
      _ => throw new ArgumentOutOfRangeException(nameof(prune), prune, null)
    };
  }
}
