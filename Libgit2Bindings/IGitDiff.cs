namespace Libgit2Bindings;

public interface IGitDiff : IDisposable
{
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
}
