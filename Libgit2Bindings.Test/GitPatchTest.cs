using Libgit2Bindings.Test.TestData;
using System.Text;

namespace Libgit2Bindings.Test;

public sealed class GitPatchTest
{


  private void RunTest(Func<EmptyRepo, string, string, IGitPatch> createPatch)
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

    using var patch = createPatch(repo, contentA, contentB);

    Assert.NotNull(patch.Delta.OldFile);
    Assert.Equal("test.txt", patch.Delta.OldFile.Path);

    var content = Encoding.UTF8.GetString(patch.GetContent());
    Assert.Contains("-Second line", content);
    Assert.Contains("+Second line is modified", content);

    var rawSize = patch.RawSize(true, true, true);
    Assert.True(rawSize > 10);

    var numHunks = patch.NumHunks;
    Assert.Equal((UIntPtr)1, numHunks);

    var numLinesInHunk = patch.GetNumLinesInHunk((UIntPtr)0);
    Assert.Equal(2, numLinesInHunk);

    var line = patch.GetLineInHunk((UIntPtr)0, (UIntPtr)0);
    var lineContent = Encoding.UTF8.GetString(line.Content);
    Assert.StartsWith("Second line", lineContent);

    var hunk = patch.GetHunk((UIntPtr)0, out var linesInHunk);
    Assert.Equal((UIntPtr)2, linesInHunk);
    Assert.Equal(2, hunk.NewStart);

    var (contextLines, addedLines, deletedLines) = patch.GetLineStats();
    Assert.Equal((UIntPtr)0, contextLines);
    Assert.Equal((UIntPtr)1, addedLines);
    Assert.Equal((UIntPtr)1, deletedLines);

    int count = 0;
    bool found = false;
    patch.Print((delta, hunk, line) =>
    {
      var content = Encoding.UTF8.GetString(line.Content);
      ++count;
      found |= content.StartsWith("Second line is modified");
      return GitOperationContinuation.Continue;
    });

    Assert.Equal(4, count);
    Assert.True(found);
  }

  [Fact]
  public void CanGetPatchFromBlobAndBuffer()
  {
    RunTest((repo, contentA, contentB) =>
    {
      var blobOid = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes(contentA));
      using var blob = repo.Repo.LookupBlob(blobOid);
      var buffer = Encoding.UTF8.GetBytes(contentB);

      return repo.Libgit2.PatchFromBlobAndBuffer(blob, "test.txt", buffer, null, new()
      {
        ContextLines = 0,
      });
    });
  }

  [Fact]
  public void CanGetPatchFromBlobs()
  {
    RunTest((repo, contentA, contentB) =>
    {
      var blobOid = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes(contentA));
      using var blob = repo.Repo.LookupBlob(blobOid);
      var newBlobOid = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes(contentB));
      using var newBlob = repo.Repo.LookupBlob(newBlobOid);

      return repo.Libgit2.PatchFromBlobs(blob, "test.txt", newBlob, null, new()
      {
        ContextLines = 0,
      });
    });
  }

  [Fact]
  public void CanGetPatchFromBuffers()
  {
    RunTest((repo, contentA, contentB) =>
    {
      var oldBuffer = Encoding.UTF8.GetBytes(contentA);
      var newBuffer = Encoding.UTF8.GetBytes(contentB);

      return repo.Libgit2.PatchFromBuffers(oldBuffer, "test.txt", newBuffer, null, new()
      {
        ContextLines = 0,
      });
    });
  }

  static IGitDiff CreateDiff(ILibgit2 libgit2)
  {
    var patch =
      "diff --git a/test.txt b/test.txt" + Environment.NewLine +
      "index 025d08b..9122a9c 100644" + Environment.NewLine +
      "--- a/test.txt" + Environment.NewLine +
      "+++ b/test.txt" + Environment.NewLine +
      "@@ -2 +2 @@" + Environment.NewLine +
      "-Second line" + Environment.NewLine +
      "+Second line is modified" + Environment.NewLine;
    return libgit2.DiffFromPatch(Encoding.UTF8.GetBytes(patch));
  }

  [Fact]
  public void CanGetPatchFromDiff()
  {
    RunTest((repo, contentA, contentB) =>
    {
      using var diff = CreateDiff(repo.Libgit2);

      Assert.Equal(1, (int)diff.GetNumDeltas());

      return diff.ToPatch(0) ?? throw new InvalidOperationException();
    });
  }
}
