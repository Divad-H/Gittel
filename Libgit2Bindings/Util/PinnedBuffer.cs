using System.Runtime.InteropServices;

namespace Libgit2Bindings.Util;

internal class PinnedBuffer : IDisposable
{
  private readonly GCHandle _handle;
  private readonly byte[] _buffer;

  public PinnedBuffer(byte[] buffer)
  {
    _handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
    _buffer = buffer;
  }

  public IntPtr Pointer => _handle.AddrOfPinnedObject();

  public int Length => _buffer.Length;

  public IntPtr GetPointerAtOffset(int offset)
  {
    if (offset < 0 || offset >= _buffer.Length)
      throw new ArgumentOutOfRangeException(nameof(offset));
    return IntPtr.Add(Pointer, offset);
  }

  public void Dispose()
  {
    _handle.Free();
  }
}
