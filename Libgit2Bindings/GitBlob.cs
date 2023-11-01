using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal class GitBlob : IGitBlob
{
  private readonly libgit2.GitBlob _nativeGitBlob;
  private bool _ownsNativeInstance;

  public GitBlob(libgit2.GitBlob nativeGitBlob, bool ownsNativeInstance = true)
  {
    _nativeGitBlob = nativeGitBlob;
    _ownsNativeInstance = ownsNativeInstance;
  }

  public bool IsBinary()
  {
    return libgit2.blob.GitBlobIsBinary(_nativeGitBlob) != 0;
  }

  public IGitBlob Duplicate()
  {
    var res = libgit2.blob.GitBlobDup(out var nativeDuplicate, _nativeGitBlob);
    CheckLibgit2.Check(res, "Unable to duplicate blob");
    return new GitBlob(nativeDuplicate);
  }

  public byte[] Filter(string asPath, GitBlobFilterOptions? options)
  {
    using var scope = new DisposableCollection();
    var nativeOptions = options?.ToNative(scope);
    var res = libgit2.blob.GitBlobFilter(out var nativeBuf, _nativeGitBlob, asPath, nativeOptions);
    CheckLibgit2.Check(res, "Unable to filter blob");
    using var gitBufDisposer = new GitBufDisposer(nativeBuf);
    return StringUtil.ToArray(nativeBuf);
  }

  public GitOid Id()
  {
    using var nativeOid = libgit2.blob.GitBlobId(_nativeGitBlob);
    return GitOidMapper.FromNative(nativeOid);
  }

  public IGitRepository Owner()
  {
    var nativeRepo = libgit2.blob.GitBlobOwner(_nativeGitBlob);
    return new GitRepository(nativeRepo, false);
  }

  public byte[] RawContent()
  {
    var bufferPtr = libgit2.blob.GitBlobRawcontent(_nativeGitBlob);
    var bufferSize = libgit2.blob.GitBlobRawsize(_nativeGitBlob);
    return StringUtil.ToArray(bufferPtr, (UIntPtr)bufferSize);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (_ownsNativeInstance)
      {
        libgit2.blob.GitBlobFree(_nativeGitBlob);
      }
      _disposedValue = true;
    }
  }

  ~GitBlob()
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
