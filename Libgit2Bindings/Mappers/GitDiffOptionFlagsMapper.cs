namespace Libgit2Bindings.Mappers;

internal static class GitDiffOptionFlagsMapper
{
  public static libgit2.GitDiffOptionT ToNative(this GitDiffOptionFlags managedFlags)
  {
    libgit2.GitDiffOptionT res = 0;

    if ((managedFlags & GitDiffOptionFlags.Reverse) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_REVERSE;
    if ((managedFlags & GitDiffOptionFlags.IncludeIgnored) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_INCLUDE_IGNORED;
    if ((managedFlags & GitDiffOptionFlags.RecurseIgnoredDirs) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_RECURSE_IGNORED_DIRS;
    if ((managedFlags & GitDiffOptionFlags.IncludeUntracked) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_INCLUDE_UNTRACKED;
    if ((managedFlags & GitDiffOptionFlags.RecurseUntrackedDirs) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_RECURSE_UNTRACKED_DIRS;
    if ((managedFlags & GitDiffOptionFlags.IncludeUnmodified) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_INCLUDE_UNMODIFIED;
    if ((managedFlags & GitDiffOptionFlags.IncludeTypeChange) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_INCLUDE_TYPECHANGE;
    if ((managedFlags & GitDiffOptionFlags.IncludeTypeChangeTrees) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_INCLUDE_TYPECHANGE_TREES;
    if ((managedFlags & GitDiffOptionFlags.IgnoreFileMode) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_IGNORE_FILEMODE;
    if ((managedFlags & GitDiffOptionFlags.IgnoreSubmodules) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_IGNORE_SUBMODULES;
    if ((managedFlags & GitDiffOptionFlags.IgnoreCase) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_IGNORE_CASE;
    if ((managedFlags & GitDiffOptionFlags.IncludeCaseChange) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_INCLUDE_CASECHANGE;
    if ((managedFlags & GitDiffOptionFlags.DisablePathspecMatch) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_DISABLE_PATHSPEC_MATCH;
    if ((managedFlags & GitDiffOptionFlags.SkipBinaryCheck) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_SKIP_BINARY_CHECK;
    if ((managedFlags & GitDiffOptionFlags.EnableFastUntrackedDirs) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_ENABLE_FAST_UNTRACKED_DIRS;
    if ((managedFlags & GitDiffOptionFlags.UpdateIndex) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_UPDATE_INDEX;
    if ((managedFlags & GitDiffOptionFlags.IncludeUnreadable) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_INCLUDE_UNREADABLE;
    if ((managedFlags & GitDiffOptionFlags.IncludeUnreadableAsUntracked) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_INCLUDE_UNREADABLE_AS_UNTRACKED;
    if ((managedFlags & GitDiffOptionFlags.IndentHeuristic) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_INDENT_HEURISTIC;
    if ((managedFlags & GitDiffOptionFlags.IgnoreBlankLines) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_IGNORE_BLANK_LINES;
    if ((managedFlags & GitDiffOptionFlags.ForceText) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_FORCE_TEXT;
    if ((managedFlags & GitDiffOptionFlags.ForceBinary) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_FORCE_BINARY;
    if ((managedFlags & GitDiffOptionFlags.IgnoreWhitespace) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_IGNORE_WHITESPACE;
    if ((managedFlags & GitDiffOptionFlags.IgnoreWhitespaceChange) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_IGNORE_WHITESPACE_CHANGE;
    if ((managedFlags & GitDiffOptionFlags.IgnoreWhitespaceEol) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_IGNORE_WHITESPACE_EOL;
    if ((managedFlags & GitDiffOptionFlags.ShowUntrackedContent) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_SHOW_UNTRACKED_CONTENT;
    if ((managedFlags & GitDiffOptionFlags.ShowUnmodified) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_SHOW_UNMODIFIED;
    if ((managedFlags & GitDiffOptionFlags.Patience) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_PATIENCE;
    if ((managedFlags & GitDiffOptionFlags.Minimal) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_MINIMAL;
    if ((managedFlags & GitDiffOptionFlags.ShowBinary) != 0)
      res |= libgit2.GitDiffOptionT.GIT_DIFF_SHOW_BINARY;

    return res;
  }
}
