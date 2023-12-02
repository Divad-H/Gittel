using libgit2;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Libgit2Bindings;

/// <summary>
/// Reference lookup strategy
/// <para/>
/// These behave like the --tags and --all options to git-describe,
/// namely they say to look for any reference in either refs/tags/ or
/// refs/ respectively.
/// </summary>
public enum GitDescribeStrategy
{
  /// <summary>
  /// Look for annotated tags.
  /// </summary>
  Default = 0,

  /// <summary>
  /// Look for any reference in refs/tags/
  /// </summary>
  Tags = 1,

  /// <summary>
  /// Look for any reference in refs/
  /// </summary>
  All = 2,
}

public record GitDescribeOptions
{
  /// <summary>
  /// Maximum number of tag candidates to consider when describing a commit.
  /// </summary>
  public UInt32 MaxCandidatesTags { get; init; } = 10;

  /// <summary>
  /// Defines which refs to be considered when describing a commit.
  /// </summary>
  public GitDescribeStrategy DescribeStrategy { get; init; } = GitDescribeStrategy.Default;

  /// <summary>
  /// Only consider tags matching the given pattern.
  /// </summary>
  public string? Pattern { get; init; } = null;

  /// <summary>
  /// When calculating the distance from the matching tag or
  /// reference, only walk down the first-parent ancestry.
  /// </summary>
  public bool OnlyFollowFirstParent { get; init; } = false;

  /// <summary>
  /// If no matching tag or reference is found, the describe
	/// operation would normally fail.If this option is set, it
  /// will instead fall back to showing the full id of the
  /// </summary>
  public bool ShowCommitOidAsFallback { get; init; } = false;
}
