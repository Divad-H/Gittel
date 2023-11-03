using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal class GitSignature : IGitSignature
{
  private readonly libgit2.GitSignature _nativeGitSignature;
  public libgit2.GitSignature NativeGitSignature => _nativeGitSignature;
  private readonly bool _ownsNativeInstance;

  public unsafe string Name => StringUtil.ToString(_nativeGitSignature.Name);

  public unsafe string Email => StringUtil.ToString(_nativeGitSignature.Email);

  public DateTimeOffset When => FromEpochAndOffset(
    _nativeGitSignature.When.Time, _nativeGitSignature.When.Offset);

  public GitSignature(libgit2.GitSignature nativeGitSignature, bool ownsNativeInstance = true)
  {
    _nativeGitSignature = nativeGitSignature ?? throw new ArgumentNullException(nameof(nativeGitSignature));
    _ownsNativeInstance = ownsNativeInstance;
  }

  public static DateTimeOffset FromEpochAndOffset(long secondsSinceEpoch, int offsetMinutesFromUtc)
  {
    return new(
      (DateTimeOffset.FromUnixTimeSeconds(secondsSinceEpoch) + TimeSpan.FromMinutes(offsetMinutesFromUtc)).Ticks,
      new(0, offsetMinutesFromUtc, 0));
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (_ownsNativeInstance)
      {
        libgit2.signature.GitSignatureFree(_nativeGitSignature);
      }
      _nativeGitSignature.Dispose();
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
