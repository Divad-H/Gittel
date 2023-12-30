using Libgit2Bindings.Util;
using System.Runtime.InteropServices;

namespace Libgit2Bindings.Callbacks;

internal class GitIndexMatchedPathCallbackImpl : IDisposable
{
  private readonly GCHandle _gcHandle;

  private readonly GitIndexMatchedPathCallback _matchedPath;

  public GitIndexMatchedPathCallbackImpl(GitIndexMatchedPathCallback matchedPath)
  {
    _matchedPath = matchedPath;

    _gcHandle = GCHandle.Alloc(this);
  }

  public IntPtr Payload => GCHandle.ToIntPtr(_gcHandle);

  public static int GitIndexMatchedPathCb(string path, string matchedPath, IntPtr payload)
  {
    Func<int> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (GitIndexMatchedPathCallbackImpl)gcHandle.Target!;
      var res = callbacks._matchedPath.Invoke(path, matchedPath);
      return (int)res;
    };
    
    return func.ExecuteInTryCatch(nameof(GitIndexMatchedPathCb));
  }

  public void Dispose()
  {
    _gcHandle.Free();
  }
}
