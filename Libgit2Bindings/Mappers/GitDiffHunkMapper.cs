using System.Text;

namespace Libgit2Bindings.Mappers;

internal static class GitDiffHunkMapper
{
  public unsafe static GitDiffHunk FromNative(libgit2.GitDiffHunk gitDiffHunk)
  {
    var gitDiffHunkInstance = (libgit2.GitDiffHunk.__Internal*)gitDiffHunk.__Instance;

    GitDiffHunk diffHunk = new()
    {
      Header = Encoding.UTF8.GetString((byte*)gitDiffHunkInstance->header, (int)gitDiffHunk.HeaderLen),
      OldStart = gitDiffHunk.OldStart,
      OldLines = gitDiffHunk.OldLines,
      NewStart = gitDiffHunk.NewStart,
      NewLines = gitDiffHunk.NewLines,
    };
    return diffHunk;
  }
}
