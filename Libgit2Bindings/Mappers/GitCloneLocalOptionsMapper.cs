namespace Libgit2Bindings.Mappers;

internal static class GitCloneLocalOptionsMapper
{
  public static libgit2.GitCloneLocalT ToNative(this CloneLocal cloneLocal)
  {
    return cloneLocal switch
    {
      CloneLocal.LocalAuto => libgit2.GitCloneLocalT.GIT_CLONE_LOCAL_AUTO,
      CloneLocal.Local => libgit2.GitCloneLocalT.GIT_CLONE_LOCAL,
      CloneLocal.NoLocal => libgit2.GitCloneLocalT.GIT_CLONE_NO_LOCAL,
      CloneLocal.LocalNoLinks => libgit2.GitCloneLocalT.GIT_CLONE_LOCAL_NO_LINKS,
      _ => throw new ArgumentOutOfRangeException(nameof(cloneLocal), cloneLocal, null)
    };
  }
}
