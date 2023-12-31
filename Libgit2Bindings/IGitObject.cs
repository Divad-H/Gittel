﻿namespace Libgit2Bindings;

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

  /// <summary>
  /// Get a short abbreviated OID string for the object
  /// </summary>
  /// <remarks>
  /// This starts at the "core.abbrev" length (default 7 characters) and iteratively 
  /// extends to a longer string if that length is ambiguous. The result will be 
  /// unambiguous (at least until new objects are added to the repository).
  /// </remarks>
  string ShortId { get; }

  /// <summary>
  /// Recursively peel an object until an object of the specified type is met.
  /// </summary>
  /// <remarks>
  /// If the query cannot be satisfied due to the object model, en exception will be thrown.
  /// (e.g. trying to peel a blob to a tree).
  /// <para/>
  /// If you pass <see cref="GitObjectType.Any"/> as the target type, then the object will 
  /// be peeled until the type changes. A tag will be peeled until the referenced object is 
  /// no longer a tag, and a commit will be peeled to a tree. Any other object type will 
  /// throw an exception.
  /// </remarks>
  /// <param name="type">The type of the requested object</param>
  /// <returns>the peeled <see cref="IGitObject"/></returns>
  IGitObject Peel(GitObjectType type);

  /// <summary>
  /// Describe a commit.
  /// </summary>
  /// <param name="options">the lookup options (or null for defaults)</param>
  /// <returns>The describe result</returns>
  IGitDescribeResult DescribeCommit(GitDescribeOptions? options);
}
