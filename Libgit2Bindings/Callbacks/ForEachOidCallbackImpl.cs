using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;
using System.Runtime.InteropServices;

namespace Libgit2Bindings.Callbacks;

internal sealed class ForEachOidCallbackImpl : IDisposable
{
  private readonly GCHandle _gcHandle;

  private readonly Func<GitOid, GitOperationContinuation> _callback;

  public ForEachOidCallbackImpl(Func<GitOid, GitOperationContinuation> callback)
  {
    _callback = callback;

    _gcHandle = GCHandle.Alloc(this);
  }
  public IntPtr Payload => GCHandle.ToIntPtr(_gcHandle);

  public static int GitForEachOidCb(IntPtr oid, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (ForEachOidCallbackImpl)gcHandle.Target!;

      using var gitOid = libgit2.GitOid.__CreateInstance(oid);
      var oidToReturn = GitOidMapper.FromNative(gitOid);

      return callbacks._callback(oidToReturn);
    };

    return func.ExecuteInTryCatch(nameof(GitForEachOidCb));
  }


  public void Dispose()
  {
     _gcHandle.Free();
  }
}
