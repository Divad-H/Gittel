namespace Libgit2Bindings.Mappers;

internal static class GitMergeFileOptionsMapper
{
  public static libgit2.GitMergeFileOptions ToNative(this GitMergeFileOptions options)
  {
    return new()
    {
      Version = (int)libgit2.GitMergeFileOptionsVersion.GIT_MERGE_FILE_OPTIONS_VERSION,
      AncestorLabel = options.AncestorLabel,
      OurLabel = options.OurLabel,
      TheirLabel = options.TheirLabel,
      Favor = options.Favor.ToNative(),
      Flags = (UInt32)options.Flags.ToNative(),
      MarkerSize = options.MarkerSize,
    };
  }
}
