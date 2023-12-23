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
  /// Transform a diff marking file renames, copies, etc.
  /// </summary>
  /// <remarks>
  /// This modifies a diff in place, replacing old entries that look like renames or copies 
  /// with new entries reflecting those changes. This also will, if requested, break modified 
  /// files into add/remove pairs if the amount of change is above a threshold.
  /// </remarks>
  /// <param name="options">Control how detection should be run, null for defaults</param>
  void FindSimilar(GitDiffFindOptions? options = null);
}
