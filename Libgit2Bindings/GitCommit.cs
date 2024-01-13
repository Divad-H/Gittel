using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;
using System.Text;

namespace Libgit2Bindings;

internal sealed class GitCommit : IGitCommit
{
  private readonly libgit2.GitCommit _nativeGitCommit;
  public libgit2.GitCommit NativeGitCommit => _nativeGitCommit;

  public IGitRepository Owner { get; }

  public GitCommit(libgit2.GitCommit nativeGitCommit, IGitRepository owner)
  {
    _nativeGitCommit = nativeGitCommit;
    Owner = owner;
  }

  public GitOid Amend(string? updateRef, IGitSignature? author, IGitSignature? committer, 
    string? message, IGitTree? tree)
  {
    var managedAuthor = GittelObjects.Downcast<GitSignature>(author);
    var managedCommitter = GittelObjects.Downcast<GitSignature>(committer);
    var managedTree = GittelObjects.Downcast<GitTree>(tree);

    var data = new libgit2.GitOid.__Internal();
    using var commitOid = libgit2.GitOid.__CreateInstance(data);
    var res = libgit2.commit.GitCommitAmend(
      commitOid, _nativeGitCommit, updateRef, managedAuthor?.NativeGitSignature,
      managedCommitter?.NativeGitSignature, null, message, managedTree?.NativeGitTree);
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

  private Encoding GetMessageEncoding()
  {
    var encoding = Encoding.UTF8;
    var encodingStr = libgit2.commit.GitCommitMessageEncoding(_nativeGitCommit);
    if (encodingStr != null)
    {
      encoding = Encoding.GetEncoding(encodingStr);
    }

    return encoding;
  }

  public string GetMessage()
  {
    var encoding = GetMessageEncoding();

    var messagePtr = libgit2.commit.__Internal.GitCommitMessage(_nativeGitCommit.__Instance);
    if (messagePtr == IntPtr.Zero)
    {
      return string.Empty;
    }
    return CppSharp.Runtime.MarshalUtil.GetString(encoding, messagePtr);
  }

  public string GetRawMessage()
  {
    var encoding = GetMessageEncoding();

    var rawMessagePtr = libgit2.commit.__Internal.GitCommitMessageRaw(_nativeGitCommit.__Instance);
    if (rawMessagePtr == IntPtr.Zero)
    {
      return string.Empty;
    }
    return CppSharp.Runtime.MarshalUtil.GetString(encoding, rawMessagePtr);
  }

  public string? GetBody()
  {
    var encoding = GetMessageEncoding();

    var bodyPtr = libgit2.commit.__Internal.GitCommitBody(_nativeGitCommit.__Instance);
    if (bodyPtr == IntPtr.Zero)
    {
      return null;
    }
    return CppSharp.Runtime.MarshalUtil.GetString(encoding, bodyPtr);
  }

  public string? GetSummary()
  {
    var encoding = GetMessageEncoding();

    var summaryPtr = libgit2.commit.__Internal.GitCommitSummary(_nativeGitCommit.__Instance);
    if (summaryPtr == IntPtr.Zero)
    {
      return null;
    }
    return CppSharp.Runtime.MarshalUtil.GetString(encoding, summaryPtr);
  }

  public byte[] GetHeaderField(string field)
  {
    var res = libgit2.commit.GitCommitHeaderField(out var headerField, _nativeGitCommit, field);
    using (headerField)
    {
      CheckLibgit2.Check(res, "Unable to get header field");
      return StringUtil.ToArray(headerField);
    }
  }

  public IGitTree GetTree()
  {
    var res = libgit2.commit.GitCommitTree(out var nativeGitTree, _nativeGitCommit);
    CheckLibgit2.Check(res, "Unable to get tree");
    return new GitTree(nativeGitTree);
  }

  public GitOid GetTreeId()
  {
    using var res = libgit2.commit.GitCommitTreeId(_nativeGitCommit);
    return GitOidMapper.FromNative(res);
  }

  public GitOid GetId()
  {
    using var res = libgit2.commit.GitCommitId(_nativeGitCommit);
    return GitOidMapper.FromNative(res);
  }

  public IGitCommit GetParent(uint n)
  {
    var res = libgit2.commit.GitCommitParent(out var nativeGitCommit, _nativeGitCommit, n);
    CheckLibgit2.Check(res, "Unable to get parent");
    return new GitCommit(nativeGitCommit, Owner);
  }

  public uint GetParentCount()
  {
    return libgit2.commit.GitCommitParentcount(_nativeGitCommit);
  }

  public GitOid? GetParentId(uint n)
  {
    using var res = libgit2.commit.GitCommitParentId(_nativeGitCommit, n);
    if (res == null)
    {
      return null;
    }
    return GitOidMapper.FromNative(res);
  }

  public IGitCommit GetNthAncestor(uint n)
  {
    var res = libgit2.commit.GitCommitNthGenAncestor(out var nativeGitCommit, _nativeGitCommit, n);
    CheckLibgit2.Check(res, "Unable to get nth ancestor");
    return new GitCommit(nativeGitCommit, Owner);
  }

  public string GetRawHeader()
  {
    return libgit2.commit.GitCommitRawHeader(_nativeGitCommit);
  }

  public DateTimeOffset GetCommitTime()
  {
    var secondsSinceEpoch = libgit2.commit.GitCommitTime(_nativeGitCommit);
    var offsetMinutesFromUtc = libgit2.commit.GitCommitTimeOffset(_nativeGitCommit);
    return GitSignature.FromEpochAndOffset(secondsSinceEpoch, offsetMinutesFromUtc);
  }

  public IEnumerable<(GitOid NoteId, GitOid AnnotatedId)> IterateNotes()
  {
    var res = libgit2.notes.GitNoteCommitIteratorNew(out var noteIterator, _nativeGitCommit);
    CheckLibgit2.Check(res, "Unable to get note iterator");
    try
    {
      while (true)
      {
        res = libgit2.notes.GitNoteNext(out var noteId, out var annotatedId, noteIterator);
        if (res == (int)libgit2.GitErrorCode.GIT_ITEROVER)
        {
          break;
        }
        CheckLibgit2.Check(res, "Unable to get next note");
        using (noteId) using (annotatedId)
        {
          yield return (GitOidMapper.FromNative(noteId), GitOidMapper.FromNative(annotatedId));
        }
      }
    }
    finally
    {
      libgit2.notes.GitNoteIteratorFree(noteIterator);
    }
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
