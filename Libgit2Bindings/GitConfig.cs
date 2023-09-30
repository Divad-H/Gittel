using Libgit2Bindings.Callbacks;

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

  public IReadOnlyCollection<GitConfigEntry> GetMultiVarEntries(string name, string? regexp)
  {
    using MultiVarForeachCallback callback = new();
    var res = libgit2.config.GitConfigGetMultivarForeach(
      _nativeGitConfig, name, regexp, MultiVarForeachCallback.GitConfigForeachCb, callback.Payload);
    CheckLibgit2.Check(res, "Unable to get config values for {0}", name);
    return callback.GetEntries();
  }

  public void SetString(string name, string value)
  {
    var res = libgit2.config.GitConfigSetString(_nativeGitConfig, name, value);
    CheckLibgit2.Check(res, "Unable to set config value for {0}", name);
  }

  public string GetString(string name)
  {
    var res = libgit2.config.GitConfigGetString(out var value, _nativeGitConfig, name);
    CheckLibgit2.Check(res, "Unable to get config value for {0}", name);
    return value;
  }

  public IGitConfig Snapshot()
  {
    var res = libgit2.config.GitConfigSnapshot(out var snapshot, _nativeGitConfig);
    CheckLibgit2.Check(res, "Unable to create snapshot");
    return new GitConfig(snapshot);
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
