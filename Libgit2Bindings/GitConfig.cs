using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal class GitConfig : IGitConfig
{
  private readonly libgit2.GitConfig _nativeGitConfig;

  public GitConfig(libgit2.GitConfig nativeGitConfig)
  {
    _nativeGitConfig = nativeGitConfig;
  }

  public void SetBool(string name, bool value)
  {
    var res = libgit2.config.GitConfigSetBool(_nativeGitConfig, name, value ? 1 : 0);
    CheckLibgit2.Check(res, "Unable to set config value for {0}", name);
  }

  public bool GetBool(string name)
  {
    var res = libgit2.config.GitConfigGetBool(out var value, _nativeGitConfig, name);
    CheckLibgit2.Check(res, "Unable to get config value for {0}", name);
    return value != 0;
  }

  public void SetInt32(string name, int value)
  {
    var res = libgit2.config.GitConfigSetInt32(_nativeGitConfig, name, value);
    CheckLibgit2.Check(res, "Unable to set config value for {0}", name);
  }

  public int GetInt32(string name)
  {
    var res = libgit2.config.GitConfigGetInt32(out var value, _nativeGitConfig, name);
    CheckLibgit2.Check(res, "Unable to get config value for {0}", name);
    return value;
  }

  public void SetInt64(string name, Int64 value)
  {
    var res = libgit2.config.GitConfigSetInt64(_nativeGitConfig, name, value);
    CheckLibgit2.Check(res, "Unable to set config value for {0}", name);
  }

  public Int64 GetInt64(string name)
  {
    var res = libgit2.config.GitConfigGetInt64(out var value, _nativeGitConfig, name);
    CheckLibgit2.Check(res, "Unable to get config value for {0}", name);
    return value;
  }

  public void SetMultiVar(string name, string regexp, string value)
  {
    var res = libgit2.config.GitConfigSetMultivar(_nativeGitConfig, name, regexp, value);
    CheckLibgit2.Check(res, "Unable to set config value for {0}", name);
  }

  private static IEnumerable<GitConfigEntry> YieldEntries(libgit2.GitConfigIterator iterator)
  {
    while (true)
    {
      var res = libgit2.config.GitConfigNext(out var entry, iterator);
      using (entry)
      {
        if (res == (int)libgit2.GitErrorCode.GIT_ITEROVER)
        {
          yield break;
        }
        CheckLibgit2.Check(res, "Error while iterating config entries");
        yield return GitConfigEntryMapper.FromNative(entry);
      }
    }
  }

  public IEnumerable<GitConfigEntry> GetMultiVarEntries(string name, string? regexp)
  {
    var res = libgit2.config.GitConfigMultivarIteratorNew
      (out var iterator, _nativeGitConfig, name, regexp);
    CheckLibgit2.Check(res, "Unable to get config values for {0}", name);
    try
    {
      foreach (var entry in YieldEntries(iterator))
        yield return entry;
    }
    finally
    {
      libgit2.config.GitConfigIteratorFree(iterator);
    }
  }

  public void SetString(string name, string value)
  {
    var res = libgit2.config.GitConfigSetString(_nativeGitConfig, name, value);
    CheckLibgit2.Check(res, "Unable to set config value for {0}", name);
  }

  public string GetString(string name)
  {
    var res = libgit2.config.GitConfigGetStringBuf(out var value, _nativeGitConfig, name);
    using (value.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to get config value for {0}", name);
      return StringUtil.ToString(value);
    }
  }

  public string GetPath(string name)
  {
    var res = libgit2.config.GitConfigGetPath(out var value, _nativeGitConfig, name);
    using (value.GetDisposer()) 
    { 
      CheckLibgit2.Check(res, "Unable to get config value for {0}", name);
      return StringUtil.ToString(value); 
    }
  }

  public GitConfigEntry GetEntry(string name)
  {
    var res = libgit2.config.GitConfigGetEntry(out var entry, _nativeGitConfig, name);
    using (entry)
    {
      CheckLibgit2.Check(res, "Unable to get config value for {0}", name);
      return GitConfigEntryMapper.FromNative(entry);
    }
  }

  public void DeleteEntry(string name)
  {
    var res = libgit2.config.GitConfigDeleteEntry(_nativeGitConfig, name);
    CheckLibgit2.Check(res, "Unable to delete config value for {0}", name);
  }

  public void DeleteMultiVar(string name, string regexp)
  {
    var res = libgit2.config.GitConfigDeleteMultivar(_nativeGitConfig, name, regexp);
    CheckLibgit2.Check(res, "Unable to delete config value for {0}", name);
  }

  public IGitConfig Snapshot()
  {
    var res = libgit2.config.GitConfigSnapshot(out var snapshot, _nativeGitConfig);
    CheckLibgit2.Check(res, "Unable to create snapshot");
    return new GitConfig(snapshot);
  }

  public IGitTransaction Lock()
  {
    var res = libgit2.config.GitConfigLock(out var transaction, _nativeGitConfig);
    CheckLibgit2.Check(res, "Unable to lock config");
    return new GitTransaction(transaction);
  }

  public void AddFileOndisk(string path, GitConfigLevel level, IGitRepository? repository, bool force)
  {
    var repo = repository is null ? null : repository as GitRepository 
      ?? throw new ArgumentException($"{nameof(repository)} must be created by Gittel");
    var res = libgit2.config.GitConfigAddFileOndisk(
      _nativeGitConfig, path, GitConfigLevelMapper.ToNative(level), repo?.NativeGitRepository, force ? 1 : 0);
    CheckLibgit2.Check(res, "Unable to add config file {0}", path);
  }

  public IGitConfig OpenLevel(GitConfigLevel level)
  {
    var res = libgit2.config.GitConfigOpenLevel(
      out var config, _nativeGitConfig, GitConfigLevelMapper.ToNative(level));
    CheckLibgit2.Check(res, "Unable to open config level {0}", level);
    return new GitConfig(config);
  }

  public IEnumerable<GitConfigEntry> GetEntries(string? regexp = null)
  {
    using GitConfigForeachCallback callback = new();
    var res = libgit2.config.GitConfigIteratorGlobNew(
      out var iterator, _nativeGitConfig, regexp);
    CheckLibgit2.Check(res, "Unable to get config entries");
    try
    {
      foreach (var entry in YieldEntries(iterator))
        yield return entry;
    }
    finally
    {
      libgit2.config.GitConfigIteratorFree(iterator);
    }
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.config.GitConfigFree(_nativeGitConfig);
      _disposedValue = true;
    }
  }

  ~GitConfig()
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
