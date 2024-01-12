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
