namespace Libgit2Bindings;

public interface IGitCommit : IDisposable
{
  /// <summary>
  /// Amend an existing commit by replacing only non-null values.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This creates a new commit that is exactly the same as the old commit, 
  /// except that any non-null values will be updated. 
  /// The new commit has the same parents as the old commit.
  /// </para>
  /// </remarks>
  /// <param name="updateRef"><see cref="IGitRepository.CreateCommit"/></param>
  /// <param name="author">Same as <see cref="IGitRepository.CreateCommit"/> but can be null</param>
  /// <param name="committer">Same as <see cref="IGitRepository.CreateCommit"/> but can be null</param>
  /// <param name="message">Same as <see cref="IGitRepository.CreateCommit"/> but can be null</param>
  /// <param name="tree">Same as <see cref="IGitRepository.CreateCommit"/> but can be null</param>
  /// <returns>The object id of the new commit</returns>
  GitOid Amend(string? updateRef, IGitSignature? author, IGitSignature? committer, 
    string? message, IGitTree? tree);

  /// <summary>
  /// Get the author of a commit.
  /// </summary>
  /// <returns>the author of a commit</returns>
  IGitSignature GetAuthor();

  /// <summary>
  /// Get the author of a commit, using the mailmap to map names and email addresses to 
  /// canonical real names and email addresses.
  /// </summary>
  /// <param name="mailmap">the mailmap to resolve with. (may be null)</param>
  /// <returns>the author of a commit</returns>
  IGitSignature GetAuthor(IGitMailmap? mailmap);

  /// <summary>
  /// Get the committer of a commit.
  /// </summary>
  /// <returns>the committer of a commit</returns>
  IGitSignature GetCommitter();

  /// <summary>
  /// Get the committer of a commit, using the mailmap to map names and email addresses to 
  /// canonical real names and email addresses.
  /// </summary>
  /// <param name="mailmap">the mailmap to resolve with. (may be null)</param>
  /// <returns>the committer of a commit</returns>
  IGitSignature GetCommitter(IGitMailmap? mailmap);

  /// <summary>
  /// Get an arbitrary header field from the commit.
  /// </summary>
  /// <param name="field">the header field to return</param>
  /// <returns>The content of the header field</returns>
  byte[] GetHeaderField(string field);

  /// <summary>
  /// Get the tree pointed to by a commit.
  /// </summary>
  /// <returns>the tree pointed to by a commit</returns>
  IGitTree GetTree();

  /// <summary>
  /// Get the id of the tree pointed to by a commit
  /// </summary>
  /// <returns>the id of tree pointed to by commit.</returns>
  GitOid GetTreeId();

  /// <summary>
  /// Get the id of the commit.
  /// </summary>
  /// <returns>the id of the commit</returns>
  GitOid GetId();

  /// <summary>
  /// Get the full message of a commit.
  /// </summary>
  /// <remarks>
  /// The returned message will be slightly prettified by removing any potential leading newlines.
  /// </remarks>
  /// <returns>the message of a commit</returns>
  string GetMessage();

  /// <summary>
  /// Get the full raw message of the commit.
  /// </summary>
  /// <returns>the raw message of the commit</returns>
  string GetRawMessage();

  /// <summary>
  /// Get the long "body" of the git commit message.
  /// </summary>
  /// <remarks>
  /// The returned message is the body of the commit, comprising everything but the first paragraph of the message.
  /// Leading and trailing whitespaces are trimmed.
  /// </remarks>
  /// <returns>the body of a commit or null when no the message only consists of a summary</returns>
  string? GetBody();

  /// <summary>
  /// Get the short "summary" of the git commit message.
  /// </summary>
  /// <remarks>
  /// The returned message is the summary of the commit, comprising the first paragraph of the message 
  /// with whitespace trimmed and squashed.
  /// </remarks>
  /// <returns>the summary of a commit or null on error</returns>
  string? GetSummary();

  /// <summary>
  /// Get the specified parent of the commit.
  /// </summary>
  /// <param name="n">the position of the parent (from 0 to `parentcount`)</param>
  /// <returns>the parent commit</returns>
  IGitCommit GetParent(UInt32 n);

  /// <summary>
  /// Get the number of parents of this commit.
  /// </summary>
  /// <returns>integer of count of parents</returns>
  UInt32 GetParentCount();

  /// <summary>
  /// Get the id of the specified parent of the commit.
  /// </summary>
  /// <param name="n">the position of the parent (from 0 to `parentcount`)</param>
  /// <returns>the id of the parent, null on error.</returns>
  GitOid? GetParentId(UInt32 n);

  /// <summary>
  /// Get the nth-generation ancestor of the commit, following only first parents.
  /// </summary>
  /// <remarks>
  /// Passing 0 as the generation number returns another instance of the base commit itself.
  /// </remarks>
  /// <param name="n">the requested generation</param>
  /// <returns>the ancestor commit</returns>
  IGitCommit GetNthAncestor(UInt32 n);

  /// <summary>
  /// Get the full raw text of the commit header.
  /// </summary>
  /// <returns>the header text of the commit</returns>
  string GetRawHeader();

  /// <summary>
  /// Get the commit time (i.e. committer time) of a commit.
  /// </summary>
  /// <returns>the time of a commit</returns>
  DateTimeOffset GetCommitTime();

  /// <summary>
  /// Get the owning repository of the commit.
  /// </summary>
  IGitRepository Owner { get; }

  /// <summary>
  /// Iterate notes from a commit
  /// </summary>
  /// <returns>Iteratable notes</returns>
  IEnumerable<(GitOid NoteId, GitOid AnnotatedId)> IterateNotes();
}
