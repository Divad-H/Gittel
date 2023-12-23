namespace Libgit2Bindings;

/// <summary>
/// When producing a binary diff, the binary data returned will be
/// either the deflated full("literal") contents of the file, or
/// the deflated binary delta between the two sides(whichever is
/// smaller).
/// </summary>
public enum GitDiffBinaryType
{
  /// <summary>
  /// There is no binary delta.
  /// </summary>
  None = 0,
  /// <summary>
  /// The binary data is the literal contents of the file.
  /// </summary>
  Literal = 1,
  /// <summary>
  /// The binary data is the delta from one side to the other.
  /// </summary>
  Delta = 2,
}

public readonly ref struct GitDiffBinaryFile
{
  /// <summary>
  /// The type of binary data for this file.
  /// </summary>
  public GitDiffBinaryType Type { get; init; }

  /// <summary>
  /// The binary data, deflated.
  /// </summary>
  public ReadOnlySpan<byte> DeflatedData { get; init; }

  /// <summary>
  /// The length of the binary data after inflation.
  /// </summary>
  public UInt64 InflatedLength { get; init; }
}

/// <summary>
///  Structure describing the binary contents of a diff.
/// <para/>
/// A binary file / delta is a file (or pair) for which no text diffs should be generated.
/// A diff can contain delta entries that are binary, but no diff content will be output 
/// for those files. There is a base heuristic for binary detection and you can further 
/// tune the behavior with git attributes or diff flags and option settings.
/// </summary>
public readonly ref struct GitDiffBinary
{
  /// <summary>
  /// Whether there is data in this binary structure or not. If this is true, 
  /// then this was produced and included binary content. If this is false then
  /// this was generated knowing only that a binary file changed but without 
  /// providing the data, probably from a patch that said `Binary files a/file.txt 
  /// and b/file.txt differ`.
  /// </summary>
  public bool ContainsData { get; init; }

  /// <summary>
  /// The contents of the old file. 
  /// </summary>
  public readonly GitDiffBinaryFile OldFile { get; init; }

  /// <summary>
  /// The contents of the new file. 
  /// </summary>
  public readonly GitDiffBinaryFile NewFile { get; init; }
}
