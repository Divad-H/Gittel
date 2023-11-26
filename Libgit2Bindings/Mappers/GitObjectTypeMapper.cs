namespace Libgit2Bindings.Mappers;

internal static class GitObjectTypeMapper
{
  public static GitObjectType FromNative(libgit2.GitObjectT nativeGitObjectType)
  {
    return nativeGitObjectType switch
    {
      libgit2.GitObjectT.GIT_OBJECT_ANY => GitObjectType.Any,
      libgit2.GitObjectT.GIT_OBJECT_INVALID=> GitObjectType.Invalid,
      libgit2.GitObjectT.GIT_OBJECT_COMMIT => GitObjectType.Commit,
      libgit2.GitObjectT.GIT_OBJECT_TREE => GitObjectType.Tree,
      libgit2.GitObjectT.GIT_OBJECT_BLOB => GitObjectType.Blob,
      libgit2.GitObjectT.GIT_OBJECT_TAG => GitObjectType.Tag,
      libgit2.GitObjectT.GIT_OBJECT_OFS_DELTA => GitObjectType.OffsetDelta,
      libgit2.GitObjectT.GIT_OBJECT_REF_DELTA => GitObjectType.RefDelta,
      _ => throw new ArgumentOutOfRangeException(nameof(nativeGitObjectType), nativeGitObjectType, null)
    };
  }

  public static libgit2.GitObjectT ToNative(this GitObjectType gitObjectType)
  {
    return gitObjectType switch
    {
      GitObjectType.Any => libgit2.GitObjectT.GIT_OBJECT_ANY,
      GitObjectType.Invalid => libgit2.GitObjectT.GIT_OBJECT_INVALID,
      GitObjectType.Commit => libgit2.GitObjectT.GIT_OBJECT_COMMIT,
      GitObjectType.Tree => libgit2.GitObjectT.GIT_OBJECT_TREE,
      GitObjectType.Blob => libgit2.GitObjectT.GIT_OBJECT_BLOB,
      GitObjectType.Tag => libgit2.GitObjectT.GIT_OBJECT_TAG,
      GitObjectType.OffsetDelta => libgit2.GitObjectT.GIT_OBJECT_OFS_DELTA,
      GitObjectType.RefDelta => libgit2.GitObjectT.GIT_OBJECT_REF_DELTA,
      _ => throw new ArgumentOutOfRangeException(nameof(gitObjectType), gitObjectType, null)
    };
  }
}
