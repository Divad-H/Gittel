namespace Libgit2Bindings.Mappers;

internal static class GitRebaseOperationMapper
{
  public static GitRebaseOperation FromNative(libgit2.GitRebaseOperation nativeRebaseOperation)
  {
    return new GitRebaseOperation()
    {
      Type = (GitRebaseOperationType)nativeRebaseOperation.Type,
      Id = nativeRebaseOperation.Id is null ? null : GitOidMapper.FromNative(nativeRebaseOperation.Id),
      Exec = nativeRebaseOperation.Exec,
    };
  }
}
