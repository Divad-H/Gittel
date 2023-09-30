namespace Libgit2Bindings.Mappers;

internal static class GitConfigLevelMapper
{
  public static GitConfigLevel FromNative(libgit2.GitConfigLevelT nativeLevel)
  {
    return nativeLevel switch
    {
      libgit2.GitConfigLevelT.GIT_CONFIG_LEVEL_PROGRAMDATA => GitConfigLevel.ProgramData,
      libgit2.GitConfigLevelT.GIT_CONFIG_LEVEL_SYSTEM => GitConfigLevel.System,
      libgit2.GitConfigLevelT.GIT_CONFIG_LEVEL_XDG => GitConfigLevel.Xdg,
      libgit2.GitConfigLevelT.GIT_CONFIG_LEVEL_GLOBAL => GitConfigLevel.Global,
      libgit2.GitConfigLevelT.GIT_CONFIG_LEVEL_LOCAL => GitConfigLevel.Local,
      libgit2.GitConfigLevelT.GIT_CONFIG_LEVEL_APP => GitConfigLevel.App,
      libgit2.GitConfigLevelT.GIT_CONFIG_HIGHEST_LEVEL => GitConfigLevel.Highest,
      _ => throw new ArgumentOutOfRangeException(nameof(nativeLevel), nativeLevel, null)
    };
  }
}
