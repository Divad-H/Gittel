namespace Libgit2Bindings;

public sealed record GitDiffStats
{
  /// <summary>
  /// The total number of deletions in a diff.
  /// </summary>
  public UInt64 Deletions { get; init; }

  /// <summary>
  /// The total number of files changed in a diff.
  /// </summary>
  public UInt64 FilesChanged { get; init; }

  /// <summary>
  /// The total number of insertions in a diff.
  /// </summary>
  public UInt64 Insertions { get; init; }
}
