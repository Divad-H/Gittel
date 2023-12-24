namespace Libgit2Bindings.Mappers;

internal static class GitDiffStatsFromatOptionsMapper
{
  public static libgit2.GitDiffStatsFormatT ToNative(this IGitDiff.GitDiffStatsFormatOptions managedFormat)
  {
    libgit2.GitDiffStatsFormatT res = 0;

    if ((managedFormat & IGitDiff.GitDiffStatsFormatOptions.None) != 0)
      res |= libgit2.GitDiffStatsFormatT.GIT_DIFF_STATS_NONE;
    if ((managedFormat & IGitDiff.GitDiffStatsFormatOptions.Full) != 0)
      res |= libgit2.GitDiffStatsFormatT.GIT_DIFF_STATS_FULL;
    if ((managedFormat & IGitDiff.GitDiffStatsFormatOptions.Short) != 0)
      res |= libgit2.GitDiffStatsFormatT.GIT_DIFF_STATS_SHORT;
    if ((managedFormat & IGitDiff.GitDiffStatsFormatOptions.Number) != 0)
      res |= libgit2.GitDiffStatsFormatT.GIT_DIFF_STATS_NUMBER;
    if ((managedFormat & IGitDiff.GitDiffStatsFormatOptions.IncludeSummary) != 0)
      res |= libgit2.GitDiffStatsFormatT.GIT_DIFF_STATS_INCLUDE_SUMMARY;

    return res;
  }
}
