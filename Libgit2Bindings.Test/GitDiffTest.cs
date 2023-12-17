using Libgit2Bindings.Test.TestData;
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
}
