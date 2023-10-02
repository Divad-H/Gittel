using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal class Libgit2 : ILibgit2, IDisposable
{
  public Libgit2()
  {
    libgit2.global.GitLibgit2Init();
  }

  public IGitRepository OpenRepository(string path)
  {
    var res = libgit2.repository.GitRepositoryOpen(out var repo, path);
    CheckLibgit2.Check(res, "Unable to open repository '{0}'", path);
    return new GitRepository(repo);
  }

  public IGitRepository InitRepository(string path, bool isBare)
  {
    var res = libgit2.repository.GitRepositoryInit(out var repo, path, isBare ? 1u : 0);
    CheckLibgit2.Check(res, "Unable to initialize repository '{0}'", path);
    return new GitRepository(repo);
  }

  public string DiscoverRepository(string startPath, bool acrossFilesystem, string[] ceilingDirectories)
  {
    var res = libgit2.repository.GitRepositoryDiscover(
      out var repoPath, 
      startPath, 
      acrossFilesystem ? 1 : 0, 
      ceilingDirectories.Any() ? string.Join((char)libgit2.PathListSeparator.GIT_PATH_LIST_SEPARATOR, ceilingDirectories) : null);

    using (repoPath.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to discover repository '{0}'", startPath);
      return StringUtil.ToString(repoPath);
    }
  }

  public IGitSignature CreateGitSignature(string signature)
  {
    var res = libgit2.signature.GitSignatureFromBuffer(out var nativeSignature, signature);
    CheckLibgit2.Check(res, "Unable to create signature");
    return new GitSignature(nativeSignature);
  }

  public IGitSignature CreateGitSignature(string name, string email, DateTimeOffset when)
  {
    var res = libgit2.signature.GitSignatureNew(
      out var signature, name, email, when.ToUnixTimeSeconds(), (int)when.Offset.TotalMinutes);
    CheckLibgit2.Check(res, "Unable to create signature");
    return new GitSignature(signature);
  }

  public string FindGlobalConfig()
  {
    var res = libgit2.config.GitConfigFindGlobal(out var path);
    using (path.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to find global config");
      return StringUtil.ToString(path);
    }
  }

  public string FindSystemConfig()
  {
    var res = libgit2.config.GitConfigFindSystem(out var path);
    using (path.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to find system config");
      return StringUtil.ToString(path);
    }
  }

  public string FindProgramdataConfig()
  {
    var res = libgit2.config.GitConfigFindProgramdata(out var path);
    using (path.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to find programdata config");
      return StringUtil.ToString(path);
    }
  }

  public string FindXdgConfig()
  {
    var res = libgit2.config.GitConfigFindXdg(out var path);
    using (path.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to find xdg config");
      return StringUtil.ToString(path);
    }
  }

  public IGitConfig NewConfig()
  {
    var res = libgit2.config.GitConfigNew(out var config);
    CheckLibgit2.Check(res, "Unable to create config");
    return new GitConfig(config);
  }

  public IGitConfig OpenConfigOndisk(string path)
  {
    var res = libgit2.config.GitConfigOpenOndisk(out var config, path);
    CheckLibgit2.Check(res, "Unable to open config");
    return new GitConfig(config);
  }

  public bool ParseConfigBool(string value)
  {
    var res = libgit2.config.GitConfigParseBool(out var result, value);
    CheckLibgit2.Check(res, "Unable to parse config value");
    return result != 0;
  }

  public int ParseConfigInt32(string value)
  {
    var res = libgit2.config.GitConfigParseInt32(out var result, value);
    CheckLibgit2.Check(res, "Unable to parse config value");
    return result;
  }

  public long ParseConfigInt64(string value)
  {
    var res = libgit2.config.GitConfigParseInt64(out var result, value);
    CheckLibgit2.Check(res, "Unable to parse config value");
    return result;
  }

  public string ParseConfigPath(string value)
  {
    var res = libgit2.config.GitConfigParsePath(out var result, value);
    using (result.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to parse config value");
      return StringUtil.ToString(result);
    }
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.global.GitLibgit2Shutdown();
      _disposedValue = true;
    }
  }

  ~Libgit2()
  {
    Dispose(disposing: false);
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
  #endregion
}
