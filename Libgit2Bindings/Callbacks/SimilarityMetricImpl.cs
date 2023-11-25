using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;
using System.Runtime.InteropServices;

namespace Libgit2Bindings.Callbacks;

internal sealed class SimilarityMetricImpl : IDisposable
{
  private readonly GCHandle _gcHandle;

  private readonly ISimilarityMetric _similarityMetric;

  public SimilarityMetricImpl(ISimilarityMetric similarityMetric)
  {
    _similarityMetric = similarityMetric;

    _gcHandle = GCHandle.Alloc(this);
  }

  public IntPtr Payload => GCHandle.ToIntPtr(_gcHandle);

  public unsafe static int FileSignature(IntPtr* @out, IntPtr file, string path, IntPtr payload)
  {
    Func<int> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (SimilarityMetricImpl)gcHandle.Target!;

      var managedFile = GitDiffFileMapper.FromNativePtr(file) 
        ?? throw new ArgumentNullException(nameof(file));
      var signature = callbacks._similarityMetric.ObjectHashSignatureForFile(managedFile, path);
      var signatureGcHandle = GCHandle.Alloc(signature);

      (*@out) = GCHandle.ToIntPtr(signatureGcHandle);
      return 0;
    };

    return func.ExecuteInTryCatch(nameof(FileSignature));
  }

  public unsafe static int BufferSignature(IntPtr* @out, IntPtr file, IntPtr buffer, UIntPtr bufferLen, IntPtr payload)
  {
    Func<int> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (SimilarityMetricImpl)gcHandle.Target!;

      var managedFile = GitDiffFileMapper.FromNativePtr(file)
        ?? throw new ArgumentNullException(nameof(file));
      var managedBuffer = StringUtil.ToArray(buffer, bufferLen);
      var signature = callbacks._similarityMetric.ObjectHashSignatureForBuffer(managedFile, managedBuffer);
      var signatureGcHandle = GCHandle.Alloc(signature);

      (*@out) = GCHandle.ToIntPtr(signatureGcHandle);
      return 0;
    };

    return func.ExecuteInTryCatch(nameof(BufferSignature));
  }

  public static void FreeSignature(IntPtr signature, IntPtr payload)
  {
    try
    {
      GCHandle signatureHandle = GCHandle.FromIntPtr(signature);
      signatureHandle.Free();
    }
    catch (Exception)
    {
      // ignore
    }
  }

  public static unsafe int Similarity(int* score, IntPtr sigA, IntPtr sigB, IntPtr payload)
  {
    Func<int> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (SimilarityMetricImpl)gcHandle.Target!;

      var signatureA = GCHandle.FromIntPtr(sigA).Target
        ?? throw new ArgumentNullException(nameof(sigA));
      var signatureB = GCHandle.FromIntPtr(sigB).Target
        ?? throw new ArgumentNullException(nameof(sigB));

      var similarity = callbacks._similarityMetric.Similarity(signatureA, signatureB);
      *score = similarity;
      return 0;
    };

    return func.ExecuteInTryCatch(nameof(Similarity));
  }

  public void Dispose()
  {
    _gcHandle.Free();
  }
}
