namespace Libgit2Bindings.Mappers;

internal static class GitIndexerProgressMapper
{
  public static GitIndexerProgress? FromNativePtr(IntPtr nativeGitIndexerProgressPtr)
  {
    if (nativeGitIndexerProgressPtr == IntPtr.Zero)
      return null;
    using var nativeGitIndexerProgress = libgit2.GitIndexerProgress.__CreateInstance(nativeGitIndexerProgressPtr);
    return FromNative(nativeGitIndexerProgress);
  }

  public static GitIndexerProgress FromNative(libgit2.GitIndexerProgress nativeGitIndexerProgress)
  {
    return new(
      TotalObjects: nativeGitIndexerProgress.TotalObjects,
      IndexedObjects: nativeGitIndexerProgress.IndexedObjects,
      ReceivedObjects: nativeGitIndexerProgress.ReceivedObjects,
      LocalObjects: nativeGitIndexerProgress.LocalObjects,
      TotalDeltas: nativeGitIndexerProgress.TotalDeltas,
      IndexedDeltas: nativeGitIndexerProgress.IndexedDeltas,
      ReceivedBytes: nativeGitIndexerProgress.ReceivedBytes);
  }
}
