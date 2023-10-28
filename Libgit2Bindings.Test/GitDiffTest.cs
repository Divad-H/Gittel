using Libgit2Bindings.Test.TestData;

namespace Libgit2Bindings.Test;

public class GitDiffTest
{
  [Fact]
  public void CanDiffTreeToWorkdir()
  {
    using var sourceRepo = new RepoWithOneCommit();

    var fileFullPath = Path.Combine(sourceRepo.TempDirectory.DirectoryPath, RepoWithOneCommit.Filename);
    File.WriteAllLines(fileFullPath, new[] { "some modified content" });

    using var diff = sourceRepo.Repo.DiffTreeToWorkdir(sourceRepo.Tree);

    Assert.NotNull(diff);
    Assert.Equal(1ul, diff.GetNumDeltas());
    var delta = diff.GetDelta(0);
    Assert.NotNull(delta);
    Assert.Equal(RepoWithOneCommit.Filename, delta.NewFile?.Path);
    Assert.Equal(RepoWithOneCommit.Filename, delta.OldFile?.Path);
    Assert.Equal(GitDeltaType.Modified, delta.Status);
  }
}
