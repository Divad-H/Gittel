using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal class GitBlob(libgit2.GitBlob nativeGitBlob, bool ownsNativeInstance = true) : IGitBlob
{
  public libgit2.GitBlob NativeGitBlob { get; } = nativeGitBlob;
  private readonly bool _ownsNativeInstance = ownsNativeInstance;

  public bool IsBinary()
  {
    return libgit2.blob.GitBlobIsBinary(NativeGitBlob) != 0;
  }

  public IGitBlob Duplicate()
  {
    var res = libgit2.blob.GitBlobDup(out var nativeDuplicate, NativeGitBlob);
    CheckLibgit2.Check(res, "Unable to duplicate blob");
    return new GitBlob(nativeDuplicate);
  }

  public byte[] Filter(string asPath, GitBlobFilterOptions? options)
  {
    using var scope = new DisposableCollection();
    var nativeOptions = options?.ToNative(scope);
    var res = libgit2.blob.GitBlobFilter(out var nativeBuf, NativeGitBlob, asPath, nativeOptions);
    CheckLibgit2.Check(res, "Unable to filter blob");
    using var gitBufDisposer = new GitBufDisposer(nativeBuf);
    return StringUtil.ToArray(nativeBuf);
  }

  public GitOid Id()
  {
    using var nativeOid = libgit2.blob.GitBlobId(NativeGitBlob);
    return GitOidMapper.FromNative(nativeOid);
  }

  public IGitRepository Owner()
  {
    var nativeRepo = libgit2.blob.GitBlobOwner(NativeGitBlob);
    return new GitRepository(nativeRepo, false);
  }

  public byte[] RawContent()
  {
    var bufferPtr = libgit2.blob.GitBlobRawcontent(NativeGitBlob);
    var bufferSize = libgit2.blob.GitBlobRawsize(NativeGitBlob);
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
        libgit2.blob.GitBlobFree(NativeGitBlob);
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
