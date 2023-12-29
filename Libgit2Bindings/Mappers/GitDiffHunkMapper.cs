using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class GitDiffHunkMapper
{
  public unsafe static GitDiffHunk FromNative(libgit2.GitDiffHunk gitDiffHunk)
  {
    if (gitDiffHunk is null)
      return new();
    var gitDiffHunkInstance = (libgit2.GitDiffHunk.__Internal*)gitDiffHunk.__Instance;

    GitDiffHunk diffHunk = new()
    {
      Header = StringUtil.ToReadOnlySpan((IntPtr)gitDiffHunkInstance->header, (UIntPtr)gitDiffHunk.HeaderLen),
      OldStart = gitDiffHunk.OldStart,
      OldLines = gitDiffHunk.OldLines,
      NewStart = gitDiffHunk.NewStart,
      NewLines = gitDiffHunk.NewLines,
    };
    return diffHunk;
  }

  public static GitDiffHunk FromNativePtr(IntPtr nativeGitDiffHunkPtr)
  {
    if (nativeGitDiffHunkPtr == IntPtr.Zero)
      return new();
    using var nativeGitDiffHunk = libgit2.GitDiffHunk.__CreateInstance(nativeGitDiffHunkPtr);
    return FromNative(nativeGitDiffHunk);
  }
}
