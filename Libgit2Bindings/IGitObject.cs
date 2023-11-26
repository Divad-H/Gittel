namespace Libgit2Bindings;

public interface IGitObject : IDisposable
{
  /// <summary>
  /// Get the id (SHA1) of a repository object
  /// </summary>
  GitOid Id { get; }

  /// <summary>
  /// Create an in-memory copy of a Git object.
  /// </summary>
  /// <returns> the copy of the object</returns>
  IGitObject Duplicate();

  /// <summary>
  /// Lookup an object that represents a tree entry.
  /// </summary>
  /// <remarks>
  /// This must be called on a root object that can be peeled to a tree
  /// </remarks>
  /// <param name="path">relative path from the root object to the desired object</param>
  /// <param name="type">type of object desired</param>
  /// <returns>the object</returns>
  IGitObject LookupByPath(string path, GitObjectType type);

  /// <summary>
  /// Get the object type of an object
  /// </summary>
  GitObjectType Type { get; }

  /// <summary>
  /// Get the repository that contains this object
  /// </summary>
  IGitRepository Owner { get; }
}
