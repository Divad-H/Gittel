namespace Libgit2Bindings;

/// <summary>
/// Structure describing a hunk of a diff.
/// <para/>
/// A `hunk` is a span of modified lines in a delta along with some stable
/// surrounding context.You can configure the amount of context and other
/// properties of how hunks are generated.Each hunk also comes with a
/// header that described where it starts and ends in both the old and new
/// versions in the delta.
/// </summary>
public readonly ref struct GitDiffHunk
{
  public GitDiffHunk()
  { }

  /// <summary>
  /// Starting line number in OldFile
  /// </summary>
  public int OldStart { get; init; }
  /// <summary>
  /// Number of lines in OldFile
  /// </summary>
  public int OldLines { get; init; }
  /// <summary>
  /// Starting line number in NewFile
  /// </summary>
  public int NewStart { get; init; }
  /// <summary>
  /// Number of lines in NewFile
  /// </summary>
  public int NewLines { get; init; }
  /// <summary>
  /// Header text
  /// </summary>
  public ReadOnlySpan<byte> Header { get; init; } = [];
}
