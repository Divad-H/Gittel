namespace Libgit2Bindings;

/// <summary>
/// A git indexer object
/// </summary>
public interface IGitIndexer : IDisposable
{
  /// <summary>
  /// Get the unique name for the resulting packfile.
  /// </summary>
  /// <remarks>
  /// The packfile's name is derived from the packfile's content. 
  /// This is only correct after the index has been finalized.
  /// </remarks>
  string Name { get; }

  /// <summary>
  /// Finalize the pack and index
  /// </summary>
  /// <remarks>
  /// Resolve any pending deltas and write out the index file
  /// </remarks>
  /// <returns>Stats</returns>
  GitIndexerProgress Commit();

  /// <summary>
  /// Add data to the indexer
  /// </summary>
  /// <param name="data">the data to add</param>
  /// <returns>Stats</returns>
  GitIndexerProgress Append(byte[] data);
}
