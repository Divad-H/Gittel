namespace Libgit2Bindings;

/// <summary>
/// Flags to control the functionality of <see cref="IGitBlob.Filter"/>
/// </summary>
[Flags]
public enum GitBlobFilterFlags
{
  /// <summary>
  /// When set, filters will not be applied to binary files.
  /// </summary>
  CheckForBinary = 1 << 0,
  /// <summary>
  /// When set, filters will not load configuration from the
  /// system-wide `gitattributes` in `/etc` (or system equivalent).
  /// </summary>
  NoSystemAttributes = 1 << 1,
  /// <summary>
  /// When set, filters will be loaded from a `.gitattributes` file
  /// in the HEAD commit.
  /// </summary>
  AttributesFromHead = 1 << 2,
  /// <summary>
  /// When set, filters will be loaded from a `.gitattributes` file
  /// in the specified commit.
  /// </summary>
  AttributesFromCommit = 1 << 3,
}

/// <summary>
/// The options used when applying filter options to a file.
/// </summary>
public record GitBlobFilterOptions
{
  /// <summary>
  /// Flags to control the filtering process, <see cref="GitBlobFilterFlags"/>
  /// </summary>
  public GitBlobFilterFlags Flags { get; init; } = GitBlobFilterFlags.CheckForBinary;

  /// <summary>
  /// The commit to load attributes from, when
  /// <see cref="GitBlobFilterFlags.AttributesFromCommit"/> is specified.
  /// </summary>
  public GitOid? AttributesCommitId { get; init; }
}
