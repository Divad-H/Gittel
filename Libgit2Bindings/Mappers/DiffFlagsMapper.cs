namespace Libgit2Bindings.Mappers;

internal static class DiffFlagsMapper
{
  public static DiffFlags FromNative(libgit2.GitDiffFlagT nativeFlags)
  {
    DiffFlags managedFlags = 0;
    if ((nativeFlags & libgit2.GitDiffFlagT.GIT_DIFF_FLAG_BINARY) != 0)
      managedFlags |= DiffFlags.Binary;
    if ((nativeFlags & libgit2.GitDiffFlagT.GIT_DIFF_FLAG_NOT_BINARY) != 0)
      managedFlags |= DiffFlags.NotBinary;
    if ((nativeFlags & libgit2.GitDiffFlagT.GIT_DIFF_FLAG_VALID_ID) != 0)
      managedFlags |= DiffFlags.ValidId;
    if ((nativeFlags & libgit2.GitDiffFlagT.GIT_DIFF_FLAG_EXISTS) != 0)
      managedFlags |= DiffFlags.Exists;
    if ((nativeFlags & libgit2.GitDiffFlagT.GIT_DIFF_FLAG_VALID_SIZE) != 0)
      managedFlags |= DiffFlags.ValidSize;
    return managedFlags;
  }
}
