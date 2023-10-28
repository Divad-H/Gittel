using Libgit2Bindings.Mappers;

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
    var nativeDelta = libgit2.diff.GitDiffGetDelta(NativeGitDiff, index);
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
