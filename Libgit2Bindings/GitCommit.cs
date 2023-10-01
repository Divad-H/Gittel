using Libgit2Bindings.Mappers;

namespace Libgit2Bindings;

internal sealed class GitCommit : IGitCommit
{
  private readonly libgit2.GitCommit _nativeGitCommit;
  public libgit2.GitCommit NativeGitCommit => _nativeGitCommit;

  public GitCommit(libgit2.GitCommit nativeGitCommit)
  {
    _nativeGitCommit = nativeGitCommit;
  }

  public GitOid Amend(string? updateRef, IGitSignature? author, IGitSignature? committer, 
    string? messageEncoding, string? message, IGitTree? tree)
  {
    var managedAuthor = author is null ? null : author as GitSignature
      ?? throw new ArgumentException($"{nameof(author)} must be created by Gittel");
    var managedCommitter = committer is null ? null : committer as GitSignature
      ?? throw new ArgumentException($"{nameof(committer)} must be created by Gittel");
    var managedTree = tree is null ? null : tree as GitTree
      ?? throw new ArgumentException($"{nameof(tree)} must be created by Gittel");

    var data = new libgit2.GitOid.__Internal();
    using var commitOid = libgit2.GitOid.__CreateInstance(data);
    var res = libgit2.commit.GitCommitAmend(
      commitOid, _nativeGitCommit, updateRef, managedAuthor?.NativeGitSignature,
      managedCommitter?.NativeGitSignature, messageEncoding, message, managedTree?.NativeGitTree);
    CheckLibgit2.Check(res, "Unable to amend commit");
    return GitOidMapper.FromNative(commitOid);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.commit.GitCommitFree(_nativeGitCommit);
      _disposedValue = true;
    }
  }

  ~GitCommit()
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
