namespace Libgit2Bindings;

/// <summary>
/// Holds blame results
/// </summary>
public interface IGitBlame : IDisposable
{
  /// <summary>
  /// Gets the blame hunk at the given index.
  /// </summary>
  /// <param name="index">index of the hunk to retrieve</param>
  /// <returns>the hunk at the given index, or null on error</returns>
  GitBlameHunk? GetHunkByIndex(UInt32 index);

  /// <summary>
  /// Gets the hunk that relates to the given line number in the newest commit.
  /// </summary>
  /// <param name="lineNumber">the (1-based) line number to find a hunk for</param>
  /// <returns>he hunk that contains the given line, or null on error</returns>
  GitBlameHunk? GetHunkByLine(UInt64 lineNumber);

  /// <summary>
  /// Gets the number of hunks in the blame.
  /// </summary>
  /// <returns>The number of hunks</returns>
  UInt32 GetHunkCount();

  /// <summary>
  /// Get blame data for a file that has been modified in memory. 
  /// Original blame is a pre-calculated blame for the in-odb history of the file. 
  /// This means that once a file blame is completed (which can be expensive), 
  /// updating the buffer blame is very fast.
  /// </summary>
  /// <remarks>
  /// Lines that differ between the buffer and the committed version are marked as 
  /// having a zero OID for their <see cref="GitBlameHunk.FinalCommitId"/>.
  /// </remarks>
  /// <param name="buffer">the (possibly) modified contents of the file</param>
  /// <returns>the resulting blame data</returns>
  IGitBlame BlameBuffer(byte[] buffer);
}
