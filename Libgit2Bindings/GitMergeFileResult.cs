namespace Libgit2Bindings;

/// <summary>
/// Information about file-level merging
/// </summary>
public sealed record GitMergeFileResult
{
  /// <summary>
  /// True if the output was automerged, false if the output contains conflict markers.
  /// </summary>
  public required bool Automergeable { get; init; }
  /// <summary>
  /// The path that the resultant merge file should use, or null if a filename conflict would occur.
  /// </summary>
  public string? Path { get; init; }
  /// <summary>
  /// The mode that the resultant merge file should use. 
  /// </summary>
  public UInt32 Mode { get; init; }
  /// <summary>
  /// The contents of the merge. 
  /// </summary>
  public required byte[] Content { get; init; }
}
