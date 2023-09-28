using Libgit2Bindings.Util;
using System.Runtime.InteropServices;

namespace Libgit2Bindings;

internal class GitSignature : IGitSignature
{
  private readonly libgit2.GitSignature _nativeGitSignature;
  public libgit2.GitSignature NativeGitSignature => _nativeGitSignature;

  public unsafe string Name => StringUtil.ToString(_nativeGitSignature.Name);

  public unsafe string Email => StringUtil.ToString(_nativeGitSignature.Email);

  public DateTimeOffset When => new(
    (DateTimeOffset.FromUnixTimeSeconds(_nativeGitSignature.When.Time)
      + TimeSpan.FromMinutes(_nativeGitSignature.When.Offset)).Ticks, 
    new(0, _nativeGitSignature.When.Offset, 0));

  public GitSignature(libgit2.GitSignature nativeGitSignature)
  {
    _nativeGitSignature = nativeGitSignature;
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.signature.GitSignatureFree(_nativeGitSignature);
      _disposedValue = true;
    }
  }

  ~GitSignature()
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
