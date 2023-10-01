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
}
