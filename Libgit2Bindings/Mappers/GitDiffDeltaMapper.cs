namespace Libgit2Bindings.Mappers;

internal static class GitDiffDeltaMapper
{
  public static GitDiffDelta? FromNativePtr(IntPtr nativeGitDiffDeltaPtr)
  {
    if (nativeGitDiffDeltaPtr == IntPtr.Zero)
      return null;
    using var nativeGitDiffDelta = libgit2.GitDiffDelta.__CreateInstance(nativeGitDiffDeltaPtr);
    return FromNative(nativeGitDiffDelta);
  }

  public static GitDiffDelta FromNative(libgit2.GitDiffDelta gitDiffDelta)
  {
    GitDiffDelta diffDelta = new(
      Status: GitDeltaTypeMapper.FromNative(gitDiffDelta.Status),
      Flags: GitDiffFlagsMapper.FromNative((libgit2.GitDiffFlagT)gitDiffDelta.Flags),
      Similarity: gitDiffDelta.Similarity,
      NFiles: gitDiffDelta.Nfiles,
      OldFile: GitDiffFileMapper.FromNative(gitDiffDelta.OldFile),
      NewFile: GitDiffFileMapper.FromNative(gitDiffDelta.NewFile)
    );
    return diffDelta;
  }
}
