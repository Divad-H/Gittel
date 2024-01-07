using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class GitMergeFileResultMapper
{
  public static unsafe GitMergeFileResult ToManaged(this libgit2.GitMergeFileResult nativeResult,
    DisposableCollection disposables)
  {
    return new GitMergeFileResult
    {
      Automergeable = nativeResult.Automergeable != 0,
      Path = nativeResult.Path,
      Mode = nativeResult.Mode,
      Content = StringUtil.ToArray(
        ((libgit2.GitMergeFileResult.__Internal*)nativeResult.__Instance)->ptr, (UIntPtr)nativeResult.Len)
    };
  }
}
