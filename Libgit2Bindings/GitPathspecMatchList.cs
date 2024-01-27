using Libgit2Bindings.Mappers;

namespace Libgit2Bindings;

internal sealed class GitPathspecMatchList(
  libgit2.GitPathspecMatchList nativeGitPathspecMatchList
) : IGitPathspecMathList
{
  public libgit2.GitPathspecMatchList NativeGitPathspecMatchList { get; } = nativeGitPathspecMatchList;

  public nuint FailedEntrycount 
    => (nuint)libgit2.pathspec.GitPathspecMatchListFailedEntrycount(NativeGitPathspecMatchList);

  public nuint Entrycount 
    => (nuint)libgit2.pathspec.GitPathspecMatchListEntrycount(NativeGitPathspecMatchList);

  public string? FailedEntry(nuint index)
  {
    return libgit2.pathspec.GitPathspecMatchListFailedEntry(NativeGitPathspecMatchList, index);
  }

  public string? Entry(nuint index)
  {
    return libgit2.pathspec.GitPathspecMatchListEntry(NativeGitPathspecMatchList, index);
  }

  public GitDiffDelta? DiffEntry(nuint index)
  {
    using var nativeDiffEntry = libgit2.pathspec.GitPathspecMatchListDiffEntry(NativeGitPathspecMatchList, index);
    if (nativeDiffEntry.__Instance == IntPtr.Zero)
    {
      return null;
    }
    return GitDiffDeltaMapper.FromNative(nativeDiffEntry);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.pathspec.GitPathspecMatchListFree(NativeGitPathspecMatchList);
      _disposedValue = true;
    }
  }

  ~GitPathspecMatchList()
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
