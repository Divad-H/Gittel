﻿using Libgit2Bindings.Test.TestData;

namespace Libgit2Bindings.Test;

public class GitNoteTest
{
  [Fact]
  public void CanCreateNoteAndRemove()
  {
    using var repo = new RepoWithOneCommit();
    var noteOid = repo.Repo.CreateNote(
      "refs/notes/test",
      repo.Signature,
      repo.Signature,
      repo.CommitOid,
      "test note",
      false);
    using var note = repo.Repo.ReadNote("refs/notes/test", repo.CommitOid);
    Assert.Equal("test note", note.Message);
    Assert.Equal(repo.Signature.Name, note.Author.Name);
    Assert.Equal(repo.Signature.Email, note.Author.Email);
    Assert.Equal(repo.Signature.Name, note.Committer.Name);
    Assert.Equal(repo.Signature.Email, note.Committer.Email);
    Assert.Equal(noteOid, note.Id);

    repo.Repo.RemoveNote("refs/notes/test",
      repo.Signature,
      repo.Signature, 
      repo.CommitOid);
    Assert.Throws<Libgit2Exception>(() => repo.Repo.ReadNote("refs/notes/test", repo.CommitOid));
  }

  [Fact]
  public void CanCreateNoteCommitAndRemove()
  {
    using var repo = new RepoWithOneCommit();
    var (commitOid, noteOid) = repo.Repo.CreateNoteCommit(
      null, repo.Signature, repo.Signature, repo.CommitOid, "test note", true);
    using var commit = repo.Repo.LookupCommit(commitOid);
    using var note = repo.Repo.ReadNoteCommit(commit, repo.CommitOid);
    Assert.Equal("test note", note.Message);
    Assert.Equal(repo.Signature.Name, note.Author.Name);
    Assert.Equal(repo.Signature.Email, note.Author.Email);
    Assert.Equal(repo.Signature.Name, note.Committer.Name);
    Assert.Equal(repo.Signature.Email, note.Committer.Email);
    Assert.Equal(noteOid, note.Id);

    var idAfterRemove = repo.Repo.RemoveNoteCommit(commit, repo.Signature, repo.Signature, repo.CommitOid);
    using var newCommit = repo.Repo.LookupCommit(idAfterRemove);
    using var newNote = repo.Repo.ReadNoteCommit(commit, repo.CommitOid);
    Assert.Equal("test note", newNote.Message);
  }

  [Fact]
  public void CanIterateCommitNotes()
  {
    using var repo = new RepoWithOneCommit();
    var (commitOid, noteOid) = repo.Repo.CreateNoteCommit(
      null, repo.Signature, repo.Signature, repo.CommitOid, "test note", true);
    using var commit = repo.Repo.LookupCommit(commitOid);
    var notes = commit.IterateNotes().ToList();
    Assert.Single(notes);
    Assert.Equal(repo.CommitOid, notes[0].AnnotatedId);
    Assert.Equal("test note", repo.Repo.ReadNoteCommit(commit, repo.CommitOid).Message);
  }

  [Fact]
  public void CanIterateRepoNotes()
  {
    using var repo = new RepoWithOneCommit();
    var noteOid = repo.Repo.CreateNote(
      null, repo.Signature, repo.Signature, repo.CommitOid, "test note", true);
    bool callbackCalled = false;
    repo.Repo.ForeachNote(null, (GitOid blobId, GitOid annotatedOid) => { 
      Assert.False(callbackCalled);
      callbackCalled = true;
      Assert.Equal(repo.CommitOid, annotatedOid);
      Assert.Equal(noteOid, blobId);
      return GitOperationContinuation.Continue;
    });
    Assert.True(callbackCalled);
  }

  [Fact]
  public void CanGetDefaultNoteRef()
  {
    using var repo = new EmptyRepo();
    Assert.Equal("refs/notes/commits", repo.Repo.DefaultNoteRef);
  }
}
