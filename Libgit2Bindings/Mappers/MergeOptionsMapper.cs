using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class MergeOptionsMapper
{
  public static libgit2.GitMergeOptions ToNative(
    this MergeOptions managedOptions,
    DisposableCollection disposables)
  {
    var nativeOptions = new libgit2.GitMergeOptions();
    try
    {
      libgit2.merge.GitMergeOptionsInit(nativeOptions,
        (uint)libgit2.GitMergeOptionsVersion.GIT_MERGE_OPTIONS_VERSION);

      nativeOptions.Flags = (UInt32)managedOptions.Flags.ToNative();
      nativeOptions.RenameThreshold = managedOptions.RenameThreshold;
      nativeOptions.TargetLimit = managedOptions.TargetLimit;

      if (managedOptions.SimilarityMetric is not null)
      {
        var callbacksImpl = new SimilarityMetricImpl(managedOptions.SimilarityMetric)
          .DisposeWith(disposables);

        nativeOptions.Metric = callbacksImpl.ToNative().DisposeWith(disposables);
      }

      nativeOptions.RecursionLimit = managedOptions.RecursionLimit;
      nativeOptions.DefaultDriver = managedOptions.DefaultDriver;

      nativeOptions.FileFavor = managedOptions.MergeFileFavor.ToNative();
      nativeOptions.FileFlags = (UInt32)managedOptions.FileFlags.ToNative();

      return nativeOptions;
    }
    catch (Exception)
    {
      nativeOptions.Dispose();
      throw;
    }
  }
}
