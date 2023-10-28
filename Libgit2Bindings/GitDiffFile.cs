namespace Libgit2Bindings;

/// <summary>
/// Flags for the delta object and the file objects on each side.
/// <para/>
/// These flags are used for both the `flags` value of the <see cref="GitDiffDelta"/>
/// and the flags for the `git_diff_file` objects representing the old and
/// new sides of the delta.Values outside of this public range should be
/// considered reserved for internal or future use.
/// </summary>
[Flags]
public enum GitDiffFlags
{
  /// <summary>
  /// file(s) treated as binary data
  /// </summary>
  Binary = 1 << 0,
  /// <summary>
  /// file(s) treated as text data
  /// </summary>
  NotBinary = 1 << 1,
  /// <summary>
  /// `id` value is known correct
  /// </summary>
  ValidId = 1 << 2,
  /// <summary>
  /// file exists at this side of the delta
  /// </summary>
  Exists = 1 << 3,
  /// <summary>
  /// file size value is known correct
  /// </summary>
  ValidSize = 1 << 4
}


/// <summary>
/// Valid modes for index and tree entries.
/// </summary>
public enum GitFilemode : UInt16
{
  Unreadable = 0,
  Tree = 16384,
  Blob = 33188,
  BlobExecutable = 33261,
  Link = 40960,
  Commit = 57344
}

/// <summary>
/// Description of one side of a delta.
/// <para>
/// Although this is called a "file", it could represent a file, a symbolic
/// link, a submodule commit id, or even a tree (although that only if you
/// are tracking type changes or ignored/untracked directories).
/// </para>
/// </summary>
public record class GitDiffFile
{
  /// <summary>
  /// The <see cref="GitOid"/> of the item. If the entry represents an
  /// absent side of a diff(e.g.the `OldFile` of a `GIT_DELTA_ADDED` delta),
  /// then the oid will be zeroes.
  /// </summary>
  public required GitOid Oid { get; init; }
  /// <summary>
  /// The path to the entry relative to the working directory of the repository.
  /// </summary>
  public required string Path { get; init; }
  /// <summary>
  /// The size of the entry in bytes.
  /// </summary>
  public required UInt64 Size { get; init; }
  /// <summary>
  /// A combination of the <see cref="GitDiffFlags"/> types
  /// </summary>
  public required GitDiffFlags Flags { get; init; }

  /// <summary>
  /// Roughly, the stat() `st_mode` value for the item.
  /// </summary>
  public GitFilemode Mode { get; init; }

  /// <summary>
  /// Represents the known length of the `id` field, when
  /// converted to a hex string.  It is generally <see cref="GitOid.HexSize"/>, unless this
  /// delta was created from reading a patch file, in which case it may be
  /// abbreviated to something reasonable, like 7 characters.
  /// </summary>
  public UInt16 IdAbbrev { get; init; }
}
