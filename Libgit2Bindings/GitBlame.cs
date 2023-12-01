using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal class GitBlame : IGitBlame
{
  private readonly libgit2.GitBlame _nativeGitBlame;
  private bool _ownsNativeInstance;

  public GitBlame(libgit2.GitBlame nativeGitBlame, bool ownsNativeInstance = true)
  {
    _nativeGitBlame = nativeGitBlame;
    _ownsNativeInstance = ownsNativeInstance;
  }

  public GitBlameHunk? GetHunkByIndex(uint index)
  {
    using var nativeHunk = libgit2.blame.GitBlameGetHunkByindex(_nativeGitBlame, index);
    return GitBlameHunkMapper.FromNative(nativeHunk);
  }

  public GitBlameHunk? GetHunkByLine(ulong lineNumber)
  {
    using var nativeHunk = libgit2.blame.GitBlameGetHunkByline(_nativeGitBlame, (UIntPtr)lineNumber);
    return GitBlameHunkMapper.FromNative(nativeHunk);
  }

  public uint GetHunkCount()
  {
    return libgit2.blame.GitBlameGetHunkCount(_nativeGitBlame);
  }

  public IGitBlame BlameBuffer(byte[] buffer)
  {
    using PinnedBuffer pinnedBuffer = new(buffer);
    var res = libgit2.blame.GitBlameBuffer(
      out var nativeBlame, _nativeGitBlame, pinnedBuffer.Pointer, (UIntPtr)pinnedBuffer.Length);
    CheckLibgit2.Check(res, "Unable to apply buffer to blame");
    return new GitBlame(nativeBlame);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (_ownsNativeInstance)
      {
        libgit2.blame.GitBlameFree(_nativeGitBlame);
      }
      _disposedValue = true;
    }
  }

  ~GitBlame()
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
