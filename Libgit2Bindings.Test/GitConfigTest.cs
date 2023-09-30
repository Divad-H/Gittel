using Libgit2Bindings.Test.Helpers;
using System.Collections.Immutable;

namespace Libgit2Bindings.Test;

public class GitConfigTest
{
  const string NotFoundErrorCode = "[-3]";

  const string int32Entry = "test.int32.setting";
  const string boolEntry = "test.bool.setting";
  const string stringEntry = "test.string.setting";

  [Fact]
  public void CanFindGlobalGitConfig()
  {
    using var libgit2 = new Libgit2();

    try
    {
      var config = libgit2.FindGlobalConfig();

      Assert.NotNull(config);
      Assert.NotEmpty(config);
    }
    catch (Libgit2Exception ex)
    {
      if (ex.Message.Contains(NotFoundErrorCode))
      {
        return;
      }
      throw;
    }
  }

  [Fact]
  public void CanFindSystemGitConfig()
  {
    using var libgit2 = new Libgit2();

    try
    {
      var config = libgit2.FindSystemConfig();

      Assert.NotNull(config);
      Assert.NotEmpty(config);
    }
    catch (Libgit2Exception ex)
    {
      if (ex.Message.Contains(NotFoundErrorCode))
      {
        return;
      }
      throw;
    }
  }

  [Fact]
  public void CanFindProgramdataGitConfig()
  {
    using var libgit2 = new Libgit2();

    try
    {
      var config = libgit2.FindProgramdataConfig();

      Assert.NotNull(config);
      Assert.NotEmpty(config);
    }
    catch (Libgit2Exception ex)
    {
      if (ex.Message.Contains(NotFoundErrorCode))
      {
        return;
      }
      throw;
    }
  }

  [Fact]
  public void CanFindXdgaGitConfig()
  {
    using var libgit2 = new Libgit2();

    try
    {
      var config = libgit2.FindXdgConfig();

      Assert.NotNull(config);
      Assert.NotEmpty(config);
    }
    catch (Libgit2Exception ex)
    {
      if (ex.Message.Contains(NotFoundErrorCode))
      {
        return;
      }
      throw;
    }
  }

  [Fact]
  public void CanSetAndGeAndDeletetBoolSetting()
  {
    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();
    using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

    using var config = repo.GetConfig();

    config.SetBool(boolEntry, true);
    var value = config.GetBool(boolEntry);

    Assert.True(value);

    config.DeleteEntry(boolEntry);

    try
    {
      config.GetBool(boolEntry);
    }
    catch(Libgit2Exception ex)
    {
      if (ex.Message.Contains(NotFoundErrorCode))
      {
        return;
      }
      throw;
    }
  }

  [Fact]
  public void CanSetAndGetInt32Setting()
  {
    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();
    using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

    using var config = repo.GetConfig();

    config.SetInt32(int32Entry, 8472);
    var value = config.GetInt32(int32Entry);

    Assert.Equal(8472, value);
  }

  [Fact]
  public void CanSetAndGetInt64Setting()
  {
    const Int64 longValue = 4815162342;

    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();
    using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

    using var config = repo.GetConfig();

    config.SetInt64("test.int64.setting", longValue);
    var value = config.GetInt64("test.int64.setting");

    Assert.Equal(longValue, value);
  }

  [Fact]
  public void CanSetAndGetAndDeleteMultiVarSetting()
  {
    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();
    using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

    using var config = repo.GetConfig();

    config.SetMultiVar("test.multi-var", "foo bar baz$", "foo bar baz");
    config.SetMultiVar("test.multi-var", "some other value$", "some other value");
    
    var entries = config.GetMultiVarEntries("test.multi-var", null).ToImmutableArray();

    Assert.Equal(2, entries.Count());
    Assert.Equal("foo bar baz", entries.First().Value);
    Assert.Equal("some other value", entries.Last().Value);

    config.SetMultiVar("test.multi-var", "foo bar baz$", "baz bar foo");

    entries = config.GetMultiVarEntries("test.multi-var", "foo$").ToImmutableArray();

    Assert.Single(entries);
    Assert.Equal("baz bar foo", entries.First().Value);

    config.DeleteMultiVar("test.multi-var", "foo$");

    entries = config.GetMultiVarEntries("test.multi-var", null).ToImmutableArray();

    Assert.Single(entries);
  }

  [Fact]
  public void CanSetAndGetStringSetting()
  {
    const string stringValue = "foo-bar";

    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();
    using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

    using var config = repo.GetConfig();

    config.SetString(stringEntry, stringValue);
    var value = config.GetString(stringEntry);

    Assert.Equal(stringValue, value);
  }

