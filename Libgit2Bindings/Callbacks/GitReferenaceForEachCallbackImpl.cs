using Libgit2Bindings.Util;
using System.Runtime.InteropServices;

namespace Libgit2Bindings.Callbacks;

internal sealed class GitReferenaceForEachCallbackImpl : IDisposable
{
  private readonly Func<IGitReference, GitOperationContinuation> _callback;
  private readonly GCHandle _gcHandle;

  public GitReferenaceForEachCallbackImpl(Func<IGitReference, GitOperationContinuation> callback)
  {
    _callback = callback;
    _gcHandle = GCHandle.Alloc(this);
  }

  public IntPtr Payload => GCHandle.ToIntPtr(_gcHandle);

  public static int GitReferenceForEachCb(IntPtr reference, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (GitReferenaceForEachCallbackImpl)gcHandle.Target!;

      var nativeReference = libgit2.GitReference.__CreateInstance(reference);
      using var managedReference = new GitReference(nativeReference);

      return callbacks._callback(managedReference);
    };

    return func.ExecuteInTryCatch(nameof(GitReferenceForEachCb));
  }

  public void Dispose()
  {
    _gcHandle.Free();
  }
}
