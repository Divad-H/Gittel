namespace Libgit2Bindings.Mappers;

internal static class GitDiffStatsFromatOptionsMapper
{
  public static libgit2.GitDiffStatsFormatT ToNative(this GitDiffStatsFormatOptions managedFormat)
  {
    libgit2.GitDiffStatsFormatT res = 0;

    if ((managedFormat & GitDiffStatsFormatOptions.None) != 0)
      res |= libgit2.GitDiffStatsFormatT.GIT_DIFF_STATS_NONE;
    if ((managedFormat & GitDiffStatsFormatOptions.Full) != 0)
      res |= libgit2.GitDiffStatsFormatT.GIT_DIFF_STATS_FULL;
    if ((managedFormat & GitDiffStatsFormatOptions.Short) != 0)
      res |= libgit2.GitDiffStatsFormatT.GIT_DIFF_STATS_SHORT;
    if ((managedFormat & GitDiffStatsFormatOptions.Number) != 0)
      res |= libgit2.GitDiffStatsFormatT.GIT_DIFF_STATS_NUMBER;
    if ((managedFormat & GitDiffStatsFormatOptions.IncludeSummary) != 0)
      res |= libgit2.GitDiffStatsFormatT.GIT_DIFF_STATS_INCLUDE_SUMMARY;

    return res;
  }
}
