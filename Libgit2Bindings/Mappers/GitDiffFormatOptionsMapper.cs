namespace Libgit2Bindings.Mappers;

internal static class GitDiffFormatOptionsMapper
{
  public static libgit2.GitDiffFormatT ToNative(this GitDiffFormatOptions managedOptions)
  {
    return (libgit2.GitDiffFormatT)managedOptions;
  }
}
