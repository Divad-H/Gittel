namespace Libgit2Bindings;

internal class GitMailmap : IGitMailmap
{
  private readonly libgit2.GitMailmap _nativeGitMailmap;
  public libgit2.GitMailmap NativeGitMailmap => _nativeGitMailmap;

  public GitMailmap(libgit2.GitMailmap nativeGitMailmap)
  {
    _nativeGitMailmap = nativeGitMailmap;
  }

  public void AddEntry(string? realName, string? realEmail, string? replaceName, string replaceEmail)
  {
    var res = libgit2.mailmap.GitMailmapAddEntry(
           _nativeGitMailmap, realName, realEmail, replaceName, replaceEmail);
    CheckLibgit2.Check(res, "Unable to add entry to mailmap");
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.mailmap.GitMailmapFree(_nativeGitMailmap);
      _disposedValue = true;
    }
  }

  ~GitMailmap()
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
