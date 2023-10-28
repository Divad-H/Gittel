namespace Libgit2Bindings.Mappers;

internal static class GitFileModeMapper
{
  public static GitFilemode FromNative(libgit2.GitFilemodeT nativeFileMode)
  {
   return (GitFilemode)nativeFileMode;
  }
  public static GitFilemode FromNative(UInt16 nativeFileMode)
  {
   return (GitFilemode)nativeFileMode;
  }
}
