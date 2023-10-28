namespace Libgit2Bindings.Mappers;

internal static class GitOidTypeMapper
{
  public static libgit2.GitOidT ToNative(this GitOidType managedOidType)
  {
    return managedOidType switch
    {
      GitOidType.Sha1 => libgit2.GitOidT.GIT_OID_SHA1,
      //GitOidType.Sha256 => libgit2.GitOidT.GIT_OID_SHA256,
      _ => throw new ArgumentException("Invalid GitOidType", nameof(managedOidType))
    };
  }
}
