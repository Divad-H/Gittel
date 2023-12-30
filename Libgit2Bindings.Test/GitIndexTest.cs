using Libgit2Bindings.Test.TestData;
using System.Text;

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

  [Fact]
  public void CanGetIndexEntry()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var fileFullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file.txt");
    File.WriteAllLines(fileFullPath, ["content"]);

    index.AddByPath("file.txt");

    var entry = index.GetEntry(0);

    Assert.Equal("file.txt", entry?.Path);
  }

  [Fact]
  public void CanAddIndexEntry()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var fileFullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file.txt");
    File.WriteAllLines(fileFullPath, ["content"]);

    index.AddByPath("file.txt");
    var entry = index.GetEntry(0);
    index.Clear();
    Assert.Equal(0ul, index.EntryCount);

    index.Add(entry);

    Assert.Equal(1ul, index.EntryCount);
  }

  [Fact]
  public void CanAddIndexEntryFromBuffer()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    byte[] buffer = Encoding.UTF8.GetBytes("content");
    var entry = new GitIndexEntry()
    {
      Path = "file.txt",
      CTime = new(),
      MTime = new(),
      Mode = 33188,
      Id = new(Enumerable.Repeat((byte)0, 20).ToArray())
    };
    index.AddFromBuffer(entry, buffer);

    Assert.Equal(1ul, index.EntryCount);
    entry = index.GetEntry(0);

    Assert.Equal("file.txt", entry.Path);

    var blob = repo.Repo.LookupBlob(entry.Id);
    Assert.Equal("content", Encoding.UTF8.GetString(blob.RawContent()));
  }
}
