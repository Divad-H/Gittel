namespace Libgit2Bindings;

public interface IGitDiff : IDisposable
{
  public delegate GitOperationContinuation FileCallback(
     GitDiffDelta delta, float progress);
  public delegate GitOperationContinuation BinaryCallback(
     GitDiffDelta delta, GitDiffBinary binary);
  public delegate GitOperationContinuation HunkCallback(
     GitDiffDelta delta, GitDiffHunk hunk);
  public delegate GitOperationContinuation LineCallback(
     GitDiffDelta delta, GitDiffHunk hunk, GitDiffLine line);

  /// <summary>
  /// Return the diff delta for an entry in the diff list.
  /// </summary>
  /// <param name="index">Index into diff list</param>
  /// <returns>The <see cref="GitDiffDelta"/> or null if index is out of range</returns>
  GitDiffDelta? GetDelta(UInt64 index);

  /// <summary>
  /// Query how many diff records are there in a diff.
  /// </summary>
  /// <returns>Count of number of deltas in the list</returns>
  UInt64 GetNumDeltas();

  /// <summary>
  /// Query how many diff deltas are there in a diff filtered by type.
  /// </summary>
  /// <remarks>
  /// This works just like <see cref="GetNumDeltas"/> with an extra parameter 
  /// that is a <see cref="GitDeltaType"/> and returns just the count of how 
  /// many deltas match that particular type.
  /// </remarks>
  /// <param name="type">A <see cref="GitDeltaType"/> value to filter the count</param>
  /// <returns>Count of number of deltas matching delta type</returns>
  UInt64 GetNumDeltasOfType(GitDeltaType type);

  /// <summary>
  /// Accumulate diff statistics for all patches.
  /// </summary>
  /// <returns>The Statistics</returns>
  GitDiffStats GetStats();

  /// <summary>
  /// Print diff statistics.
  /// </summary>
  /// <param name="format">Formatting option.</param>
  /// <param name="width">
  /// Target width for output (only affects <see cref="GitDiffStatsFormatOptions.Full"/>)
  /// </param>
  /// <returns>formatted diff statistics</returns>
  string GetStatsFormatted(GitDiffStatsFormatOptions format, UInt32 width);

  /// <summary>
  /// Check if deltas are sorted case sensitively or insensitively.
  /// </summary>
  /// <returns>false if case sensitive, true if case is ignored</returns>
  bool IsSortedCaseInsensitively();

  /// <summary>
  /// Transform a diff marking file renames, copies, etc.
  /// </summary>
  /// <remarks>
  /// This modifies a diff in place, replacing old entries that look like renames or copies 
  /// with new entries reflecting those changes. This also will, if requested, break modified 
  /// files into add/remove pairs if the amount of change is above a threshold.
  /// </remarks>
  /// <param name="options">Control how detection should be run, null for defaults</param>
  void FindSimilar(GitDiffFindOptions? options = null);

  /// <summary>
  /// Merge one diff into another.
  /// </summary>
  /// <remarks>
  /// This merges items from the "from" list into the this list. 
  /// The resulting diff will have all items that appear in either list. 
  /// If an item appears in both lists, then it will be "merged" to appear 
  /// as if the old version was from this list and the new version is from 
  /// the "from" list (with the exception that if the item has a pending 
  /// DELETE in the middle, then it will show as deleted).
  /// </remarks>
  /// <param name="from">Diff to merge.</param>
  void Merge(IGitDiff from);

  /// <summary>
  /// Loop over all deltas in a diff issuing callbacks.
  /// </summary>
  /// <remarks>
  /// This will iterate through all of the files described in a diff. 
  /// You should provide a file callback to learn about each file.
  /// <para/>
  /// The "hunk" and "line" callbacks are optional, and the text diff of 
  /// the files will only be calculated if they are not null.Of course, 
  /// these callbacks will not be invoked for binary files on the diff or 
  /// for files whose only changed is a file mode change.
  /// <para/>
  /// Returning a non-zero value from any of the callbacks will terminate 
  /// the iteration and return the value to the user.
  /// </remarks>
  /// <param name="fileCallback">Callback function to make per file in the diff.</param>
  /// <param name="binaryCallback">Optional callback to make for binary files.</param>
  /// <param name="hunkCallback">
  /// Optional callback to make per hunk of text diff. 
  /// This callback is called to describe a range of lines in the diff. 
  /// It will not be issued for binary files.
  /// </param>
  /// <param name="lineCallback">
  /// Optional callback to make per line of diff text. 
  /// This same callback will be made for context lines, added, and removed lines, 
  /// and even for a deleted trailing newline.
  /// </param>
  void ForEach(
    FileCallback? fileCallback = null,
    BinaryCallback? binaryCallback = null,
    HunkCallback? hunkCallback = null,
    LineCallback? lineCallback = null);

