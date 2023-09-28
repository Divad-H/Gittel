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
}
