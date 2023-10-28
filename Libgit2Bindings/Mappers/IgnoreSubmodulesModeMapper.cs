namespace Libgit2Bindings.Mappers;

internal static class IgnoreSubmodulesModeMapper
{
  public static libgit2.GitSubmoduleIgnoreT ToNative(IgnoreSubmodulesMode mode)
  {
    return mode switch
    {
      IgnoreSubmodulesMode.Unspecified => libgit2.GitSubmoduleIgnoreT.GIT_SUBMODULE_IGNORE_UNSPECIFIED,
      IgnoreSubmodulesMode.None => libgit2.GitSubmoduleIgnoreT.GIT_SUBMODULE_IGNORE_NONE,
      IgnoreSubmodulesMode.Untracked => libgit2.GitSubmoduleIgnoreT.GIT_SUBMODULE_IGNORE_UNTRACKED,
      IgnoreSubmodulesMode.Dirty => libgit2.GitSubmoduleIgnoreT.GIT_SUBMODULE_IGNORE_DIRTY,
      IgnoreSubmodulesMode.All => libgit2.GitSubmoduleIgnoreT.GIT_SUBMODULE_IGNORE_ALL,
      _ => throw new ArgumentException("Invalid value for IgnoreSubmodulesMode", nameof(mode)),
    };
  }
}
