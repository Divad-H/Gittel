using Libgit2Bindings.Test.TestData;
using Libgit2Bindings.Util;
using System.Text;

namespace Libgit2Bindings.Test;

public class GitDiffTest
{
  static IGitDiff CreateDiff(ILibgit2 libgit2)
  {
    var patch =
      "diff --git a/test.txt b/test.txt" + Environment.NewLine +
      "index 025d08b..9122a9c 100644" + Environment.NewLine +
      "--- a/test.txt" + Environment.NewLine +
      "+++ b/test.txt" + Environment.NewLine +
      "@@ -1 +1 @@" + Environment.NewLine +
      "-my content" + Environment.NewLine +
      "+some modified content" + Environment.NewLine;
    return libgit2.DiffFromPatch(Encoding.UTF8.GetBytes(patch));
  }


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

  [Fact]
  public void CanIterateDiff()
  {
    using var libgit2 = new Libgit2();
    using var diff = CreateDiff(libgit2);

    int fileCount = 0;
    bool fileFound = false;
    int hunkCount = 0;
    int lineCount = 0;

    diff.ForEach(
      fileCallback: (delta, progress) =>
      {
        ++fileCount;
        fileFound |= delta.NewFile?.Path == RepoWithOneCommit.Filename;
        return GitOperationContinuation.Continue;
      },
      hunkCallback: (delta, hunk) =>
      {
        ++hunkCount;
        return GitOperationContinuation.Continue;
      },
      lineCallback: (delta, hunk, line) =>
      {
        if (Encoding.UTF8.GetString(line.Content).StartsWith("some modified content"))
        {
          ++lineCount;
          Assert.Equal(GitDiffLine.Addition, line.Origin);
        }
        else if (Encoding.UTF8.GetString(line.Content).StartsWith("my content"))
        {
          ++lineCount;
          Assert.Equal(GitDiffLine.Deletion, line.Origin);
        }
        return GitOperationContinuation.Continue;
      }
      );

    Assert.Equal(1, fileCount);
    Assert.True(fileFound);
    Assert.Equal(1, hunkCount);
    Assert.Equal(2, lineCount);
  }

  [Fact]
  public void CanIterateBinaryDiff()
  {
    using var sourceRepo = new RepoWithOneCommit();

    var fileFullPath = Path.Combine(sourceRepo.TempDirectory.DirectoryPath, RepoWithOneCommit.Filename);
    File.WriteAllLines(fileFullPath, ["some modified content"]);

    using var diff = sourceRepo.Repo.DiffTreeToWorkdir(sourceRepo.Tree, new()
    {
      Flags = GitDiffOptionFlags.ForceBinary | GitDiffOptionFlags.ShowBinary,
    });

    Assert.NotNull(diff);

    int fileCount = 0;
    bool fileFound = false;
    int binaryCount = 0;

    diff.ForEach(
      fileCallback: (delta, progress) =>
      {
        ++fileCount;
        fileFound |= delta.NewFile?.Path == RepoWithOneCommit.Filename;
        return GitOperationContinuation.Continue;
      },
      binaryCallback: (delta, binary) =>
      {
        ++binaryCount;
        Assert.Equal(11ul, binary.OldFile.InflatedLength);
        Assert.Equal(22ul, binary.NewFile.InflatedLength);

        var inflatedData = binary.NewFile.Inflate();
        var content = Encoding.UTF8.GetString(inflatedData);
        Assert.StartsWith("some modified content", content);

        return GitOperationContinuation.Continue;
      }
    );

    Assert.Equal(1, fileCount);
    Assert.True(fileFound);
    Assert.Equal(1, binaryCount);
  }

  [Fact]
  public void CanGetGitDiffStats()
  {
    using var libgit2 = new Libgit2();
    using var diff = CreateDiff(libgit2);

    Assert.NotNull(diff);

    var stats = diff.GetStats();
    Assert.Equal(1ul, stats.FilesChanged);
    Assert.Equal(1ul, stats.Insertions);
    Assert.Equal(1ul, stats.Deletions);
  }

  [Fact]
  public void CanGetGitDiffStatsFormatted()
  {
    using var libgit2 = new Libgit2();
    using var diff = CreateDiff(libgit2);

    var stats = diff.GetStatsFormatted(GitDiffStatsFormatOptions.Short, 80);
    Assert.StartsWith(" 1 file changed, 1 insertion(+), 1 deletion(-)", stats);
  }

  [Fact]
  public void CanFormatDiff()
  {
    using var libgit2 = new Libgit2();
    using var diff = CreateDiff(libgit2);

    var buffer = diff.ToBuffer(GitDiffFormatOptions.Patch);
    var content = Encoding.UTF8.GetString(buffer);
    Assert.Contains("diff --git a/test.txt b/test.txt", content);
    Assert.Contains("index 025d08b..9122a9c 100644", content);
    Assert.Contains("--- a/test.txt", content);
    Assert.Contains("+++ b/test.txt", content);
    Assert.Contains("@@ -1 +1 @@", content);
    Assert.Contains("-my content", content);
    Assert.Contains("+some modified content", content);
  }

