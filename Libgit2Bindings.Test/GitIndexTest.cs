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
  public void CanSetAndReadIndexCapabilities()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    index.SetCapabilities(GitIndexCapability.IgnoreCase);
    Assert.Equal(GitIndexCapability.IgnoreCase, index.Capabilities);

    index.SetCapabilities(GitIndexCapability.NoFilemode);
    Assert.Equal(GitIndexCapability.NoFilemode, index.Capabilities);
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

  [Fact]
  public void CanGetIndexEntryStage()
  {
    var entry = new GitIndexEntry()
    {
      Path = "file.txt",
      CTime = new(),
      MTime = new(),
      Mode = 33188,
      Flags = 2 << 12,
      Id = new(Enumerable.Repeat((byte)0, 20).ToArray())
    };
    Assert.Equal(2, entry.GetStage());
    Assert.True(entry.IsConflict());
  }

  [Fact]
  public void CanGetIndexChecksum()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var fileFullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file.txt");
    File.WriteAllLines(fileFullPath, ["content"]);

    index.AddByPath("file.txt");
    index.Write();

    var checksum = index.GetChecksum();

    Assert.Contains(checksum.Id, b => b != 0);
  }

  [Fact]
  public void CanAddConflict()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var ancestor = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("ancestor"));
    var ours = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("ours"));
    var theirs = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("theirs"));

    var ancestorEntry = new GitIndexEntry()
    {
      Path = "file.txt",
      CTime = new(),
      MTime = new(),
      Mode = 33188,
      Id = ancestor
    };
    var ourEntry = new GitIndexEntry()
    {
      Path = "file.txt",
      CTime = new(),
      MTime = new(),
      Mode = 33188,
      Id = ours
    };
    var theirEntry = new GitIndexEntry()
    {
      Path = "file.txt",
      CTime = new(),
      MTime = new(),
      Mode = 33188,
      Id = theirs
    };

    index.AddConflict(ancestorEntry, ourEntry, theirEntry);
    Assert.Equal(3ul, index.EntryCount);

    Assert.True(index.HasConflicts());

    var entry = index.GetEntry(0);
    Assert.Equal("file.txt", entry.Path);
    Assert.Equal(ancestor, entry.Id);
    Assert.Equal(1, entry.GetStage());
  }

  [Fact]
  public void CanSetVersion()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    index.SetVersion(4u);
    Assert.Equal(4u, index.Version);
    index.SetVersion(3u);
    Assert.Equal(3u, index.Version);
  }

  [Fact]
  public void CanCleanupConflicts()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var ancestor = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("ancestor"));

    var ancestorEntry = new GitIndexEntry()
    {
      Path = "file.txt",
      CTime = new(),
      MTime = new(),
      Mode = 33188,
      Id = ancestor
    };

    index.AddConflict(ancestorEntry, null, null);
    Assert.Equal(1ul, index.EntryCount);

    index.CleanupConflicts();
    Assert.Equal(0ul, index.EntryCount);
  }

  [Fact]
  public void CanGetConflict()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var ancestor = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("ancestor"));
    var ours = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("ours"));
    var theirs = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("theirs"));

    var ancestorEntry = new GitIndexEntry()
    {
      Path = "file.txt",
      CTime = new(),
      MTime = new(),
      Mode = 33188,
      Id = ancestor
    };
    var ourEntry = new GitIndexEntry()
    {
      Path = "file.txt",
      CTime = new(),
      MTime = new(),
      Mode = 33188,
      Id = ours
    };
    var theirEntry = new GitIndexEntry()
    {
      Path = "file.txt",
      CTime = new(),
      MTime = new(),
      Mode = 33188,
      Id = theirs
    };

    index.AddConflict(ancestorEntry, ourEntry, theirEntry);
    Assert.Equal(3ul, index.EntryCount);

    var conflict = index.GetConflict("file.txt");
    Assert.Equal(ancestor, conflict.Ancestor?.Id);
    Assert.Equal(ours, conflict.Our?.Id);
    Assert.Equal(theirs, conflict.Their?.Id);
  }

  [Fact]
  public void CanRemoveConflict()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var ancestor = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("ancestor"));
    var ours = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("ours"));
    var theirs = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("theirs"));

    var ancestorEntry = new GitIndexEntry()
    {
      Path = "file.txt",
      CTime = new(),
      MTime = new(),
      Mode = 33188,
      Id = ancestor
    };
    var ourEntry = new GitIndexEntry()
    {
      Path = "file.txt",
      CTime = new(),
      MTime = new(),
      Mode = 33188,
      Id = ours
    };
    var theirEntry = new GitIndexEntry()
    {
      Path = "file.txt",
      CTime = new(),
      MTime = new(),
      Mode = 33188,
      Id = theirs
    };

    index.AddConflict(ancestorEntry, ourEntry, theirEntry);
    Assert.Equal(3ul, index.EntryCount);

    index.RemoveConflict("file.txt");
    Assert.Equal(0ul, index.EntryCount);
  }

  [Fact]
  public void CanFindIndexEntryIndex()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var fileFullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file.txt");
    File.WriteAllLines(fileFullPath, ["content"]);

    index.AddByPath("file.txt");

    var pos = index.FindEntryIndex("file.txt");
    Assert.Equal(0ul, pos);
  }

  [Fact]
  public void CanFindIndexEntryIndexByPrefix()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var fileFullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file.txt");
    File.WriteAllLines(fileFullPath, ["content"]);

    index.AddByPath("file.txt");

    var pos = index.FindEntryIndexByPrefix("f");
    Assert.Equal(0ul, pos);
  }

  [Fact]
  public void CanGetIndexEntryByPath()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var fileFullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file.txt");
    File.WriteAllLines(fileFullPath, ["content"]);

    index.AddByPath("file.txt");

    var entry = index.GetEntryByPath("file.txt", 0);
    Assert.Equal("file.txt", entry?.Path);
  }
}
