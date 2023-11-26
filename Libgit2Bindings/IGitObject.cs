namespace Libgit2Bindings;

public interface IGitObject : IDisposable
{
  /// <summary>
  /// Get the id (SHA1) of a repository object
  /// </summary>
  public GitOid Id { get; }

  /// <summary>
  /// Create an in-memory copy of a Git object.
  /// </summary>
  /// <returns> the copy of the object</returns>
  public IGitObject Duplicate();
}
