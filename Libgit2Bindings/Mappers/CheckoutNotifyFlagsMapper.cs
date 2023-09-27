namespace Libgit2Bindings.Mappers;

internal static class CheckoutNotifyFlagsMapper
{
  public static libgit2.GitCheckoutNotifyT ToNative(CheckoutNotifyFlags notifyFlags)
  {
    libgit2.GitCheckoutNotifyT nativeFlags = 0;
    if ((notifyFlags & CheckoutNotifyFlags.Conflict) != 0)
      nativeFlags |= libgit2.GitCheckoutNotifyT.GIT_CHECKOUT_NOTIFY_CONFLICT;
    if ((notifyFlags & CheckoutNotifyFlags.Dirty) != 0)
      nativeFlags |= libgit2.GitCheckoutNotifyT.GIT_CHECKOUT_NOTIFY_DIRTY;
    if ((notifyFlags & CheckoutNotifyFlags.Updated) != 0)
      nativeFlags |= libgit2.GitCheckoutNotifyT.GIT_CHECKOUT_NOTIFY_UPDATED;
    if ((notifyFlags & CheckoutNotifyFlags.Untracked) != 0)
      nativeFlags |= libgit2.GitCheckoutNotifyT.GIT_CHECKOUT_NOTIFY_UNTRACKED;
    if ((notifyFlags & CheckoutNotifyFlags.Ignored) != 0)
      nativeFlags |= libgit2.GitCheckoutNotifyT.GIT_CHECKOUT_NOTIFY_IGNORED;
    return nativeFlags;
  }

  public static CheckoutNotifyFlags FromNative(libgit2.GitCheckoutNotifyT nativeFlags)
  {
    CheckoutNotifyFlags managedFlags = 0;
    if ((nativeFlags & libgit2.GitCheckoutNotifyT.GIT_CHECKOUT_NOTIFY_CONFLICT) != 0)
      managedFlags |= CheckoutNotifyFlags.Conflict;
    if ((nativeFlags & libgit2.GitCheckoutNotifyT.GIT_CHECKOUT_NOTIFY_DIRTY) != 0)
      managedFlags |= CheckoutNotifyFlags.Dirty;
    if ((nativeFlags & libgit2.GitCheckoutNotifyT.GIT_CHECKOUT_NOTIFY_UPDATED) != 0)
      managedFlags |= CheckoutNotifyFlags.Updated;
    if ((nativeFlags & libgit2.GitCheckoutNotifyT.GIT_CHECKOUT_NOTIFY_UNTRACKED) != 0)
      managedFlags |= CheckoutNotifyFlags.Untracked;
    if ((nativeFlags & libgit2.GitCheckoutNotifyT.GIT_CHECKOUT_NOTIFY_IGNORED) != 0)
      managedFlags |= CheckoutNotifyFlags.Ignored;
    return managedFlags;
  }
}
