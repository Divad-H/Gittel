using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class GitBlobFilterOptionsMapper
{
  public static libgit2.GitBlobFilterOptions ToNative(this GitBlobFilterOptions options,
    DisposableCollection disposables)
  {
    libgit2.GitBlobFilterOptions res = new();
    res.Version = (Int32)libgit2.GitBlobFilterOptionsVersion.GIT_BLOB_FILTER_OPTIONS_VERSION;
    if (options.Flags.HasFlag(GitBlobFilterFlags.CheckForBinary))
      res.Flags |= (uint)libgit2.GitBlobFilterFlagT.GIT_BLOB_FILTER_CHECK_FOR_BINARY;
    if (options.Flags.HasFlag(GitBlobFilterFlags.NoSystemAttributes))
      res.Flags |= (uint)libgit2.GitBlobFilterFlagT.GIT_BLOB_FILTER_NO_SYSTEM_ATTRIBUTES;
    if (options.Flags.HasFlag(GitBlobFilterFlags.AttributesFromHead))
      res.Flags |= (uint)libgit2.GitBlobFilterFlagT.GIT_BLOB_FILTER_ATTRIBUTES_FROM_HEAD;
    if (options.Flags.HasFlag(GitBlobFilterFlags.AttributesFromCommit))
      res.Flags |= (uint)libgit2.GitBlobFilterFlagT.GIT_BLOB_FILTER_ATTRIBUTES_FROM_COMMIT;
    if (options.AttributesCommitId is not null)
    {
      res.AttrCommitId = GitOidMapper.ToNative(options.AttributesCommitId)
        .DisposeWith(disposables);
    }
    return res;
  }
}
