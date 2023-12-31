using libgit2;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System.Drawing;
using System;

namespace Libgit2Bindings;

/// <summary>
/// Time structure used in a git index entry
/// </summary>
public sealed record GitIndexTime
{
  public Int32 Seconds { get; init; }
  /// <summary>
  /// nsec should not be stored as time_t compatible
  /// </summary>
  public UInt32 Nanoseconds { get; init; }
}

/// <summary>
///  * In-memory representation of a file entry in the index.
/// <para/>
/// This is a public structure that represents a file entry in the index.
/// The meaning of the fields corresponds to core Git's documentation (in
/// "Documentation/technical/index-format.txt").
/// <para/>
/// The `flags` field consists of a number of bit fields which can be
/// accessed via the first set of `GIT_INDEX_ENTRY_...` bitmasks below.
/// These flags are all read from and persisted to disk.
/// <para/>
/// The `flags_extended` field also has a number of bit fields which can be
/// accessed via the later `GIT_INDEX_ENTRY_...` bitmasks.  Some of
/// these flags are read from and written to disk, but some are set aside
/// for in-memory only reference.
/// <para/>
/// Note that the time and size fields are truncated to 32 bits.This
/// is enough to detect changes, which is enough for the index to
/// function as a cache, but it should not be taken as an authoritative
/// </summary>
public sealed record GitIndexEntry
{
  public required GitIndexTime CTime { get; init; }
  public required GitIndexTime MTime { get; init; }

  public UInt32 Dev { get; init; }
  public UInt32 Ino { get; init; }
  public UInt32 Mode { get; init; }
  public UInt32 Uid { get; init; }
  public UInt32 Gid { get; init; }
  public UInt32 FileSize { get; init; }

  public required GitOid Id { get; init; }

  public UInt16 Flags { get; init; }
  public UInt16 FlagsExtended { get; init; }

  public required string Path { get; init; }
}

public static class GitIndexEntryExtensions
{
  const int StageMask = 0x3000;
  const int StageShift = 12;

  public static int GetStage(this GitIndexEntry entry)
  {
    return (entry.Flags & StageMask) >> StageShift;
  }

  public static bool IsConflict(this GitIndexEntry entry)
  {
    return entry.GetStage() > 0;
  }
}

/// <summary>
/// Representation of a conflict in the index.
/// </summary>
public sealed record ConflictEntries
{
  /// <summary>
  /// The ancestor version of the file.
  /// </summary>
  public GitIndexEntry? Ancestor { get; init; }
  /// <summary>
  /// The version of the file from 'our' branch.
  /// </summary>
  public GitIndexEntry? Our { get; init; }
  /// <summary>
  /// The version of the file from 'their' branch.
  /// </summary>
  public GitIndexEntry? Their { get; init; }
}
