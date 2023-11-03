using Libgit2Bindings.Test.TestData;
using System.Text;

namespace Libgit2Bindings.Test;

public class GitBlameTest
{
  [Fact]
  public void CanBlameFile()
  {
    using var repo = new RepoWithOneCommit();

    using var blame = repo.Repo.BlameFile(RepoWithOneCommit.Filename);
    Assert.NotNull(blame);
    Assert.Equal(1u, blame.GetHunkCount());

    using var hunk = blame.GetHunkByIndex(0);
    Assert.NotNull(hunk);
    Assert.Equal(1ul, hunk.LinesInHunk);
    Assert.Equal(repo.CommitOid, hunk.FinalCommitId);
    Assert.Equal(1u, hunk.FinalStartLineNumber);
    Assert.NotNull(hunk.FinalSignature);
    Assert.Equal(repo.CommitOid, hunk.OriginalCommitId);
    Assert.Equal(RepoWithOneCommit.Filename, hunk.OriginalPath);
    Assert.Equal(1u, hunk.OriginalStartLineNumber);
    Assert.NotNull(hunk.OriginalSignature);
    Assert.True(hunk.Boundary);
  }

  [Fact]
  public void CanBlameBuffer()
  {
    using var repo = new RepoWithOneCommit();
    using var blame = repo.Repo.BlameFile(RepoWithOneCommit.Filename);
    var buffer = Encoding.ASCII.GetBytes("my content");
    using var bufferBlame = blame.BlameBuffer(buffer);
    
    Assert.NotNull(bufferBlame);
    Assert.Equal(1u, bufferBlame.GetHunkCount());

    using var hunk = bufferBlame.GetHunkByIndex(0);
    Assert.NotNull(hunk);
    Assert.Equal(1ul, hunk.LinesInHunk);
    Assert.Equal(new GitOid(Enumerable.Repeat((byte)0, 20).ToArray()), hunk.FinalCommitId);
  }

  [Fact]
  public void CanGetHunkByLineNumber()
  {
    using var repo = new RepoWithOneCommit();

    using var blame = repo.Repo.BlameFile(RepoWithOneCommit.Filename);

    using var hunk = blame.GetHunkByLine(1);

    Assert.NotNull(hunk);
    Assert.Equal(1ul, hunk.LinesInHunk);
  }
}
