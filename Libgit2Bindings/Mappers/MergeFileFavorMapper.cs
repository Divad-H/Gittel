namespace Libgit2Bindings.Mappers;

internal static class MergeFileFavorMapper
{
  public static libgit2.GitMergeFileFavorT ToNative(this MergeFileFavor managedFavor)
  {
    return managedFavor switch
    {
      MergeFileFavor.Normal => libgit2.GitMergeFileFavorT.GIT_MERGE_FILE_FAVOR_NORMAL,
      MergeFileFavor.Ours => libgit2.GitMergeFileFavorT.GIT_MERGE_FILE_FAVOR_OURS,
      MergeFileFavor.Theirs => libgit2.GitMergeFileFavorT.GIT_MERGE_FILE_FAVOR_THEIRS,
      MergeFileFavor.Union => libgit2.GitMergeFileFavorT.GIT_MERGE_FILE_FAVOR_UNION,
      _ => throw new ArgumentException(nameof(managedFavor))
    };
  }
}
