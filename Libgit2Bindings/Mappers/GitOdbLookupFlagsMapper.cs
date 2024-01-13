namespace Libgit2Bindings.Mappers;

internal static class GitOdbLookupFlagsMapper
{
  public static libgit2.GitOdbLookupFlagsT ToNative(this GitOdbLookupFlags managedFlags)
  {
    return (libgit2.GitOdbLookupFlagsT)managedFlags;
  }
}
