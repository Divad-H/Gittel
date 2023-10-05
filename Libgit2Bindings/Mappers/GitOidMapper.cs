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

  public static GitOid FromShortId(byte[] shortId)
  {
    if (shortId.Length > 20)
    {
      throw new ArgumentException("Length of shortId must be less than or equal to 20.");
    }
    byte[] id = new byte[20];
    Array.Copy(shortId, id, shortId.Length);
    return new GitOid(id);
  }
}
