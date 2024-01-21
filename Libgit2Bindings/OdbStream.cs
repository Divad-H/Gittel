using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal sealed class OdbStream(libgit2.GitOdbStream nativeGitOdbStream) : IOdbStream
{
  public libgit2.GitOdbStream NativeGitOdbStream { get; } = nativeGitOdbStream;

  public void Write(byte[] buffer, int length)
  {
    using var pinnedData = new PinnedBuffer(buffer);
    var res = libgit2.odb.GitOdbStreamWrite(
      NativeGitOdbStream, pinnedData.Pointer, (UIntPtr)Math.Min(length, pinnedData.Length));
    CheckLibgit2.Check(res, "Unable to write to ODB stream");
  }

  public GitOid FinalizeWrite()
  {
    var res = libgit2.odb.GitOdbStreamFinalizeWrite(out var nativeOid, NativeGitOdbStream);
    CheckLibgit2.Check(res, "Unable to finalize write to ODB stream");
    using (nativeOid)
    {
      return GitOidMapper.FromNative(nativeOid);
    }
  }

  public unsafe void Read(byte[] buffer, int length)
  {
    using var pinnedBuffer = new PinnedBuffer(buffer);
    var res = libgit2.odb.GitOdbStreamRead(
      NativeGitOdbStream, (sbyte*)pinnedBuffer.Pointer, (UIntPtr)Math.Min(length, pinnedBuffer.Length));
    CheckLibgit2.Check(res, "Unable to read from ODB stream");
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.odb.GitOdbStreamFree(NativeGitOdbStream);
      _disposedValue = true;
    }
  }

  ~OdbStream()
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
