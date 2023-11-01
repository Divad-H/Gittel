using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;
using System.Runtime.InteropServices;

namespace Libgit2Bindings;

internal class GitWriteStream : AbstractGitWriteStream
{
  private readonly libgit2.GitWritestream _nativeGitWriteStream;
  
  public GitWriteStream(libgit2.GitWritestream nativeGitWriteStream)
  {
    _nativeGitWriteStream = nativeGitWriteStream;
  }

  public override bool CanRead => false;

  public override bool CanSeek => false;

  public override bool CanWrite => true;

  public override long Length => throw new NotSupportedException();

  public override long Position 
  { 
    get => throw new NotSupportedException(); 
    set => throw new NotSupportedException(); 
  }

  public override void Flush()
  { }

  public override int Read(byte[] buffer, int offset, int count)
  {
    throw new NotSupportedException();
  }

  public override long Seek(long offset, SeekOrigin origin)
  {
    throw new NotSupportedException();
  }

  public override void SetLength(long value)
  {
    throw new NotSupportedException();
  }

  private delegate int WriteCallback(IntPtr stream, IntPtr buffer, UIntPtr len);
  private unsafe static WriteCallback? GetWriteCallback(libgit2.GitWritestream nativeWriteStream)
  {
    var __ptr0 = ((libgit2.GitWritestream.__Internal*)nativeWriteStream.__Instance)->write;
    return __ptr0 == IntPtr.Zero ? null : (WriteCallback)Marshal.GetDelegateForFunctionPointer(__ptr0, typeof(WriteCallback));
  }

  public override void Write(byte[] buffer, int offset, int count)
  {
    if (buffer == null)
      throw new ArgumentNullException(nameof(buffer));
    if (offset < 0 || offset > buffer.Length)
      throw new ArgumentOutOfRangeException(nameof(offset));
    if (count <= 0 || offset + count > buffer.Length)
      throw new ArgumentOutOfRangeException(nameof(count));
    if (_disposedValue)
      throw new ObjectDisposedException(nameof(GitWriteStream));
    if (_committed)
      throw new InvalidOperationException("Write stream already committed");

    var writeCallback = GetWriteCallback(_nativeGitWriteStream);
    if (writeCallback == null)
      throw new InvalidOperationException("Unable to get write callback");
    using var pinnedBuffer = new PinnedBuffer(buffer);
    var ptr = pinnedBuffer.GetPointerAtOffset(offset);
    var res = writeCallback(_nativeGitWriteStream.__Instance, ptr, new UIntPtr((uint)count));
    CheckLibgit2.Check(res, "Unable to write to write stream");
  }

  private bool _committed = false;

  public override GitOid Commit()
  {
    if (_disposedValue)
      throw new ObjectDisposedException(nameof(GitWriteStream));
    if (_committed)
      throw new InvalidOperationException("Write stream already committed");

    var res = libgit2.blob.GitBlobCreateFromStreamCommit(out var nativeOid, _nativeGitWriteStream);
    CheckLibgit2.Check(res, "Unable to commit write stream");
    using (nativeOid)
    {
      _committed = true;
      return GitOidMapper.FromNative(nativeOid);
    }
  }

  private bool _disposedValue;
  protected override void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (!_committed)
      {
        _nativeGitWriteStream.Close(_nativeGitWriteStream.__Instance);
        _nativeGitWriteStream.Free(_nativeGitWriteStream.__Instance);
      }
      _nativeGitWriteStream.Dispose();

      base.Dispose(disposing);
      _disposedValue = true;
    }
  }
}
