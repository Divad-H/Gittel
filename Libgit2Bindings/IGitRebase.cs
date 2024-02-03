﻿namespace Libgit2Bindings;

public interface IGitRebase : IDisposable
{
  /// <summary>
  /// Performs the next rebase operation and returns the information about it. If the operation 
  /// is one that applies a patch (which is any operation except <see cref="GitRebaseOperationType.Exec"/>) 
  /// then the patch will be applied and the index and working directory will be updated with the changes. 
  /// If there are conflicts, you will need to address those before committing the changes.
  /// </summary>
  /// <returns> the rebase operation that is to be performed next</returns>
  GitRebaseOperation? Next();

  /// <summary>
  /// Commits the current patch. You must have resolved any conflicts that were introduced during the 
  /// patch application from the <see cref="Next"/> invocation.
  /// </summary>
  /// <param name="author">The author of the updated commit, or null to keep the author from the original commit</param>
  /// <param name="committer">The committer of the rebase</param>
  /// <param name="message">The message for this commit, or null to use the message from the original commit.</param>
  /// <returns>the OID of the newly created commit</returns>
  GitOid Commit(IGitSignature? author, IGitSignature? committer, string? message);

  /// <summary>
  /// Finishes a rebase that is currently in progress once all patches have been applied.
  /// </summary>
  /// <param name="signature">The identity that is finishing the rebase (optional)</param>
  void Finish(IGitSignature? signature);
}