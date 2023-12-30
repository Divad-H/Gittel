using Libgit2Bindings.Test.TestData;

namespace Libgit2Bindings.Test;

public sealed class GitIndexTest
{
  [Fact]
  public void CanAddAllFilesUsingCallback()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var file1FullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file1.txt");
    File.WriteAllLines(file1FullPath, ["content"]);

    var file2FullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file2.txt");
    File.WriteAllLines(file2FullPath, ["content"]);

    index.AddAll(new[] { "*.txt" }, GitIndexAddOption.Default, (path, matched) =>
    {
      Assert.Equal("*.txt", matched);
      return path == "file1.txt" ? GitAddToIndexOperation.Add : GitAddToIndexOperation.Skip;
    });

    Assert.Equal(1ul, index.EntryCount);
  }

  [Fact]
  public void CanReadIndexCapabilities()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    Assert.True((int)index.Capabilities < 8);
  }

  [Fact]
  public void CanClearIndex()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var fileFullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file.txt");
    File.WriteAllLines(fileFullPath, ["content"]);

    index.AddByPath("file.txt");

    Assert.Equal(1ul, index.EntryCount);

    index.Clear();

    Assert.Equal(0ul, index.EntryCount);
  }
}
