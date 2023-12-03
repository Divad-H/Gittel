using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class GitDiffBinaryMapper
{
  public static GitDiffBinary FromNative(libgit2.GitDiffBinary gitDiffBinary)
  {
    GitDiffBinary diffBinary = new()
    {
      ContainsData = gitDiffBinary.ContainsData != 0,
      OldFile = FromNative(gitDiffBinary.OldFile),
      NewFile = FromNative(gitDiffBinary.NewFile),
    };
    return diffBinary;
  }

  public static unsafe GitDiffBinaryFile FromNative(libgit2.GitDiffBinaryFile gitDiffBinaryFile)
  {
    GitDiffBinaryFile diffBinaryFile = new()
    {
      Type = FromNative(gitDiffBinaryFile.Type),
      DeflatedData = StringUtil.ToArray(
        ((libgit2.GitDiffBinaryFile.__Internal*)gitDiffBinaryFile.__Instance)->data,
        (UIntPtr)gitDiffBinaryFile.Datalen),
      InflatedLength = gitDiffBinaryFile.Inflatedlen
    };
    return diffBinaryFile;
  }

  public static GitDiffBinaryType FromNative(libgit2.GitDiffBinaryT gitDiffBinaryType)
  {
    return gitDiffBinaryType switch
    {
      libgit2.GitDiffBinaryT.GIT_DIFF_BINARY_NONE => GitDiffBinaryType.None,
      libgit2.GitDiffBinaryT.GIT_DIFF_BINARY_LITERAL => GitDiffBinaryType.Literal,
      libgit2.GitDiffBinaryT.GIT_DIFF_BINARY_DELTA => GitDiffBinaryType.Delta,
      _ => throw new InvalidOperationException($"Unknown GitDiffBinaryType value: {gitDiffBinaryType}")
    };
  }
}
