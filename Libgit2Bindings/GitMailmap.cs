using Libgit2Bindings.Util;

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

  public NameAndEmail Resolve(string name, string email)
  {
    var res = libgit2.mailmap.GitMailmapResolve(
      out var realName, out var realEmail, _nativeGitMailmap, name, email);
    CheckLibgit2.Check(res, "Unable to resolve mailmap");
    return new(realName, realEmail);
  }

  public IGitSignature ResolveSignature(IGitSignature signature)
  {
    using var managedSignature = GittelObjects.DowncastNonNull<GitSignature>(signature);
    var res = libgit2.mailmap.GitMailmapResolveSignature(
      out var realSignature, _nativeGitMailmap, managedSignature.NativeGitSignature);
    CheckLibgit2.Check(res, "Unable to resolve signature");
    return new GitSignature(realSignature);
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
