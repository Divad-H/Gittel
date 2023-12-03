using System.Net.NetworkInformation;

namespace Libgit2Bindings;

/// <summary>
/// Structure describing a line (or data span) of a diff.
/// <para/>
/// A `line` is a range of characters inside a hunk.It could be a context
/// line(i.e. in both old and new versions), an added line(i.e.only in
/// the new version), or a removed line(i.e.only in the old version).
/// Unfortunately, we don't know anything about the encoding of data in the
/// file being diffed, so we cannot tell you much about the line content.
/// Line data will not be NUL-byte terminated, however, because it will be
/// just a span of bytes inside the larger file.
/// </summary>
public record GitDiffLine
{
  /// <summary>
  /// These values will be sent to the diff line callback along with the line
  /// </summary>
  public const char Context = ' ';
  public const char Addition = '+';
  public const char Deletion = '-';
  /// <summary>
  /// Both files have no LF at end
  /// </summary>
  public const char ContextEOFNL = '=';
  /// <summary>
  /// Old has no LF at end, new does
  /// </summary>
  public const char AdditionEOFNL = '>';
  /// <summary>
  /// Old has LF at end, new does not
  /// </summary>
  public const char DeletionEOFNL = '<';

  /// <summary>
  /// The following values will only be sent to a diff line callback when
	/// the content of a diff is being formatted through IGitDiff.Print().
  /// </summary>
  public const char FileHdr = 'F';
  public const char HunkHdr = 'H';
  public const char Binary = 'B';

  /// <summary>
  /// A value from the characters above
  /// </summary>
  public char Origin { get; init; }
  /// <summary>
  ///  Line number in old file or -1 for added line
  /// </summary>
  public int OldLineNumber { get; init; }
  /// <summary>
  /// Line number in new file or -1 for deleted line
  /// </summary>
  public int NewLineNumber { get; init; }
  /// <summary>
  /// Number of newline characters in content
  /// </summary>
  public int NumLines { get; init; }

  /// <summary>
  /// Offset in the original file to the content
  /// </summary>
  public Int64 ContentOffset { get; init; }

  /// <summary>
  /// Length of the content
  /// </summary>
  public UIntPtr ContentLength { get; init; }

  /// <summary>
  /// diff text
  /// </summary>
  public byte[] Content { get; init; } = Array.Empty<byte>();
}
