namespace Libgit2Bindings;

public interface IGitRebase : IDisposable
{
  /// <summary>
  /// Gets the original HEAD id for merge rebases.
  /// </summary>
  GitOid OriginalHeadId { get; }

  /// <summary>
  /// Gets the original HEAD ref name for merge rebases.
  /// </summary>
  string? OriginalHeadName { get; }

  /// <summary>
  /// Gets the onto id for merge rebases.
  /// </summary>
  GitOid OntoId { get; }

  /// <summary>
  /// Gets the onto ref name for merge rebases.
  /// </summary>
  string? OntoName { get; }

  /// <summary>
  /// Performs the next rebase operation and returns the information about it. If the operation 
  /// is one that applies a patch (which is any operation except <see cref="GitRebaseOperationType.Exec"/>) 
  /// then the patch will be applied and the index and working directory will be updated with the changes. 
  /// If there are conflicts, you will need to address those before committing the changes.
  /// </summary>
  /// <returns> the rebase operation that is to be performed next or null if there is no more iteration</returns>
  GitRebaseOperation? Next();

  /// <summary>
  /// Commits the current patch. You must have resolved any conflicts that were introduced during the 
  /// patch application from the <see cref="Next"/> invocation.
  /// </summary>
  /// <param name="author">The author of the updated commit, or null to keep the author from the original commit</param>
  /// <param name="committer">The committer of the rebase</param>
  /// <param name="message">The message for this commit, or null to use the message from the original commit.</param>
  /// <returns>the OID of the newly created commit</returns>
  GitOid Commit(IGitSignature? author, IGitSignature committer, string? message = null);

  /// <summary>
  /// Finishes a rebase that is currently in progress once all patches have been applied.
  /// </summary>
  /// <param name="signature">The identity that is finishing the rebase (optional)</param>
  void Finish(IGitSignature? signature = null);

  /// <summary>
  /// Aborts a rebase that is currently in progress, resetting the repository and working directory to their 
  /// state before rebase began.
  /// </summary>
  void Abort();

  /// <summary>
  /// Gets the index produced by the last operation, which is the result of <see cref="Next"/> and which 
  /// will be committed by the next invocation of <see cref="Commit"/>. This is useful for resolving 
  /// conflicts in an in-memory rebase before committing them.
  /// </summary>
  /// <returns>index of the last operation.</returns>
  IGitIndex GetInMemoryIndex();
}
