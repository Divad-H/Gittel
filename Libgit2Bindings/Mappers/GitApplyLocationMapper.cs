namespace Libgit2Bindings.Mappers;

internal static class GitApplyLocationMapper
{
  public static libgit2.GitApplyLocationT ToNative(this GitApplyLocation managedLocation)
  {
    return (libgit2.GitApplyLocationT)managedLocation;
  }
}
