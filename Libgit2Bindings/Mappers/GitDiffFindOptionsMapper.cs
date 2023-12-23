using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class GitDiffFindOptionsMapper
{
  public static libgit2.GitDiffFindOptions ToNative(this GitDiffFindOptions options,
    DisposableCollection disposables)
  {
    var nativeOptions = new libgit2.GitDiffFindOptions();

    try
    {
      libgit2.diff.GitDiffFindOptionsInit(nativeOptions,
        (UInt32)libgit2.GitDiffFindOptionsVersion.GIT_DIFF_FIND_OPTIONS_VERSION);

      nativeOptions.Flags = (UInt32)GitDiffFindFlagsMapper.ToNative(options.Flags);
      nativeOptions.RenameThreshold = options.RenameThreshold;
      nativeOptions.RenameFromRewriteThreshold = options.RenameFromRewriteThreshold;
      nativeOptions.CopyThreshold = options.CopyThreshold;
      nativeOptions.BreakRewriteThreshold = options.BreakRewriteThreshold;
      nativeOptions.RenameLimit = options.RenameLimit;

      if (options.SimilarityMetric is not null)
      {
        var callbacksImpl = new SimilarityMetricImpl(options.SimilarityMetric)
          .DisposeWith(disposables);

        nativeOptions.Metric = callbacksImpl.ToNative().DisposeWith(disposables);
      }
    }
    catch (Exception)
    {
      nativeOptions.Dispose();
      throw;
    }

    return nativeOptions;
  }
}
