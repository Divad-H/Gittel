using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

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
    var managedAuthor = GittelObjects.Downcast<GitSignature>(author);
    var managedCommitter = GittelObjects.Downcast<GitSignature>(committer);
    var managedTree = GittelObjects.Downcast<GitTree>(tree);

    var data = new libgit2.GitOid.__Internal();
    using var commitOid = libgit2.GitOid.__CreateInstance(data);
    var res = libgit2.commit.GitCommitAmend(
      commitOid, _nativeGitCommit, updateRef, managedAuthor?.NativeGitSignature,
      managedCommitter?.NativeGitSignature, messageEncoding, message, managedTree?.NativeGitTree);
    CheckLibgit2.Check(res, "Unable to amend commit");
    return GitOidMapper.FromNative(commitOid);
  }

  public IGitSignature GetAuthor()
  {
    var nativeAuthor = libgit2.commit.GitCommitAuthor(_nativeGitCommit);
    return new GitSignature(nativeAuthor);
  }

  public IGitSignature GetAuthor(IGitMailmap? mailmap)
  {
    var managedMailmap = GittelObjects.Downcast<GitMailmap>(mailmap);
    var res = libgit2.commit.GitCommitAuthorWithMailmap(
      out var nativeAuthor, _nativeGitCommit, managedMailmap?.NativeGitMailmap);
    CheckLibgit2.Check(res, "Unable to get author");
    return new GitSignature(nativeAuthor);
  }

  public IGitSignature GetCommitter()
  {
    var nativeCommitter = libgit2.commit.GitCommitCommitter(_nativeGitCommit);
    return new GitSignature(nativeCommitter);
  }

  public IGitSignature GetCommitter(IGitMailmap? mailmap)
  {
    var managedMailmap = GittelObjects.Downcast<GitMailmap>(mailmap);
    var res = libgit2.commit.GitCommitCommitterWithMailmap(
      out var nativeCommitter, _nativeGitCommit, managedMailmap?.NativeGitMailmap);
    CheckLibgit2.Check(res, "Unable to get committer");
    return new GitSignature(nativeCommitter);
  }

  public string? GetBody()
  {
    return libgit2.commit.GitCommitBody(_nativeGitCommit);
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
