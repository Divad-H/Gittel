using Libgit2Bindings.Mappers;

namespace Libgit2Bindings;

internal class GitNote : IGitNote
{
  private readonly libgit2.GitNote _nativeGitNote;

  public GitNote(libgit2.GitNote nativeGitNote)
  {
    _nativeGitNote = nativeGitNote;
  }

  #region IDisposable Support
  private bool _disposedValue;

  public IGitSignature Author
  {
    get
    {
      var author = libgit2.notes.GitNoteAuthor(_nativeGitNote);
      return new GitSignature(author);
    }
  }

  public IGitSignature Committer
  {
    get
    {
      var committer = libgit2.notes.GitNoteCommitter(_nativeGitNote);
      return new GitSignature(committer);
    }
  }

  public string Message
  {
    get
    {
      var message = libgit2.notes.GitNoteMessage(_nativeGitNote);
      return message;
    }
  }

  public GitOid Id
  {
    get
    {
      using var oid = libgit2.notes.GitNoteId(_nativeGitNote);
      return GitOidMapper.FromNative(oid);
    }
  }

  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.notes.GitNoteFree(_nativeGitNote);
      _disposedValue = true;
    }
  }

  ~GitNote()
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
