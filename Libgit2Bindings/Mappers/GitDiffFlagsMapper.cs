namespace Libgit2Bindings.Mappers;

internal static class GitDiffFlagsMapper
{
  public static GitDiffFlags FromNative(libgit2.GitDiffFlagT nativeFlags)
  {
    GitDiffFlags managedFlags = 0;
    if ((nativeFlags & libgit2.GitDiffFlagT.GIT_DIFF_FLAG_BINARY) != 0)
      managedFlags |= GitDiffFlags.Binary;
    if ((nativeFlags & libgit2.GitDiffFlagT.GIT_DIFF_FLAG_NOT_BINARY) != 0)
      managedFlags |= GitDiffFlags.NotBinary;
    if ((nativeFlags & libgit2.GitDiffFlagT.GIT_DIFF_FLAG_VALID_ID) != 0)
      managedFlags |= GitDiffFlags.ValidId;
    if ((nativeFlags & libgit2.GitDiffFlagT.GIT_DIFF_FLAG_EXISTS) != 0)
      managedFlags |= GitDiffFlags.Exists;
    if ((nativeFlags & libgit2.GitDiffFlagT.GIT_DIFF_FLAG_VALID_SIZE) != 0)
      managedFlags |= GitDiffFlags.ValidSize;
    return managedFlags;
  }
}
