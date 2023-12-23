﻿using Libgit2Bindings.Test.TestData;
using System.Text;

namespace Libgit2Bindings.Test;

public class GitDiffTest
{
  [Fact]
  public void CanDiffTreeToWorkdir()
  {
    using var sourceRepo = new RepoWithOneCommit();

    var fileFullPath = Path.Combine(sourceRepo.TempDirectory.DirectoryPath, RepoWithOneCommit.Filename);
    File.WriteAllLines(fileFullPath, ["some modified content"]);

    using var diff = sourceRepo.Repo.DiffTreeToWorkdir(sourceRepo.Tree);

    Assert.NotNull(diff);
    Assert.Equal(1ul, diff.GetNumDeltas());
    var delta = diff.GetDelta(0);
    Assert.NotNull(delta);
    Assert.Equal(RepoWithOneCommit.Filename, delta.NewFile?.Path);
    Assert.Equal(RepoWithOneCommit.Filename, delta.OldFile?.Path);
    Assert.Equal(GitDeltaType.Modified, delta.Status);
  }

  [Fact]
  public void CanDiffBlobToBuffer()
  {
    using var repo = new EmptyRepo();

    const string contentA = @"First line
Second line
Third line
";

    const string contentB = @"First line
Second line is modified
Third line
";

    var blobOid = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes(contentA));
    using var blob = repo.Repo.LookupBlob(blobOid);
    var buffer = Encoding.UTF8.GetBytes(contentB);

    int count = 0;
    bool found = false;

    repo.Libgit2.DiffBlobToBuffer(blob, null, buffer, null,
      lineCallback: (diffDelta, diffHunk, diffLine) => {
        var content = Encoding.UTF8.GetString(diffLine.Content);
        ++count;
        found |= content.StartsWith("Second line is modified");
        return GitOperationContinuation.Continue;
      });

    Assert.Equal(4, count);
    Assert.True(found);
  }

  [Fact]
  public void CanDiffBlobs()
  {
    using var repo = new EmptyRepo();

    const string contentA = @"First line
Second line
Third line
";

    const string contentB = @"First line
Second line is modified
Third line
";

    var blobOidA = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes(contentA));
    var blobOidB = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes(contentB));
    using var blobA = repo.Repo.LookupBlob(blobOidA);
    using var blobB = repo.Repo.LookupBlob(blobOidB);

    int count = 0;
    bool found = false;

    repo.Libgit2.DiffBlobs(blobA, null, blobB, null,
      lineCallback: (diffDelta, diffHunk, diffLine) =>
      {
        var content = Encoding.UTF8.GetString(diffLine.Content);
        ++count;
        found |= content.StartsWith("Second line is modified");
        return GitOperationContinuation.Continue;
      });

    Assert.Equal(4, count);
    Assert.True(found);
  }

  [Fact]
  public void CanDiffBuffers()
  {
    using var repo = new EmptyRepo();

    const string contentA = @"First line
Second line
Third line
";

    const string contentB = @"First line
Second line is modified
Third line
";

    var bufferA = Encoding.UTF8.GetBytes(contentA);
    var bufferB = Encoding.UTF8.GetBytes(contentB);

    int count = 0;
    bool found = false;

    repo.Libgit2.DiffBuffers(bufferA, null, bufferB, null,
      lineCallback: (diffDelta, diffHunk, diffLine) =>
      {
        var content = Encoding.UTF8.GetString(diffLine.Content);
        ++count;
        found |= content.StartsWith("Second line is modified");
        return GitOperationContinuation.Continue;
      });

    Assert.Equal(4, count);
    Assert.True(found);
  }

  [Fact]
  public void CanUpdateDiffByFindSimilar()
  {
    using var sourceRepo = new RepoWithOneCommit();

    const string newFileName = "other.txt";
    var fileFullPath = Path.Combine(sourceRepo.TempDirectory.DirectoryPath, RepoWithOneCommit.Filename);
    var otherFileFFullPath = Path.Combine(sourceRepo.TempDirectory.DirectoryPath, newFileName);
    File.WriteAllLines(otherFileFFullPath, ["my content", "and some more"]);
    File.Delete(fileFullPath);

    using var diff = sourceRepo.Repo.DiffTreeToWorkdir(sourceRepo.Tree, new()
    {
      Flags = GitDiffOptionFlags.IncludeUntracked,
    });

    Assert.NotNull(diff);
    Assert.Equal(2ul, diff.GetNumDeltas());
    for (int i = 0; i < 2; ++i)
    {
      var delta = diff.GetDelta((uint)i);
      Assert.NotNull(delta);
      if (RepoWithOneCommit.Filename == delta.NewFile?.Path)
      {
        Assert.Equal(GitDeltaType.Deleted, delta.Status);
        Assert.Equal(RepoWithOneCommit.Filename, delta.OldFile?.Path);
      }
      else
      {
        Assert.Equal(GitDeltaType.Untracked, delta.Status);
        Assert.Equal(newFileName, delta.NewFile?.Path);
      }
    }

    diff.FindSimilar(new()
    {
      Flags = GitDiffFindFlags.ForUntracked
    });
    
    Assert.Equal(1ul, diff.GetNumDeltas());
    var newDelta = diff.GetDelta(0);
    Assert.NotNull(newDelta);
    Assert.Equal(GitDeltaType.Renamed, newDelta.Status);
    Assert.Equal(RepoWithOneCommit.Filename, newDelta.OldFile?.Path);
    Assert.Equal(newFileName, newDelta.NewFile?.Path);
  }
}
