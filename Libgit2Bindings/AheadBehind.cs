namespace Libgit2Bindings;

/// <summary>
/// Represents the number of commits that a branch is ahead and behind another.
/// </summary>
public sealed record AheadBehind
{
  /// <summary>
  /// number of unique commits in the branch
  /// </summary>
  public UInt64 Ahead { get; init; }
  /// <summary>
  /// number of unique commits in the other branch
  /// </summary>
  public UInt64 Behind { get; init; }
}
