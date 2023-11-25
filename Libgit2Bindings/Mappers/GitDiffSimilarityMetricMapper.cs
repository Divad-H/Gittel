using Libgit2Bindings.Callbacks;

namespace Libgit2Bindings.Mappers;

internal static class GitDiffSimilarityMetricMapper
{
  public unsafe static libgit2.GitDiffSimilarityMetric ToNative(this SimilarityMetricImpl managedMetric)
  {
    var nativeMetric = new libgit2.GitDiffSimilarityMetric();
    try
    {
      nativeMetric.FileSignature = SimilarityMetricImpl.FileSignature;
      nativeMetric.BufferSignature = SimilarityMetricImpl.BufferSignature;
      nativeMetric.FreeSignature = SimilarityMetricImpl.FreeSignature;
      nativeMetric.Similarity = SimilarityMetricImpl.Similarity;
      nativeMetric.Payload = managedMetric.Payload;

      return nativeMetric;
    }
    catch (Exception)
    {
      nativeMetric.Dispose();
      throw;
    }
  }
}
