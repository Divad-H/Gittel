namespace Libgit2Bindings;

/// <summary>
/// Options for indexer configuration
/// </summary>
public sealed record GitIndexerOptions
{
  /// <summary>
  /// progress_cb function to call with progress information
  /// </summary>
  public IndexerProgressHandler? ProgressCallback { get; init; }

  /// <summary>
  ///  Do connectivity checks for the received pack
  /// </summary>
  public bool Verify { get; init; }
}
