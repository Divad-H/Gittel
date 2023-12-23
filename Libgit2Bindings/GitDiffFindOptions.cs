namespace Libgit2Bindings;

/// <summary>
/// Flags to control the behavior of diff rename/copy detection.
/// </summary>
[Flags]
public enum GitDiffFindFlags
{
  /// <summary>
  /// Obey `diff.renames`. Overridden by any other flag.
  /// </summary>
  ByConfig = 0,
  /// <summary>
  /// Look for renames? (`--find-renames`) 
  /// </summary>
  Renames = 1 << 0,
  /// <summary>
  /// Consider old side of MODIFIED for renames? (`--break-rewrites=N`)
  /// </summary>
  RenamesFromRewrites = 1 << 1,
  /// <summary>
  /// Look for copies? (a la `--find-copies`).
  /// </summary>
  Copies = 1 << 2,
  /// <summary>
  /// Consider UNMODIFIED as copy sources? (`--find-copies-harder`).
  /// </summary>
  /// <remarks>
  /// For this to work correctly, use <see cref="GitDiffOptionFlags.IncludeUnmodified"/> when
	/// the initial diff is being generated.
  /// </remarks>
  CopiesFromUnmodified = 1 << 3,
  /// <summary>
  ///  Mark significant rewrites for split (`--break-rewrites=/M`)
  /// </summary>
  Rewrites = 1 << 4,
  /// <summary>
  /// Actually split large rewrites into delete/add pairs
  /// </summary>
  BreakRewrites = 1 << 5,
  /// <summary>
  /// Mark rewrites for split and break into delete/add pairs
  /// </summary>
  AndBreakRewrites = Rewrites | BreakRewrites,
  /// <summary>
  /// Find renames/copies for UNTRACKED items in working directory.
  /// </summary>
  /// <remarks>
  /// For this to work correctly, use <see cref="GitDiffOptionFlags.IncludeUntracked"/> when the
	/// initial diff is being generated (and obviously the diff must
	/// be against the working directory for this to make sense).
  /// </remarks>
  ForUntracked = 1 << 6,
  /// <summary>
  /// Turn on all finding features.
  /// </summary>
  All = 0x0ff,

  /// <summary>
  /// Measure similarity ignoring leading whitespace (default)
  /// </summary>
  IgnoreLeadingWhitespace = 0,
  /// <summary>
  /// Measure similarity ignoring all whitespace
  /// </summary>
  IgnoreWhitespace = 1 << 12,
  /// <summary>
  /// Measure similarity including all data
  /// </summary>
  DontIgnoreWhitespace = 1 << 13,
  /// <summary>
  /// Measure similarity only by comparing SHAs (fast and cheap)
  /// </summary>
  ExactMatchOnly = 1 << 14,
  /// <summary>
  /// Do not break rewrites unless they contribute to a rename.
  /// </summary>
  /// <remarks>
  /// Normally, <see cref="BreakRewrites"/> will measure the self-
  /// similarity of modified files and split the ones that have changed a
  /// lot into a DELETE / ADD pair.  Then the sides of that pair will be
  /// considered candidates for rename and copy detection.
  /// <para/>
  /// If you add this flag in and the split pair is *not* used for an
  /// actual rename or copy, then the modified record will be restored to
  /// a regular MODIFIED record instead of being split.
  /// </remarks>
  BreakRewritesForRenamesOnly = 1 << 15,
  /// <summary>
  /// Remove any UNMODIFIED deltas after find_similar is done.
  /// </summary>
  /// <remarks>
  /// Using <see cref="CopiesFromUnmodified"/> to emulate the
	/// --find-copies-harder behavior requires building a diff with the
	/// <see cref="GitDiffOptionFlags.IncludeUnmodified"/> flag.  If you do not want UNMODIFIED
	/// records in the final result, pass this flag to have them removed.
  /// </remarks>
  RemoveUnmodified = 1 << 16,
}

public sealed record GitDiffFindOptions
{
  /// <summary>
  /// Combination of <see cref="GitDiffFindFlags"/> values (default <see cref="GitDiffFindFlags.ByConfig"/>).
	/// NOTE: if you don't explicitly set this, `diff.renames` could be set
	/// to false, resulting in <see cref="IGitDiff.FindSimilar(GitDiffFindOptions?)"/> doing nothing.
  /// </summary>
  public GitDiffFindFlags Flags { get; init; } = 0;

  /// <summary>
  /// Threshold above which similar files will be considered renames.
	/// This is equivalent to the -M option.Defaults to 50.
  /// </summary>
  public UInt16 RenameThreshold { get; init; } = 50;

  /// <summary>
  /// Threshold below which similar files will be eligible to be a rename source.
  /// This is equivalent to the first part of the -B option.Defaults to 50.
  /// </summary>
  public UInt16 RenameFromRewriteThreshold { get; init; } = 50;

  /// <summary>
  /// Threshold above which similar files will be considered copies.
	/// This is equivalent to the -C option.Defaults to 50.
  /// </summary>
  public UInt16 CopyThreshold { get; init; } = 50;

  /// <summary>
  /// Threshold below which similar files will be split into a delete/add pair.
	/// This is equivalent to the last part of the -B option.Defaults to 60.
  /// </summary>
  public UInt16 BreakRewriteThreshold { get; init; } = 60;

  /// <summary>
  /// Maximum number of matches to consider for a particular file.
	/// <para/>
	/// This is a little different from the `-l` option from Git because we
  /// will still process up to this many matches before abandoning the search.
	/// Defaults to 1000.
  /// </summary>
  public UInt64 RenameLimit { get; init; } = 1000;

  /// <summary>
  /// The option allows you to plug in a custom similarity metric.
	/// <para/>
	/// Set it to null to use the default internal metric.
  /// <para/>
  /// The default metric is based on sampling hashes of ranges of data in
	/// the file, which is a pretty good similarity approximation that should
  /// work fairly well for both text and binary data while still being
	/// pretty fast with a fixed memory overhead.
  /// </summary>
  public ISimilarityMetric? SimilarityMetric { get; init; } = null;
}
