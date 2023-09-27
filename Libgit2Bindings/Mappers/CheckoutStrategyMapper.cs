namespace Libgit2Bindings.Mappers;

internal static class CheckoutStrategyMapper
{
  public static libgit2.GitCheckoutStrategyT ToNative(CheckoutStrategy strategy)
  {
    libgit2.GitCheckoutStrategyT nativeStrategy = 0;
    if ((strategy & CheckoutStrategy.Safe) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_SAFE;
    if ((strategy & CheckoutStrategy.Force) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_FORCE;
    if ((strategy & CheckoutStrategy.RecreateMissing) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_RECREATE_MISSING;
    if ((strategy & CheckoutStrategy.AllowConflicts) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_ALLOW_CONFLICTS;
    if ((strategy & CheckoutStrategy.RemoveUntracked) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_REMOVE_UNTRACKED;
    if ((strategy & CheckoutStrategy.RemoveIgnored) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_REMOVE_IGNORED;
    if ((strategy & CheckoutStrategy.UpdateOnly) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_UPDATE_ONLY;
    if ((strategy & CheckoutStrategy.DontUpdateIndex) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_DONT_UPDATE_INDEX;
    if ((strategy & CheckoutStrategy.NoRefresh) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_NO_REFRESH;
    if ((strategy & CheckoutStrategy.SkipUnmerged) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_SKIP_UNMERGED;
    if ((strategy & CheckoutStrategy.UseOurs) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_USE_OURS;
    if ((strategy & CheckoutStrategy.UseTheirs) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_USE_THEIRS;
    if ((strategy & CheckoutStrategy.DisablePathspecMatch) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_DISABLE_PATHSPEC_MATCH;
    if ((strategy & CheckoutStrategy.SkipLockedDirectories) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_SKIP_LOCKED_DIRECTORIES;
    if ((strategy & CheckoutStrategy.DontOverwriteIgnored) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_DONT_OVERWRITE_IGNORED;
    if ((strategy & CheckoutStrategy.ConflictStyleMerge) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_CONFLICT_STYLE_MERGE;
    if ((strategy & CheckoutStrategy.ConflictStyleDiff3) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_CONFLICT_STYLE_DIFF3;
    if ((strategy & CheckoutStrategy.DontRemoveExisting) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_DONT_REMOVE_EXISTING;
    if ((strategy & CheckoutStrategy.DontWriteIndex) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_DONT_WRITE_INDEX;
    if ((strategy & CheckoutStrategy.DryRun) != 0)
      nativeStrategy |= libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_DRY_RUN;
    return nativeStrategy;
  }

  public static CheckoutStrategy FromNative(libgit2.GitCheckoutStrategyT nativeStrategy)
  {
    CheckoutStrategy managedStrategy = 0;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_SAFE) != 0)
      managedStrategy |= CheckoutStrategy.Safe;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_FORCE) != 0)
      managedStrategy |= CheckoutStrategy.Force;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_RECREATE_MISSING) != 0)
      managedStrategy |= CheckoutStrategy.RecreateMissing;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_ALLOW_CONFLICTS) != 0)
      managedStrategy |= CheckoutStrategy.AllowConflicts;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_REMOVE_UNTRACKED) != 0)
      managedStrategy |= CheckoutStrategy.RemoveUntracked;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_REMOVE_IGNORED) != 0)
      managedStrategy |= CheckoutStrategy.RemoveIgnored;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_UPDATE_ONLY) != 0)
      managedStrategy |= CheckoutStrategy.UpdateOnly;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_DONT_UPDATE_INDEX) != 0)
      managedStrategy |= CheckoutStrategy.DontUpdateIndex;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_NO_REFRESH) != 0)
      managedStrategy |= CheckoutStrategy.NoRefresh;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_SKIP_UNMERGED) != 0)
      managedStrategy |= CheckoutStrategy.SkipUnmerged;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_USE_OURS) != 0)
      managedStrategy |= CheckoutStrategy.UseOurs;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_USE_THEIRS) != 0)
      managedStrategy |= CheckoutStrategy.UseTheirs;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_DISABLE_PATHSPEC_MATCH) != 0)
      managedStrategy |= CheckoutStrategy.DisablePathspecMatch;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_SKIP_LOCKED_DIRECTORIES) != 0)
      managedStrategy |= CheckoutStrategy.SkipLockedDirectories;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_DONT_OVERWRITE_IGNORED) != 0)
      managedStrategy |= CheckoutStrategy.DontOverwriteIgnored;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_CONFLICT_STYLE_MERGE) != 0)
      managedStrategy |= CheckoutStrategy.ConflictStyleMerge;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_CONFLICT_STYLE_DIFF3) != 0)
      managedStrategy |= CheckoutStrategy.ConflictStyleDiff3;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_DONT_REMOVE_EXISTING) != 0)
      managedStrategy |= CheckoutStrategy.DontRemoveExisting;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_DONT_WRITE_INDEX) != 0)
      managedStrategy |= CheckoutStrategy.DontWriteIndex;
    if ((nativeStrategy & libgit2.GitCheckoutStrategyT.GIT_CHECKOUT_DRY_RUN) != 0)
      managedStrategy |= CheckoutStrategy.DryRun;
    return managedStrategy;
  }
}
