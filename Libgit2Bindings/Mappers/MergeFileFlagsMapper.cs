namespace Libgit2Bindings.Mappers;

internal static class MergeFileFlagsMapper
{
  public static libgit2.GitMergeFileFlagT ToNative(this MergeFileFlags managedFlags)
  {
    libgit2.GitMergeFileFlagT result = 0;

    if ((managedFlags & MergeFileFlags.Default) != 0)
      result |= libgit2.GitMergeFileFlagT.GIT_MERGE_FILE_DEFAULT;
    if ((managedFlags & MergeFileFlags.StyleMerge) != 0)
      result |= libgit2.GitMergeFileFlagT.GIT_MERGE_FILE_STYLE_MERGE;
    if ((managedFlags & MergeFileFlags.StyleDiff3) != 0)
      result |= libgit2.GitMergeFileFlagT.GIT_MERGE_FILE_STYLE_DIFF3;
    if ((managedFlags & MergeFileFlags.SimplifyAlnum) != 0)
      result |= libgit2.GitMergeFileFlagT.GIT_MERGE_FILE_SIMPLIFY_ALNUM;
    if ((managedFlags & MergeFileFlags.IgnoreWhitespace) != 0)
      result |= libgit2.GitMergeFileFlagT.GIT_MERGE_FILE_IGNORE_WHITESPACE;
    if ((managedFlags & MergeFileFlags.IgnoreWhitespaceChange) != 0)
      result |= libgit2.GitMergeFileFlagT.GIT_MERGE_FILE_IGNORE_WHITESPACE_CHANGE;
    if ((managedFlags & MergeFileFlags.IgnoreWhitespaceEol) != 0)
      result |= libgit2.GitMergeFileFlagT.GIT_MERGE_FILE_IGNORE_WHITESPACE_EOL;
    if ((managedFlags & MergeFileFlags.DiffPatience) != 0)
      result |= libgit2.GitMergeFileFlagT.GIT_MERGE_FILE_DIFF_PATIENCE;
    if ((managedFlags & MergeFileFlags.DiffMinimal) != 0)
      result |= libgit2.GitMergeFileFlagT.GIT_MERGE_FILE_DIFF_MINIMAL;
    if ((managedFlags & MergeFileFlags.StyleZdiff3) != 0)
      result |= libgit2.GitMergeFileFlagT.GIT_MERGE_FILE_STYLE_ZDIFF3;
    if ((managedFlags & MergeFileFlags.AcceptConflict) != 0)
      result |= libgit2.GitMergeFileFlagT.GIT_MERGE_FILE_ACCEPT_CONFLICTS;

    return result;
  }
}
