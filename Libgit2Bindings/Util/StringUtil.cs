namespace Libgit2Bindings.Util;

internal static class StringUtil
{
  public static unsafe string ToString(libgit2.GitError err)
  {
    return new string(err.Message);
  }

  public static unsafe string ToString(libgit2.GitBuf buf)
  {
    return new string(buf.Ptr, 0, (int)buf.Size);
  }

}
