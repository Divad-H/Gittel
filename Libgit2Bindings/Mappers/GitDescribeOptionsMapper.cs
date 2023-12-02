namespace Libgit2Bindings.Mappers;

internal static class GitDescribeOptionsMapper
{
  public static libgit2.GitDescribeOptions ToNative(this GitDescribeOptions managedOptions)
  {
    var nativeOptions = new libgit2.GitDescribeOptions
    {
      Version = (UInt32)libgit2.GitDescribeOptionsVersion.GIT_DESCRIBE_OPTIONS_VERSION,
      MaxCandidatesTags = managedOptions.MaxCandidatesTags,
      DescribeStrategy = (UInt32)(managedOptions.DescribeStrategy switch { 
        GitDescribeStrategy.Default => libgit2.GitDescribeStrategyT.GIT_DESCRIBE_DEFAULT,
        GitDescribeStrategy.All => libgit2.GitDescribeStrategyT.GIT_DESCRIBE_ALL,
        GitDescribeStrategy.Tags => libgit2.GitDescribeStrategyT.GIT_DESCRIBE_TAGS,
        _ => libgit2.GitDescribeStrategyT.GIT_DESCRIBE_DEFAULT
      }),
      Pattern = managedOptions.Pattern,
      OnlyFollowFirstParent = managedOptions.OnlyFollowFirstParent ? 1 : 0,
      ShowCommitOidAsFallback = managedOptions.ShowCommitOidAsFallback ? 1 : 0,
    };

    return nativeOptions;
  }
}
