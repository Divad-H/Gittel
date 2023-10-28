namespace Libgit2Bindings.Mappers;

internal static class GitDiffFileMapper
{
  public static GitDiffFile? FromNativePtr(IntPtr nativeDiffFilePtr)
  {
    if (nativeDiffFilePtr == IntPtr.Zero)
      return null;
    using var nativeDiffFile = libgit2.GitDiffFile.__CreateInstance(nativeDiffFilePtr);
    return FromNative(nativeDiffFile);
  }

  public static GitDiffFile FromNative(libgit2.GitDiffFile nativeDiffFile)
  {
    GitDiffFile diffFile = new()
    {
      Oid = GitOidMapper.FromNative(nativeDiffFile.Id),
      Path = nativeDiffFile.Path,
      Size = nativeDiffFile.Size,
      Flags = GitDiffFlagsMapper.FromNative((libgit2.GitDiffFlagT)nativeDiffFile.Flags),
      Mode = GitFileModeMapper.FromNative(nativeDiffFile.Mode),
      IdAbbrev = nativeDiffFile.IdAbbrev
    };
    return diffFile;
  }
}
