using Libgit2Bindings.Mappers;

namespace Libgit2Bindings;

internal sealed class GitPathspecMatchList : IGitPathspecMathList
{
  public libgit2.GitPathspecMatchList NativeGitPathspecMatchList { get; }

  public GitPathspecMatchList(
    libgit2.GitPathspecMatchList nativeGitPathspecMatchList,
    bool copyDiffEntries = false
  )
  {
    NativeGitPathspecMatchList = nativeGitPathspecMatchList;
    if (copyDiffEntries)
    {
      List<GitDiffDelta> diffEntries = new((int)Entrycount);
      for (nuint i = 0; i < Entrycount; i++)
      {
        using var nativeDiffEntry = libgit2.pathspec.GitPathspecMatchListDiffEntry(NativeGitPathspecMatchList, i);
        if (nativeDiffEntry.__Instance != IntPtr.Zero)
        {
          diffEntries.Add(GitDiffDeltaMapper.FromNative(nativeDiffEntry));
        }
      }
      _diffEntries = diffEntries;
    }
  }

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

  private IReadOnlyList<GitDiffDelta>? _diffEntries;
  public GitDiffDelta? DiffEntry(nuint index)
  {
    return _diffEntries?[(int)index];
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
