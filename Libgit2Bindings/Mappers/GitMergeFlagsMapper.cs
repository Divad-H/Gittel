namespace Libgit2Bindings.Mappers;

internal static class GitMergeFlagsMapper
{
  public static libgit2.GitMergeFlagT ToNative(this GitMergeFlags managedFlags)
  {
    libgit2.GitMergeFlagT result = 0;

    if ((managedFlags & GitMergeFlags.FindRenames) != 0)
      result |= libgit2.GitMergeFlagT.GIT_MERGE_FIND_RENAMES;
    if ((managedFlags & GitMergeFlags.FailOnConflict) != 0)
      result |= libgit2.GitMergeFlagT.GIT_MERGE_FAIL_ON_CONFLICT;
    if ((managedFlags & GitMergeFlags.SkipReuc) != 0)
      result |= libgit2.GitMergeFlagT.GIT_MERGE_SKIP_REUC;
    if ((managedFlags & GitMergeFlags.NoRecursive) != 0)
      result |= libgit2.GitMergeFlagT.GIT_MERGE_NO_RECURSIVE;
    if ((managedFlags & GitMergeFlags.VirtualBase) != 0)
      result |= libgit2.GitMergeFlagT.GIT_MERGE_VIRTUAL_BASE;

    return result;
  }
}
