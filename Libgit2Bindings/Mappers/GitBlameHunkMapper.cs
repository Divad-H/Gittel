namespace Libgit2Bindings.Mappers;

internal static class GitBlameHunkMapper
{
  public static GitBlameHunk FromNative(libgit2.GitBlameHunk? nativeHunk)
  {
    if (nativeHunk is null)
    {
      return null;
    }
    return new GitBlameHunk()
    {
      LinesInHunk = nativeHunk.LinesInHunk,
      FinalCommitId = GitOidMapper.FromNative(nativeHunk.FinalCommitId),
      FinalStartLineNumber = nativeHunk.FinalStartLineNumber,
      FinalSignature = nativeHunk.FinalSignature is null ? null : new GitSignature(nativeHunk.FinalSignature, false),
      OriginalCommitId = GitOidMapper.FromNative(nativeHunk.OrigCommitId),
      OriginalPath = nativeHunk.OrigPath,
      OriginalStartLineNumber = nativeHunk.OrigStartLineNumber,
      OriginalSignature = nativeHunk.OrigSignature is null ? null : new GitSignature(nativeHunk.OrigSignature, false),
      Boundary = nativeHunk.Boundary != 0,
    };
  }
}
