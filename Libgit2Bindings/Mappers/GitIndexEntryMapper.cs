namespace Libgit2Bindings.Mappers;

internal static class GitIndexEntryMapper
{
  public static GitIndexTime ToManaged(this libgit2.GitIndexTime time)
  {
    return new()
    {
      Seconds = time.Seconds,
      Nanoseconds = time.Nanoseconds,
    };
  }

  public static GitIndexEntry ToManaged(this libgit2.GitIndexEntry entry)
  {
    return new()
    {
      CTime = entry.Ctime.ToManaged(),
      MTime = entry.Mtime.ToManaged(),
      
      Dev = entry.Dev,
      Ino = entry.Ino,
      Mode = entry.Mode,
      Uid = entry.Uid,
      Gid = entry.Gid,
      FileSize = entry.FileSize,

      Id = GitOidMapper.FromNative(entry.Id),

      Flags = entry.Flags,
      FlagsExtended = entry.FlagsExtended,

      Path = entry.Path,
    };
  }

  public static libgit2.GitIndexTime ToNative(this GitIndexTime time)
  {
    return new()
    {
      Seconds = time.Seconds,
      Nanoseconds = time.Nanoseconds,
    };
  }

  public static libgit2.GitIndexEntry ToNative(this GitIndexEntry entry)
  {
    return new()
    {
      Ctime = entry.CTime.ToNative(),
      Mtime = entry.MTime.ToNative(),

      Dev = entry.Dev,
      Ino = entry.Ino,
      Mode = entry.Mode,
      Uid = entry.Uid,
      Gid = entry.Gid,
      FileSize = entry.FileSize,

      Id = GitOidMapper.ToNative(entry.Id),

      Flags = entry.Flags,
      FlagsExtended = entry.FlagsExtended,

      Path = entry.Path,
    };
  }
}
