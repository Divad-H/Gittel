namespace Libgit2Bindings.Mappers;

internal static class GitConfigEntryMapper
{
  public static GitConfigEntry? FromNativePtr(IntPtr nativeGitConfigEntryPtr)
  {
    if (nativeGitConfigEntryPtr == IntPtr.Zero)
      return null;
    using var nativeGitConfigEntry = libgit2.GitConfigEntry.__CreateInstance(nativeGitConfigEntryPtr);
    return FromNative(nativeGitConfigEntry);
  }

  public static GitConfigEntry FromNative(libgit2.GitConfigEntry nativeGitConfigEntry)
  {
    return new()
    {
      Name = nativeGitConfigEntry.Name,
      Value = nativeGitConfigEntry.Value,
      BackendType = nativeGitConfigEntry.BackendType,
      OriginPath = nativeGitConfigEntry.OriginPath,
      IncludeDepth = nativeGitConfigEntry.IncludeDepth,
      Level = GitConfigLevelMapper.FromNative(nativeGitConfigEntry.Level),
    };
  }
}
