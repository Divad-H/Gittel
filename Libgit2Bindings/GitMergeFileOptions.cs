namespace Libgit2Bindings;

/// <summary>
/// Options for merging a file
/// </summary>
public sealed record GitMergeFileOptions
{
  /// <summary>
  /// Label for the ancestor file side of the conflict which will be prepended
	/// to labels in diff3-format merge files.
  /// </summary>
  public string? AncestorLabel { get; init; }
  /// <summary>
  /// Label for our file side of the conflict which will be prepended
	/// to labels in merge files.
  /// </summary>
  public string? OurLabel { get; init; }
  /// <summary>
  /// Label for their file side of the conflict which will be prepended
	/// to labels in merge files.
  /// </summary>
  public string? TheirLabel { get; init; }
  /// <summary>
  /// The file to favor in region conflicts.
  /// </summary>
  public MergeFileFavor Favor { get; init; }
  /// <summary>
  /// See <see cref="MergeFileFlags"/>
  /// </summary>
  public MergeFileFlags Flags { get; init; }
  /// <summary>
  /// The size of conflict markers (eg, "<<<<<<<").  Default is 7.
  /// </summary>
  public UInt16 MarkerSize { get; init; } = 7;
}
