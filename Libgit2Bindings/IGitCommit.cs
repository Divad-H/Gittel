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
  /// <param name="messageEncoding">Same as <see cref="IGitRepository.CreateCommit"/></param>
  /// <param name="message">Same as <see cref="IGitRepository.CreateCommit"/> but can be null</param>
  /// <param name="tree">Same as <see cref="IGitRepository.CreateCommit"/> but can be null</param>
  /// <returns>The object id of the new commit</returns>
  GitOid Amend(string? updateRef, IGitSignature? author, IGitSignature? committer, 
    string? messageEncoding, string? message, IGitTree? tree);

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
  /// Get the long "body" of the git commit message.
  /// </summary>
  /// <remarks>
  /// The returned message is the body of the commit, comprising everything but the first paragraph of the message.
  /// Leading and trailing whitespaces are trimmed.
  /// </remarks>
  /// <returns>the body of a commit or null when no the message only consists of a summary</returns>
  string? GetBody();
}
