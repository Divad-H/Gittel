using Libgit2Bindings.Test.Helpers;

namespace Libgit2Bindings.Test;

public class GitConfigTest
{
  const string FileNotFoundErrorCode = "[-3]";

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
      if (ex.Message.Contains(FileNotFoundErrorCode))
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
      if (ex.Message.Contains(FileNotFoundErrorCode))
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
      if (ex.Message.Contains(FileNotFoundErrorCode))
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
      if (ex.Message.Contains(FileNotFoundErrorCode))
      {
        return;
      }
      throw;
    }
  }

  [Fact]
  public void GanSetAndGetBoolSetting()
  {
    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();
    using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

    using var config = repo.GetConfig();

    config.SetBool("test.bool.setting", true);
    var value = config.GetBool("test.bool.setting");

    Assert.True(value);
  }

  [Fact]
  public void GanSetAndGetInt32Setting()
  {
    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();
    using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

    using var config = repo.GetConfig();

    config.SetInt32("test.int32.setting", 8472);
    var value = config.GetInt32("test.int32.setting");

    Assert.Equal(8472, value);
  }

  [Fact]
  public void GanSetAndGetInt64Setting()
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
  public void GanSetAndGetMultiVarSetting()
  {
    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();
    using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

    using var config = repo.GetConfig();

    config.SetMultiVar("test.multi-var", "foo bar baz$", "foo bar baz");
    config.SetMultiVar("test.multi-var", "some other value$", "some other value");
    
    var entries = config.GetMultiVarEntries("test.multi-var", null);

    Assert.Equal(2, entries.Count);
    Assert.Equal("foo bar baz", entries.First().Value);
    Assert.Equal("some other value", entries.Last().Value);

    config.SetMultiVar("test.multi-var", "foo bar baz$", "baz bar foo");

    entries = config.GetMultiVarEntries("test.multi-var", "foo$");

    Assert.Single(entries);
    Assert.Equal("baz bar foo", entries.First().Value);
  }

  [Fact]
  public void GanSetAndGetStringSettingAndCreateSnapshot()
  {
    const string stringValue = "foo-bar";

    using var libgit2 = new Libgit2();

    using var tempDirectory = new TemporaryDirectory();
    using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

    using var config = repo.GetConfig();

    config.SetString("test.string.setting", stringValue);

    using var snapshot = config.Snapshot();

    var value = snapshot.GetString("test.string.setting");

    Assert.Equal(stringValue, value);
  }
}
