namespace Libgit2Bindings.Mappers;

internal static class DiffFileMapper
{
  public static DiffFile? FromNativePtr(IntPtr nativeDiffFilePtr)
  {
    if (nativeDiffFilePtr == IntPtr.Zero)
      return null;
    using var nativeDiffFile = libgit2.GitDiffFile.__CreateInstance(nativeDiffFilePtr);
    return FromNative(nativeDiffFile);
  }

  public static DiffFile FromNative(libgit2.GitDiffFile nativeDiffFile)
  {
    DiffFile diffFile = new()
    {
      Oid = GitOidMapper.FromNative(nativeDiffFile.Id),
      Path = nativeDiffFile.Path,
      Size = nativeDiffFile.Size,
      Flags = DiffFlagsMapper.FromNative((libgit2.GitDiffFlagT)nativeDiffFile.Flags)
    };
    return diffFile;
  }
}