  [Fact]
  public void CanGetEntry()
  {
    const string stringValue = "foo-bar";

    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();
    using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

    using var config = repo.GetConfig();

    config.SetString(stringEntry, stringValue);
    var entry = config.GetEntry(stringEntry);

    Assert.Equal(stringValue, entry.Value);
  }

  [Fact]
  public void CanGetPathSetting()
  {
    const string pathValue = @"C:\foo\bar";

    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();
    using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

    using var config = repo.GetConfig();

    config.SetString("test.path.setting", pathValue);
    var value = config.GetPath("test.path.setting");

    Assert.Equal(pathValue, value);
  }

  [Fact]
  public void CanCreateSnapshot()
  {
    using var libgit2 = new Libgit2();
    using var tempDirectory = new TemporaryDirectory();

    var configPath = Path.Combine(tempDirectory.DirectoryPath, "test.config");
    var config = libgit2.OpenConfigOndisk(configPath);

    config.SetInt32(int32Entry, 8472);

    using var snapshot = config.Snapshot();

    config.SetInt32(int32Entry, 42);

    var value = snapshot.GetInt32(int32Entry);

    Assert.Equal(8472, value);
  }

  [Fact]
  public void CanLockConfig()
  {
    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();
    using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

    using var config = repo.GetConfig();

    config.SetInt32(int32Entry, 8472);

    using (var transaction = config.Lock())
    {
      config.SetInt32(int32Entry, 42);

      using var otherConfig = repo.GetConfig();

      var value = otherConfig.GetInt32(int32Entry);

      Assert.Equal(8472, value);

      transaction.Commit();
    }

    using var finalConfig = repo.GetConfig();

    var finalValue = finalConfig.GetInt32(int32Entry);

    Assert.Equal(42, finalValue);
  }

  [Fact]
  public void CanCreateNewConfigAndAttachAFileToIt()
  {
    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();

    using var config = libgit2.NewConfig();

    var configPath = Path.Combine(tempDirectory.DirectoryPath, "test.config");
    config.AddFileOndisk(configPath, GitConfigLevel.Highest, null, false);

    config.SetInt32(int32Entry, 8472);

    Assert.True(File.Exists(configPath));
  }

  [Fact]
  public void CanOpenSpecificConfigLevel()
  {
    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();
    
    using var config = libgit2.NewConfig();

    var globalConfigPath = Path.Combine(tempDirectory.DirectoryPath, "global.config");
    config.AddFileOndisk(globalConfigPath, GitConfigLevel.Global, null, false);

    config.SetInt32(int32Entry, 8472);

    var configPath = Path.Combine(tempDirectory.DirectoryPath, "local.config");
    config.AddFileOndisk(configPath, GitConfigLevel.Local, null, false);

    config.SetInt32(int32Entry, 42);

    using var globalLevelConfig = config.OpenLevel(GitConfigLevel.Global);

    var value = globalLevelConfig.GetInt32(int32Entry);

    Assert.Equal(8472, value);
  }

  [Fact]
  public void CanOpenConfigFile()
  {
    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();
    using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

    var configPath = Path.Combine(tempDirectory.DirectoryPath, ".git/config");
    using var config = libgit2.OpenConfigOndisk(configPath);

    var value = config.GetBool("core.bare");

    Assert.False(value);
  }

  [Fact]
  public void CanParseConfigBool()
  {
    using var libgit2 = new Libgit2();
    var value = libgit2.ParseConfigBool("yes");
    Assert.True(value);
  }

  [Fact]
  public void CanParseConfigInt32()
  {
    using var libgit2 = new Libgit2();
    var value = libgit2.ParseConfigInt32("9k");
    Assert.Equal(9 * 1024, value);
  }


  [Fact]
  public void CanParseConfigInt64()
  {
    using var libgit2 = new Libgit2();
    var value = libgit2.ParseConfigInt64("9k");
    Assert.Equal(9 * 1024, value);
  }


  [Fact]
  public void CanParseConfigPath()
  {
    using var libgit2 = new Libgit2();
    var value = libgit2.ParseConfigPath("~/foo");
    Assert.EndsWith("foo", value);
  }

  [Fact]
  public void CanIterateConfigEntries()
  {
    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();

    var configPath = Path.Combine(tempDirectory.DirectoryPath, "config");
    using var config = libgit2.OpenConfigOndisk(configPath);

    config.SetInt32(int32Entry, 8472);
    config.SetBool(boolEntry, true);
    config.SetString(stringEntry, "foo-bar");

    var entries = config.GetEntries();

    Assert.Equal(3, entries.Count());
  }
}
