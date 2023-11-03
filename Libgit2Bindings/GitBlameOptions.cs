namespace Libgit2Bindings;

/// <summary>
/// Flags for indicating option behavior for git blame.
/// </summary>
[Flags]
public enum GitBlameFlags
{
  /// <summary>
  /// Normal blame, the default
  /// </summary>
  Normal = 0,

  /// <summary>
  /// Track lines that have moved within a file (like `git blame -M`).
	///
	/// This is not yet implemented and reserved for future use.
  /// </summary>
  TrackCopiesSameFile = 1 << 0,

  /// <summary>
  /// Track lines that have moved across files in the same commit
	/// (like `git blame -C`).
	///
	/// This is not yet implemented and reserved for future use.
  /// </summary>
  TrackCopiesSameCommitMoves = 1 << 1,

  /// <summary>
  /// Track lines that have been copied from another file that exists
	/// in the same commit (like `git blame -CC`).  Implies SAME_FILE.
	///
	/// This is not yet implemented and reserved for future use.
  /// </summary>
  TrackCopiesSameCommitCopies = 1 << 2,

  /// <summary>
  /// Track lines that have been copied from another file that exists in
	/// *any* commit (like `git blame -CCC`).  Implies SAME_COMMIT_COPIES.
	///
	/// This is not yet implemented and reserved for future use.
  /// </summary>
  TrackCopiesAnyCommitCopies = 1 << 3,

  /// <summary>
  ///  Restrict the search of commits to those reachable following only
	/// the first parents.
  /// </summary>
  FirstParent = 1 << 4,

  /// <summary>
  /// Use mailmap file to map author and committer names and email
	/// addresses to canonical real names and email addresses. The
	/// mailmap will be read from the working directory, or HEAD in a
	/// bare repository.
  /// </summary>
  UseMailmap = 1 << 5,

  /// <summary>
  /// Ignore whitespace differences
  /// </summary>
  IgnoreWhitespace = 1 << 6,
}

public record GitBlameOptions
{
  /// <summary>
  /// A combination of <see cref="GitBlameFlags"/>
  /// </summary>
  public GitBlameFlags Flags { get; init; } = GitBlameFlags.Normal;

  /// <summary>
  /// The lower bound on the number of alphanumeric characters that
	/// must be detected as moving/copying within a file for it to
	/// associate those lines with the parent commit.The default value
  /// is 20.
  /// </summary>
	/// <remarks>
	/// This value only takes effect if any of the `GitBlameFlags.TrackCopies*`
	/// flags are specified.
  /// </remarks>
  public UInt16 MinMatchCharacters { get; init; } = 20;

  /// <summary>
  /// The id of the newest commit to consider. The default is HEAD.
  /// </summary>
  public GitOid? NewestCommit { get; init; }

  /// <summary>
  /// The id of the oldest commit to consider. The default is the
  /// first commit encountered with a null parent.
  /// </summary>
  public GitOid? OldestCommit { get; init; }

  /// <summary>
  /// The first line in the file to blame.
	/// The default is 1 (line numbers start with 1).
  /// </summary>
  public UInt64 MinLine { get; init; }

  /// <summary>
  /// The last line in the file to blame.
	/// The default is the last line of the file.
  /// </summary>
  public UInt64 MaxLine { get; init; }
}