  [Fact]
  public void CanGetDiffFromPatch()
  {
    using var libgit2 = new Libgit2();
    using var diff = CreateDiff(libgit2);

    Assert.NotNull(diff);
    Assert.Equal(1ul, diff.GetNumDeltas());
    var delta = diff.GetDelta(0);
    Assert.NotNull(delta);
    Assert.Equal(RepoWithOneCommit.Filename, delta.NewFile?.Path);
    Assert.Equal(RepoWithOneCommit.Filename, delta.OldFile?.Path);
    Assert.Equal(GitDeltaType.Modified, delta.Status);
  }

  [Fact]
  public void CanCheckIfDiffIsSortedCaseInsensitively()
  {
    using var libgit2 = new Libgit2();
    using var diff = CreateDiff(libgit2);

    Assert.False(diff.IsSortedCaseInsensitively());
  }

  [Fact]
  public void CanDiffIndexToWorkdir()
  {
    using var sourceRepo = new RepoWithOneCommit();

    var fileFullPath = Path.Combine(sourceRepo.TempDirectory.DirectoryPath, RepoWithOneCommit.Filename);
    File.WriteAllLines(fileFullPath, ["some modified content"]);

    using var index = sourceRepo.Repo.GetIndex();

    using var diff = sourceRepo.Repo.DiffIndexToWorkdir(index);

    Assert.NotNull(diff);
    Assert.Equal(1ul, diff.GetNumDeltas());
    var delta = diff.GetDelta(0);
    Assert.NotNull(delta);
    Assert.Equal(RepoWithOneCommit.Filename, delta.NewFile?.Path);
    Assert.Equal(RepoWithOneCommit.Filename, delta.OldFile?.Path);
    Assert.Equal(GitDeltaType.Modified, delta.Status);
  }

  [Fact]
  public void CanDiffIndexToIndex()
  {
    // Use CherrypickCommit to create a second index object.
    using var repo = new RepoWithOneCommit();
    using var commit = repo.Repo.LookupCommit(repo.CommitOid);

    var fileFullPath = Path.Combine(repo.TempDirectory.DirectoryPath, RepoWithOneCommit.Filename);
    File.WriteAllLines(fileFullPath, ["my content", "more content"]);

    using var oldIndex = repo.Repo.GetIndex();

    oldIndex.AddByPath(RepoWithOneCommit.Filename);
    var treeOid = oldIndex.WriteTree();
    oldIndex.Write();

    using var tree = repo.Repo.LookupTree(treeOid);
    var commitOid = repo.Repo.CreateCommit(null, repo.Signature, repo.Signature, "msg", tree, [commit]);
    using var secondCommit = repo.Repo.LookupCommit(commitOid);

    repo.Repo.CheckoutHead(new() { Strategy = CheckoutStrategy.Force });

    using var newIndex = repo.Repo.CherrypickCommit(secondCommit, commit, 0);

    using var diff = repo.Repo.DiffIndexToIndex(oldIndex, newIndex);

    Assert.NotNull(diff);
    Assert.Equal(1ul, diff.GetNumDeltas());
    var delta = diff.GetDelta(0);
    Assert.NotNull(delta);
    Assert.Equal(RepoWithOneCommit.Filename, delta.NewFile?.Path);
    Assert.Equal(RepoWithOneCommit.Filename, delta.OldFile?.Path);
    Assert.Equal(GitDeltaType.Modified, delta.Status);
  }

  [Fact]
  public void CanDiffTreeToTree()
  {

    using var repo = new RepoWithOneCommit();
    using var commit = repo.Repo.LookupCommit(repo.CommitOid);

    var fileFullPath = Path.Combine(repo.TempDirectory.DirectoryPath, RepoWithOneCommit.Filename);
    File.WriteAllLines(fileFullPath, ["my content", "more content"]);

    using var index = repo.Repo.GetIndex();

    index.AddByPath(RepoWithOneCommit.Filename);
    var treeOid = index.WriteTree();

    using var newTree = repo.Repo.LookupTree(treeOid);
    
    using var diff = repo.Repo.DiffTreeToTree(repo.Tree, newTree);

    Assert.NotNull(diff);
    Assert.Equal(1ul, diff.GetNumDeltas());
    var delta = diff.GetDelta(0);
    Assert.NotNull(delta);
    Assert.Equal(RepoWithOneCommit.Filename, delta.NewFile?.Path);
    Assert.Equal(RepoWithOneCommit.Filename, delta.OldFile?.Path);
    Assert.Equal(GitDeltaType.Modified, delta.Status);
  }
}
