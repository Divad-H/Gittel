using libgit2;
using System.Buffers.Text;
using System.Collections.Generic;
using System;

namespace Libgit2Bindings;

/// <summary>
///  * Flags for <see cref="MergeOptions"/>. A combination of these flags can be
/// passed in via the <see cref="MergeOptions.Flags"/> value.
/// </summary>
[Flags]
public enum GitMergeFlags
{
  /// <summary>
  /// Detect renames that occur between the common ancestor and the "ours"
	/// side or the common ancestor and the "theirs" side.  This will enable
	/// the ability to merge between a modified and renamed file.
  /// </summary>
  FindRenames = (1 << 0),
  /// <summary>
  /// If a conflict occurs, exit immediately instead of attempting to
	/// continue resolving conflicts.  The merge operation will fail with
	/// GIT_EMERGECONFLICT and no index will be returned.
  /// </summary>
  FailOnConflict = (1 << 1),
  /// <summary>
  /// Do not write the REUC extension on the generated index
  /// </summary>
  SkipReuc = (1 << 2),
  /// <summary>
  /// If the commits being merged have multiple merge bases, do not build
	/// a recursive merge base (by merging the multiple merge bases),
	/// instead simply use the first base. This flag provides a similar
	/// merge base to `git-merge-resolve`.
  /// </summary>
  NoRecursive = (1 << 3),
  /// <summary>
  /// Treat this merge as if it is to produce the virtual base
	/// of a recursive merge. This will ensure that there are
	/// no conflicts, any conflicting regions will keep conflict
	/// markers in the merge result.
  /// </summary>
  VirtualBase = (1 << 4),
}

public enum MergeFileFavor
{
  /// <summary>
  /// When a region of a file is changed in both branches, a conflict
  /// will be recorded in the index so that `git_checkout` can produce
  /// a merge file with conflict markers in the working directory.
  /// This is the default.
  /// </summary>
  Normal = 0,

  /// <summary>
  /// When a region of a file is changed in both branches, the file
  /// created in the index will contain the "ours" side of any conflicting
  /// region.  The index will not record a conflict.
  /// </summary>
  Ours = 1,

  /// <summary>
  /// When a region of a file is changed in both branches, the file
  /// created in the index will contain the "theirs" side of any conflicting
  /// region.  The index will not record a conflict.
  /// </summary>
  Theirs = 2,

  /// <summary>
  /// When a region of a file is changed in both branches, the file
  /// created in the index will contain each unique line from each side,
  /// which has the result of combining both files.  The index will not
  /// record a conflict.
  /// </summary>
  Union = 3
}

/// <summary>
/// File merging flags
/// </summary>
[Flags]
public enum MergeFileFlags
{
  /// <summary> Defaults </summary>
  Default = 0,

  /// <summary> Create standard conflicted merge files </summary>
  StyleMerge = (1 << 0),

  /// <summary> Create diff3-style files </summary>
  StyleDiff3 = (1 << 1),

  /// <summary> Condense non-alphanumeric regions for simplified diff file </summary>
  SimplifyAlnum = (1 << 2),

  /// <summary> Ignore all whitespace </summary>
  IgnoreWhitespace = (1 << 3),

  /// <summary> Ignore changes in amount of whitespace </summary>
  IgnoreWhitespaceChange = (1 << 4),

  /// <summary> Ignore whitespace at end of line </summary>
  IgnoreWhitespaceEol = (1 << 5),

  /// <summary> Use the "patience diff" algorithm </summary>
  DiffPatience = (1 << 6),

  /// <summary> Take extra time to find minimal diff </summary>
  DiffMinimal = (1 << 7),

  /// <summary> Create zdiff3 ("zealous diff3")-style files </summary>
  StyleZdiff3 = (1 << 8),

  /// <summary>
	/// Do not produce file conflicts when common regions have
	/// changed; keep the conflict markers in the file and accept
	/// that as the merge result.
	/// </summary>
  AcceptConflict = (1 << 9)
}

public interface ISimilarityMetric
{
  int Similarity(object signatureA, object signatureB);

  object ObjectHashSignatureForFile(GitDiffFile file, string path);
  object ObjectHashSignatureForBuffer(GitDiffFile file, byte[] buffer);
}

public interface ISimilarityMetric<T> where T : notnull
{
  int Similarity(object signatureA, object signatureB)
  {
    return Similarity((T)signatureA, (T)signatureB);
  }

  int Similarity(T signatureA, T signatureB);

  object ObjectHashSignatureForFile(GitDiffFile file, string path)
  {
    return HashSignatureForFile(file, path);
  }

  T HashSignatureForFile(GitDiffFile file, string path);

  object ObjectHashSignatureForBuffer(GitDiffFile file, byte[] buffer)
  {
    return HashSignatureForBuffer(file, buffer);
  }

  T HashSignatureForBuffer(GitDiffFile file, byte[] buffer);
}

/// <summary>
/// Merging options
/// </summary>
public record MergeOptions
{
  /// <summary>
  /// <see cref="GitMergeFlags"/>
  /// </summary>
  public GitMergeFlags Flags { get; init; } = GitMergeFlags.FindRenames;

  /// <summary>
  /// Similarity to consider a file renamed (default 50).  If
	/// `GIT_MERGE_FIND_RENAMES` is enabled, added files will be compared
  /// with deleted files to determine their similarity.Files that are
	/// more similar than the rename threshold (percentage-wise) will be
	/// treated as a rename.
  /// </summary>
  public UInt32 RenameThreshold { get; init; } = 50;

  /// <summary>
  /// Maximum similarity sources to examine for renames (default 200).
	/// If the number of rename candidates(add / delete pairs) is greater
  /// than this value, inexact rename detection is aborted.
  /// </summary>
  /// <remarks>
  /// This setting overrides the `merge.renameLimit` configuration value.
  /// </remarks>
  public UInt32 TargetLimit { get; init; } = 200;

  /// <summary>
  /// The `metric` option allows you to plug in a custom similarity metric.
	/// <para/>
	/// Set it to NULL to use the default internal metric.
  /// <para/>
  /// The default metric is based on sampling hashes of ranges of data in
	/// the file, which is a pretty good similarity approximation that should
  /// work fairly well for both text and binary data while still being
	/// pretty fast with a fixed memory overhead.
  /// </summary>
  public ISimilarityMetric? SimilarityMetric { get; init; } = null;

  /// <summary>
  /// Maximum number of times to merge common ancestors to build a
	/// virtual merge base when faced with criss-cross merges.When this
  /// limit is reached, the next ancestor will simply be used instead of
  /// attempting to merge it.The default is unlimited.
  /// </summary>
  public UInt32 RecursionLimit { get; init; } = 0;

  /// <summary>
  ///  Default merge driver to be used when both sides of a merge have
	/// changed.The default is the `text` driver.
  /// </summary>
  public string? DefaultDriver { get; init; } = null;

  /// <summary>
  /// <see cref="Libgit2Bindings.MergeFileFavor"/>
  /// Options for handling conflicting content, to be used with the standard
	/// (`text`) merge driver.
  /// </summary>
  public MergeFileFavor MergeFileFavor { get; init; } = MergeFileFavor.Normal;

  /// <summary>
  /// <see cref="MergeFileFlags"/>
  /// </summary>
  public MergeFileFlags FileFlags { get; init; } = MergeFileFlags.Default;
}
