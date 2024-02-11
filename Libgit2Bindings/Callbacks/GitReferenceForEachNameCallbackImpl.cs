using Libgit2Bindings.Util;
using System.Runtime.InteropServices;

namespace Libgit2Bindings.Callbacks;

internal sealed class GitReferenceForEachNameCallbackImpl: IDisposable
{
  private readonly Func<string, GitOperationContinuation> _callback;
  private readonly GCHandle _gcHandle;

  public GitReferenceForEachNameCallbackImpl(Func<string, GitOperationContinuation> callback)
  {
    _callback = callback;
    _gcHandle = GCHandle.Alloc(this);
  }

  public IntPtr Payload => GCHandle.ToIntPtr(_gcHandle);

  public static int GitReferenceForEachNameCb(string name, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (GitReferenceForEachNameCallbackImpl)gcHandle.Target!;

      return callbacks._callback(name);
    };

    return func.ExecuteInTryCatch(nameof(GitReferenceForEachNameCb));
  }

  public void Dispose()
  {
    _gcHandle.Free();
  }
}
