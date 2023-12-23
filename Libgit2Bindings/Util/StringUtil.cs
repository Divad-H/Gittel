using System.Runtime.InteropServices;
using System.Text;

namespace Libgit2Bindings.Util;

internal static class StringUtil
{
  public static unsafe string ToString(libgit2.GitError err)
  {
    return ToString(err.Message);
  }

  public static unsafe string ToString(libgit2.GitBuf buf)
  {
    return new string(buf.Ptr, 0, (int)buf.Size, Encoding.UTF8);
  }

  public static unsafe string ToString(sbyte* utf8String)
  {
    var span = MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*)utf8String);
    return Encoding.UTF8.GetString(span);
  }

  public static unsafe byte[] ToArray(libgit2.GitBuf buf)
  {
    return ToArray((IntPtr)buf.Ptr, (UIntPtr)buf.Size);
  }

  public static byte[] ToArray(IntPtr ptr, UIntPtr size)
  {
    var bytes = new byte[(int)size];
    Marshal.Copy(ptr, bytes, 0, (int)size);
    return bytes;
  }

  public unsafe static ReadOnlySpan<byte> ToReadOnlySpan(IntPtr ptr, UIntPtr size)
  {
    return new(ptr.ToPointer(), (int)size);
  }
}
