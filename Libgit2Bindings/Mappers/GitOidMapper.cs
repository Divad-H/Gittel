namespace Libgit2Bindings.Mappers;

internal static class GitOidMapper
{
  public static GitOid FromNative(libgit2.GitOid nativeOid)
  {
    return new(nativeOid.Id);
  }
}
