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
    using var data = new EmptyRepoWithConflicts();

    Assert.Equal(3ul, data.Index.EntryCount);

    Assert.True(data.Index.HasConflicts());

    var entry = data.Index.GetEntry(0);
    Assert.Equal("file.txt", entry.Path);
    Assert.Equal(data.Ancestor, entry.Id);
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
    using var data = new EmptyRepoWithConflicts();

    Assert.Equal(3ul, data.Index.EntryCount);

    var conflict = data.Index.GetConflict("file.txt");
    Assert.Equal(data.Ancestor, conflict.Ancestor?.Id);
    Assert.Equal(data.Ours, conflict.Our?.Id);
    Assert.Equal(data.Theirs, conflict.Their?.Id);
  }

  [Fact]
  public void CanRemoveConflict()
  {
    using var data = new EmptyRepoWithConflicts();

    Assert.Equal(3ul, data.Index.EntryCount);

    data.Index.RemoveConflict("file.txt");
    Assert.Equal(0ul, data.Index.EntryCount);
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

  [Fact]
  public void CanGetIndexPath()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    Assert.EndsWith("index", index.Path);
  }

  [Fact]
  public void CanIterateConflicts()
  {
    using var data = new EmptyRepoWithConflicts();

    var conflicts = data.Index.GetAllConflicts().ToArray();

    Assert.Single(conflicts);

    Assert.Equal(data.Ancestor, conflicts[0].Ancestor?.Id);
    Assert.Equal(data.Ours, conflicts[0].Our?.Id);
    Assert.Equal(data.Theirs, conflicts[0].Their?.Id);
  }

  [Fact]
  public void CanIterateIndexEntries()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var file1FullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file1.txt");
    File.WriteAllLines(file1FullPath, ["content"]);

    var file2FullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file2.txt");
    File.WriteAllLines(file2FullPath, ["content"]);

    index.AddByPath("file1.txt");
    index.AddByPath("file2.txt");

    var entries = index.GetEntries().ToArray();

    Assert.Equal(2, entries.Length);

    Assert.Equal("file1.txt", entries[0].Path);
    Assert.Equal("file2.txt", entries[1].Path);
  }

  [Fact]
  public void CanReadIndex()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var fileFullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file.txt");
    File.WriteAllLines(fileFullPath, ["content"]);

    index.AddByPath("file.txt");
    index.Write();
    Assert.Equal(1ul, index.EntryCount);

    index.Clear();
    Assert.Equal(0ul, index.EntryCount);

    index.Read(true);
    Assert.Equal(1ul, index.EntryCount);
  }

  [Fact]
  public void CanReadTreeIntoIndex()
  {
    using var repo = new RepoWithTwoCommits();
    using var index = repo.Repo.GetIndex();

    var tree = repo.FirstTree;
    index.ReadTree(tree);
    Assert.Equal(1ul, index.EntryCount);
    var entry = index.GetEntry(0);
    Assert.Equal("test.txt", entry.Path);
  }

  [Fact]
  public void CanRemoveIndexEntry()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var fileFullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file.txt");
    File.WriteAllLines(fileFullPath, ["content"]);

    index.AddByPath("file.txt");
    Assert.Equal(1ul, index.EntryCount);

    index.Remove("file.txt", 0);
    Assert.Equal(0ul, index.EntryCount);
  }

  [Fact]
  public void CanRemoveIndexEntryByPath()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var fileFullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file.txt");
    File.WriteAllLines(fileFullPath, ["content"]);

    index.AddByPath("file.txt");
    Assert.Equal(1ul, index.EntryCount);

    index.RemoveByPath("file.txt");
    Assert.Equal(0ul, index.EntryCount);
  }

  [Fact]
  public void CanRemoveIndexEntriesByDirectory()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var fileFullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "dir", "file1.txt");
    Directory.CreateDirectory(Path.GetDirectoryName(fileFullPath)!);
    File.WriteAllLines(fileFullPath, ["content"]);

    index.AddByPath("dir/file1.txt");
    Assert.Equal(1ul, index.EntryCount);

    index.RemoveDirectory("dir", 1);
    Assert.Equal(1ul, index.EntryCount);

    index.RemoveDirectory("dir", 0);
    Assert.Equal(0ul, index.EntryCount);
  }

  [Fact]
  public void CanRemoveIndexEntriesByPathspec()
  {
    using var repo = new EmptyRepo();
    using var index = repo.Repo.GetIndex();

    var file1FullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file1.txt");
    File.WriteAllLines(file1FullPath, ["content"]);

    var file2FullPath = Path.Combine(repo.TempDirectory.DirectoryPath, "file2.txt");
    File.WriteAllLines(file2FullPath, ["content"]);

    index.AddByPath("file1.txt");
    index.AddByPath("file2.txt");
    Assert.Equal(2ul, index.EntryCount);

    index.RemoveAll(["*.txt"]);
    Assert.Equal(0ul, index.EntryCount);
  }

  [Fact]
  public void CanUpdateIndexByPathspec()
  {
    using var repo = new RepoWithOneCommit();
    using var index = repo.Repo.GetIndex();

    var file1FullPath = Path.Combine(repo.TempDirectory.DirectoryPath, RepoWithOneCommit.Filename);
    File.WriteAllLines(file1FullPath, ["content"]);

    Assert.Equal(1ul, index.EntryCount);
    var entry = index.GetEntry(0);

    var id = entry.Id;

    index.UpdateAll(["*.txt"]);
    Assert.Equal(1ul, index.EntryCount);

    entry = index.GetEntry(0);
    Assert.NotEqual(id, entry.Id);
  }

  private sealed class EmptyRepoWithConflicts : IDisposable
  {
    public EmptyRepo Repo { get; } = new();
    public IGitIndex Index { get; }

    public GitOid Ancestor { get; }
    public GitOid Ours { get; }
    public GitOid Theirs { get; }

    public EmptyRepoWithConflicts()
    {
      Index = Repo.Repo.GetIndex();

      Ancestor = Repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("ancestor"));
      Ours = Repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("ours"));
      Theirs = Repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("theirs"));

      var ancestorEntry = new GitIndexEntry()
      {
        Path = "file.txt",
        CTime = new(),
        MTime = new(),
        Mode = 33188,
        Id = Ancestor
      };
      var ourEntry = new GitIndexEntry()
      {
        Path = "file.txt",
        CTime = new(),
        MTime = new(),
        Mode = 33188,
        Id = Ours
      };
      var theirEntry = new GitIndexEntry()
      {
        Path = "file.txt",
        CTime = new(),
        MTime = new(),
        Mode = 33188,
        Id = Theirs
      };

      Index.AddConflict(ancestorEntry, ourEntry, theirEntry);
    }

    public void Dispose()
    {
      Index.Dispose();
      Repo.Dispose();
    }
  }
}
