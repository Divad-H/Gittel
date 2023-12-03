using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class GitDiffLineMapper
{
  public unsafe static GitDiffLine FromNative(libgit2.GitDiffLine gitDiffLine)
  {
    var gitDiffLineInstance = (libgit2.GitDiffLine.__Internal*)gitDiffLine.__Instance;

    GitDiffLine diffLine = new()
    {
      Origin = Convert.ToChar(gitDiffLine.Origin),
      OldLineNumber = gitDiffLine.OldLineno,
      NewLineNumber = gitDiffLine.NewLineno,
      NumLines = gitDiffLine.NumLines,
      ContentOffset = gitDiffLine.ContentOffset,
      ContentLength = (UIntPtr)gitDiffLine.ContentLen,
      Content = StringUtil.ToArray(
        gitDiffLineInstance->content,
        (UIntPtr)gitDiffLineInstance->content_len)
    };
    return diffLine;
  }
}
