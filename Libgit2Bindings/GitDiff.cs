using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal class GitDiff : IGitDiff
{
  public libgit2.GitDiff NativeGitDiff { get; }
  private bool _ownsNativeInstance;

  public GitDiff(libgit2.GitDiff nativeGitDiff, bool ownsNativeInstance = true)
  {
    NativeGitDiff = nativeGitDiff;
    _ownsNativeInstance = ownsNativeInstance;
  }

  public GitDiffDelta? GetDelta(UInt64 index)
  {
    var nativeDelta = libgit2.diff.GitDiffGetDelta(NativeGitDiff, (UIntPtr)index);
    if (nativeDelta is null)
    {
      return null;
    }
    return GitDiffDeltaMapper.FromNative(nativeDelta);
  }

  public UInt64 GetNumDeltas()
  {
    return libgit2.diff.GitDiffNumDeltas(NativeGitDiff);
  }

  public void FindSimilar(GitDiffFindOptions? options = null)
  {
    using DisposableCollection disposables = new();

    var nativeOptions = options?.ToNative(disposables);
    var res = libgit2.diff.GitDiffFindSimilar(NativeGitDiff, nativeOptions);

    CheckLibgit2.Check(res, "Find similar failed");
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (_ownsNativeInstance)
      {
        libgit2.diff.GitDiffFree(NativeGitDiff);
      }
      _disposedValue = true;
    }
  }

  ~GitDiff()
  {
    Dispose(disposing: false);
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
  #endregion
}
