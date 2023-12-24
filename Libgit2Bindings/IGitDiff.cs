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
