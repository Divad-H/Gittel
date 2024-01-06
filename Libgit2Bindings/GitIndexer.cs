using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal class GitIndexer(
  libgit2.GitIndexer nativeGitIndexer,
  DisposableCollection disposables
) : IGitIndexer
{
  public libgit2.GitIndexer NativeGitIndexer { get; } = nativeGitIndexer;
  private readonly DisposableCollection _disposables = disposables;
  private readonly libgit2.GitIndexerProgress _progress = new();

  public string Name => libgit2.indexer.GitIndexerName(NativeGitIndexer);

  public GitIndexerProgress Commit()
  {
    var res = libgit2.indexer.GitIndexerCommit(NativeGitIndexer, _progress);
    CheckLibgit2.Check(res, "Unable to commit indexer");
    return GitIndexerProgressMapper.FromNative(_progress);
  }

  public GitIndexerProgress Append(byte[] data)
  {
    using var pinnedData = new PinnedBuffer(data);
    var res = libgit2.indexer.GitIndexerAppend(
      NativeGitIndexer, pinnedData.Pointer, (UIntPtr)pinnedData.Length, _progress);
    CheckLibgit2.Check(res, "Unable to append to indexer");
    return GitIndexerProgressMapper.FromNative(_progress);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      _progress.Dispose();
      libgit2.indexer.GitIndexerFree(NativeGitIndexer);
      _disposables.Dispose();
      _disposedValue = true;
    }
  }

  ~GitIndexer()
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
