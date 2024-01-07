namespace Libgit2Bindings;

/// <summary>
/// The file inputs to <see cref="IGitRepository.MergeFile"/>. Callers should populate the 
/// record with descriptions of the files in each side of the conflict for use in producing 
/// the merge file.
/// </summary>
public sealed record GitMergeFileInput
{
  /// <summary>
  /// The contents of the file
  /// </summary>
  public required byte[] FileContent { get; init; }
  /// <summary>
  /// File name of the conflicted file, or null to not merge the path. 
  /// </summary>
  public string? Path { get; init; }
  /// <summary>
  /// File mode of the conflicted file, or `0` to not merge the mode. 
  /// </summary>
  public UInt32 Mode { get; init; }
}
