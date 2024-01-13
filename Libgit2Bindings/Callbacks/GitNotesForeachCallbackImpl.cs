using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;
using System.Runtime.InteropServices;

namespace Libgit2Bindings.Callbacks;

internal class GitNotesForeachCallbackImpl : IDisposable
{
  private readonly GCHandle _gcHandle;
  private readonly GitNoteForeachCallback _callback;

  public GitNotesForeachCallbackImpl(GitNoteForeachCallback callback)
  {
    _callback = callback;

    _gcHandle = GCHandle.Alloc(this);
  }

  public IntPtr Payload => GCHandle.ToIntPtr(_gcHandle);

  public static int GitNotesForeachCb(IntPtr blobId, IntPtr annotatedOid, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (GitNotesForeachCallbackImpl)gcHandle.Target!;
      var blobOid = GitOidMapper.FromNativePtr(blobId);
      var annotatedOidOid = GitOidMapper.FromNativePtr(annotatedOid);
      return callbacks._callback(blobOid, annotatedOidOid);
    };

    return func.ExecuteInTryCatch(nameof(GitNoteForeachCallback));
  }

  public void Dispose()
  {
    _gcHandle.Free();
  }
}
