namespace Libgit2Bindings.Mappers;

internal static class BranchTypeMapper
{
  public static libgit2.GitBranchT ToNative(BranchType branchType)
  {
    libgit2.GitBranchT nativeFlags = 0;
    if (branchType == BranchType.LocalBranch)
      nativeFlags |= libgit2.GitBranchT.GIT_BRANCH_LOCAL;
    if (branchType == BranchType.RemoteBranch)
      nativeFlags |= libgit2.GitBranchT.GIT_BRANCH_REMOTE;
    if (branchType == BranchType.All)
      nativeFlags |= libgit2.GitBranchT.GIT_BRANCH_ALL;
    return nativeFlags;
  }
}
