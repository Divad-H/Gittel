namespace Libgit2Bindings.Mappers;

internal static class GitDiffFindFlagsMapper
{
  public static libgit2.GitDiffFindT ToNative(this GitDiffFindFlags managesFlags)
  {
    libgit2.GitDiffFindT res = 0;

    if ((managesFlags & GitDiffFindFlags.Renames) != 0)
      res |= libgit2.GitDiffFindT.GIT_DIFF_FIND_RENAMES;
    if ((managesFlags & GitDiffFindFlags.RenamesFromRewrites) != 0)
      res |= libgit2.GitDiffFindT.GIT_DIFF_FIND_RENAMES_FROM_REWRITES;
    if ((managesFlags & GitDiffFindFlags.Copies) != 0)
      res |= libgit2.GitDiffFindT.GIT_DIFF_FIND_COPIES;
    if ((managesFlags & GitDiffFindFlags.CopiesFromUnmodified) != 0)
      res |= libgit2.GitDiffFindT.GIT_DIFF_FIND_COPIES_FROM_UNMODIFIED;
    if ((managesFlags & GitDiffFindFlags.Rewrites) != 0)
      res |= libgit2.GitDiffFindT.GIT_DIFF_FIND_REWRITES;
    if ((managesFlags & GitDiffFindFlags.BreakRewrites) != 0)
      res |= libgit2.GitDiffFindT.GIT_DIFF_FIND_AND_BREAK_REWRITES;
    if ((managesFlags & GitDiffFindFlags.ForUntracked) != 0)
      res |= libgit2.GitDiffFindT.GIT_DIFF_FIND_FOR_UNTRACKED;
    if ((managesFlags & GitDiffFindFlags.All) != 0)
      res |= libgit2.GitDiffFindT.GIT_DIFF_FIND_ALL;
    if ((managesFlags & GitDiffFindFlags.IgnoreLeadingWhitespace) != 0)
      res |= libgit2.GitDiffFindT.GIT_DIFF_FIND_IGNORE_LEADING_WHITESPACE;
    if ((managesFlags & GitDiffFindFlags.IgnoreWhitespace) != 0)
      res |= libgit2.GitDiffFindT.GIT_DIFF_FIND_IGNORE_WHITESPACE;
    if ((managesFlags & GitDiffFindFlags.DontIgnoreWhitespace) != 0)
      res |= libgit2.GitDiffFindT.GIT_DIFF_FIND_DONT_IGNORE_WHITESPACE;
    if ((managesFlags & GitDiffFindFlags.ExactMatchOnly) != 0)
      res |= libgit2.GitDiffFindT.GIT_DIFF_FIND_EXACT_MATCH_ONLY;
    if ((managesFlags & GitDiffFindFlags.BreakRewritesForRenamesOnly) != 0)
      res |= libgit2.GitDiffFindT.GIT_DIFF_BREAK_REWRITES_FOR_RENAMES_ONLY;
    if ((managesFlags & GitDiffFindFlags.RemoveUnmodified) != 0)
      res |= libgit2.GitDiffFindT.GIT_DIFF_FIND_REMOVE_UNMODIFIED;

    return res;
  }
}
