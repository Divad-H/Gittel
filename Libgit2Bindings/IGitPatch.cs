namespace Libgit2Bindings;

/// <summary>
///  The diff patch is used to store all the text diffs for a delta.
/// <para/>
/// You can easily loop over the content of patches and get information about them.
/// </summary>
public interface IGitPatch : IDisposable
{
  /// <summary>
  /// Get the delta associated with a patch.
  /// </summary>
  GitDiffDelta Delta { get; }

  /// <summary>
  /// Get the content of a patch as a single diff text.
  /// </summary>
  /// <returns>The content</returns>
  byte[] GetContent();

  /// <summary>
  /// Look up size of patch diff data in bytes
  /// </summary>
  /// <param name="includeContext">Include context lines in size if true</param>
  /// <param name="includeHunkHeaders">Include hunk header lines if true</param>
  /// <param name="includeFileHeaders">Include file header lines if true</param>
  /// <returns>The number of bytes of data</returns>
  UIntPtr RawSize(bool includeContext, bool includeHunkHeaders, bool includeFileHeaders);

  /// <summary>
  /// Get the number of lines in a hunk.
  /// </summary>
  /// <param name="hunkIdx">Index of the hunk</param>
  /// <returns>Number of lines in hunk</returns>
  int GetNumLinesInHunk(UIntPtr hunkIdx);

  /// <summary>
  /// Get the number of hunks in a patch.
  /// </summary>
  UIntPtr NumHunks { get; }

  /// <summary>
  /// Get the information about a hunk in a patch
  /// </summary>
  /// <remarks>
  /// Given a patch and a hunk index into the patch, this returns detailed information about that hunk.
  /// </remarks>
  /// <param name="hunkIdx">Input index of hunk to get information about</param>
  /// <param name="linesInHunk">Output number of lines in the hunk</param>
  /// <returns>The hunk</returns>
  GitDiffHunk GetHunk(UIntPtr hunkIdx, out UIntPtr linesInHunk);

  /// <summary>
  /// Get data about a line in a hunk of a patch.
  /// </summary>
  /// <remarks>
  /// Given a patch, a hunk index, and a line index in the hunk, this will return a lot of 
  /// details about that line. If you pass a hunk index larger than the number of hunks or 
  /// a line index larger than the number of lines in the hunk, this will throw.
  /// </remarks>
  /// <param name="hunkIdx">The index of the hunk</param>
  /// <param name="lineOfHunk">The index of the line in the hunk</param>
  /// <returns>The <see cref="GitDiffLine"/> data for this line</returns>
  GitDiffLine GetLineInHunk(UIntPtr hunkIdx, UIntPtr lineOfHunk);

  /// <summary>
  /// Get line counts of each type in a patch.
  /// </summary>
  /// <remarks>
  /// This helps imitate a diff --numstat type of output. For that purpose, you only need the 
  /// addedLines and deletedLines values, but we include the contextLines line count in case 
  /// you want the total number of lines of diff output that will be generated.
  /// </remarks>
  /// <returns>Line count stats</returns>
  (UIntPtr contextLines, UIntPtr addedLines, UIntPtr deletedLines) GetLineStats();

  /// <summary>
  /// Serialize the patch to text via callback.
  /// </summary>
  /// <param name="printCallback">
  /// Callback function to output lines of the patch. Will be called for file headers, 
  /// hunk headers, and diff lines.
  /// </param>
  void Print(IGitDiff.LineCallback printCallback);
}
