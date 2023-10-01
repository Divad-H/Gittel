using System.Runtime.InteropServices;

namespace Libgit2Bindings.Mappers;

internal static class GitOidMapper
{
  public static GitOid FromNative(libgit2.GitOid nativeOid)
  {
    return new(nativeOid.Id);
  }

  public unsafe static libgit2.GitOid ToNative(GitOid oid)
  {
    var data = new libgit2.GitOid.__Internal();
    Marshal.Copy(oid.Id, 0, (IntPtr)data.id, oid.Id.Length);
    return libgit2.GitOid.__CreateInstance(data);
  }
}
