using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers
{
  internal static class GitPushUpdateMapper
  {
    public static GitPushUpdate FromNativePtr(IntPtr nativeGitPushUpdatePtr)
    {
      if (nativeGitPushUpdatePtr == IntPtr.Zero)
        throw new ArgumentNullException(nameof(nativeGitPushUpdatePtr));
      using var nativeGitPushUpdate = libgit2.GitPushUpdate.__CreateInstance(nativeGitPushUpdatePtr);
      return FromNative(nativeGitPushUpdate);
    }

    public unsafe static GitPushUpdate FromNative(libgit2.GitPushUpdate nativeGitPushUpdate)
    {
      return new(
        SourceReferenceName: StringUtil.ToString(nativeGitPushUpdate.SrcRefname),
        DestinationReferenceName: StringUtil.ToString(nativeGitPushUpdate.DstRefname),
        SourceTarget: GitOidMapper.FromNative(nativeGitPushUpdate.Src),
        DestinationTarget: GitOidMapper.FromNative(nativeGitPushUpdate.Dst)
      );
    }
  }
}
