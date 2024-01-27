using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal sealed class GitPathspec : IGitPathspec
{
  public libgit2.GitPathspec NativeGitPathspec { get; }

  public GitPathspec(libgit2.GitPathspec nativeGitPathspec)
  {
    NativeGitPathspec = nativeGitPathspec;
  }

  public bool MatchesPath(GitPathspecFlags flags, string path)
  {
    var res = libgit2.pathspec.GitPathspecMatchesPath(NativeGitPathspec, (UInt32)flags, path);
    if (res < 0)
    {
      CheckLibgit2.Check(res, "Unable to match path '{0}'", path);
    }
    return res == 1;
  }

  public IGitPathspecMathList MatchWorkdir(IGitRepository repo, GitPathspecFlags flags)
  {
    var managedRepo = GittelObjects.DowncastNonNull<GitRepository>(repo);
    var res = libgit2.pathspec.GitPathspecMatchWorkdir(
      out var nativeGitPathspecMatchList, managedRepo.NativeGitRepository, (UInt32)flags, NativeGitPathspec);
    CheckLibgit2.Check(res, "Unable to match workdir");
    return new GitPathspecMatchList(nativeGitPathspecMatchList);
  }

  public IGitPathspecMathList MatchIndex(IGitIndex index, GitPathspecFlags flags)
  {
    var managedIndex = GittelObjects.DowncastNonNull<GitIndex>(index);
    var res = libgit2.pathspec.GitPathspecMatchIndex(
      out var nativeGitPathspecMatchList, managedIndex.NativeGitIndex, (UInt32)flags, NativeGitPathspec);
    CheckLibgit2.Check(res, "Unable to match index");
    return new GitPathspecMatchList(nativeGitPathspecMatchList);
  }

  public IGitPathspecMathList MatchTree(IGitTree tree, GitPathspecFlags flags)
  {
    var managedTree = GittelObjects.DowncastNonNull<GitTree>(tree);
    var res = libgit2.pathspec.GitPathspecMatchTree(
      out var nativeGitPathspecMatchList, managedTree.NativeGitTree, (UInt32)flags, NativeGitPathspec);
    CheckLibgit2.Check(res, "Unable to match tree");
    return new GitPathspecMatchList(nativeGitPathspecMatchList);
  }

  public IGitPathspecMathList MatchDiff(IGitDiff diff, GitPathspecFlags flags)
  {
    var managedDiff = GittelObjects.DowncastNonNull<GitDiff>(diff);
    var res = libgit2.pathspec.GitPathspecMatchDiff(
      out var nativeGitPathspecMatchList, managedDiff.NativeGitDiff, (UInt32)flags, NativeGitPathspec);
    CheckLibgit2.Check(res, "Unable to match diff");
    return new GitPathspecMatchList(nativeGitPathspecMatchList, true);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.pathspec.GitPathspecFree(NativeGitPathspec);
      _disposedValue = true;
    }
  }

  ~GitPathspec()
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