  /// <summary>
  /// Produce the complete formatted text output from a diff into a buffer.
  /// </summary>
  /// <param name="format">
  /// A <see cref="GitDiffFormatOptions"/> value to pick the text format.
  /// </param>
  /// <returns>the diff text</returns>
  byte[] ToBuffer(GitDiffFormatOptions format);

  /// <summary>
  /// Iterate over a diff generating formatted text output
  /// </summary>
  /// <param name="format">
  /// A <see cref="GitDiffFormatOptions"/> value to pick the text format.
  /// </param>
  /// <param name="printCallback">Callback to make per line of diff text.</param>
  void Print(GitDiffFormatOptions format, LineCallback printCallback);

  /// <summary>
  /// Calculate the patch ID for the given patch.
  /// </summary>
  /// <remarks>
  /// Calculate a stable patch ID for the given patch by summing the hash of the file 
  /// diffs, ignoring whitespace and line numbers. This can be used to derive whether 
  /// two diffs are the same with a high probability.
  /// <para/>
  /// Currently, this function only calculates stable patch IDs, as defined in 
  /// git-patch-id(1), and should in fact generate the same IDs as the upstream git 
  /// project does.
  /// </remarks>
  /// <param name="options">
  /// Options for how to calculate the patch ID. This is intended for future changes, 
  /// as currently no options are available.
  /// </param>
  /// <returns></returns>
  GitOid PatchId(GitDiffPatchIdOptions? options = null);

  /// <summary>
  /// Return a patch for an entry in the diff list.
  /// </summary>
  /// <remarks>
  /// The <see cref="IGitPatch"/> is a newly created object contains the text diffs for the delta. 
  /// You can use the patch object to loop over all the hunks and lines in the diff of the one delta.
  /// <para/>
  /// For an unchanged file or a binary file, no <see cref="IGitPatch"/> will be created, null will be
  /// returned, and the binary flag will be set true in the <see cref="GitDiffDelta"/> structure.
  /// </remarks>
  /// <param name="index">Index into diff list</param>
  /// <returns>the delta patch object</returns>
  IGitPatch? ToPatch(UIntPtr index);
}


/// <summary>
/// Formatting options for diff stats
/// </summary>
public enum GitDiffStatsFormatOptions
{
  /// <summary>
  /// No stats
  /// </summary>
  None = 0,
  /// <summary>
  /// Full statistics, equivalent of `--stat`
  /// </summary>
  Full = 1 << 0,
  /// <summary>
  /// Short statistics, equivalent of `--shortstat`
  /// </summary>
  Short = 1 << 1,
  /// <summary>
  /// Number statistics, equivalent of `--numstat`
  /// </summary>
  Number = 1 << 2,
  /// <summary>
  /// Extended header information such as creations, renames and mode changes, equivalent of `--summary`
  /// </summary>
  IncludeSummary = 1 << 3,
}

/// <summary>
/// Possible output formats for diff data
/// </summary>
public enum GitDiffFormatOptions
{
  /// <summary>
  /// full git diff
  /// </summary>
  Patch = 1,
  /// <summary>
  /// just the file headers of patch
  /// </summary>
  PatchHeader = 2,
  /// <summary>
  /// like git diff --raw
  /// </summary>
  Raw = 3,
  /// <summary>
  /// like git diff --name-only
  /// </summary>
  NameOnly = 4,
  /// <summary>
  /// like git diff --name-status
  /// </summary>
  NameStatus = 5,
  /// <summary>
  /// git diff as used by git patch-id
  /// </summary>
  PatchId = 6
}
