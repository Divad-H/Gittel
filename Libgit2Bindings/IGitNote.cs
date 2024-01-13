namespace Libgit2Bindings;
public interface IGitNote : IDisposable
{
  /// <summary>
  /// Get the note author
  /// </summary>
  IGitSignature Author { get; }

  /// <summary>
  /// Get the note committer
  /// </summary>
  IGitSignature Committer { get; }

  /// <summary>
  /// Get the note message
  /// </summary>
  string Message { get; }

  /// <summary>
  /// Get the note object's id
  /// </summary>
  GitOid Id { get; }
}

/// <summary>
/// Callback for <see cref="IGitRepository.ForeachNote"/>
/// </summary>
/// <param name="blobId">Oid of the blob containing the message</param>
/// <param name="annotatedOid">Oid of the git object being annotated</param>
/// <returns><see cref="GitOperationContinuation"/></returns>
public delegate GitOperationContinuation GitNoteForeachCallback(GitOid blobId, GitOid annotatedOid);
