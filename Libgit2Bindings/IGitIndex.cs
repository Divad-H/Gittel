﻿namespace Libgit2Bindings;

public interface IGitIndex : IDisposable
{
  /// <summary>
  /// Get the count of entries currently in the index
  /// </summary>
  UInt64 EntryCount { get; }

  /// <summary>
  /// Read index capabilities flags.
  /// </summary>
  GitIndexCapability Capabilities { get; }

  /// <summary>
  /// Add or update an index entry from a file on disk
  /// </summary>
  /// <remarks>
  /// <para>the file path must be relative to the repository's working folder and must be readable.</para>
  /// <para>This method will fail in bare index instances.</para>
  /// <para>This forces the file to be added to the index, not looking at gitignore rules. </para>
  /// <para>
  /// If this file currently is the result of a merge conflict, this file will no longer be marked 
  /// as conflicting. The data about the conflict will be moved to the "resolve undo" (REUC) section.
  /// </para>
  /// </remarks>
  /// <param name="path">filename to add</param>
  void AddByPath(string path);

  /// <summary>
  /// Add or update index entries matching files in the working directory.
  /// </summary>
  /// <param name="pathspecs">array of path patterns</param>
  /// <param name="flags">combination of <see cref="GitIndexAddOption"/> flags</param>
  /// <param name="callback">
  /// notification callback for each added/updated path (also gets index of matching pathspec entry); 
  /// can be nulkl.
  /// </param>
  void AddAll(IReadOnlyCollection<string> pathspecs, GitIndexAddOption flags, 
    GitIndexMatchedPathCallback? callback = null);

  /// <summary>
  /// Clear the contents (all the entries) of an index object.
  /// </summary>
  /// <remarks>
  /// This clears the index object in memory; changes must be explicitly written to disk for them 
  /// to take effect persistently.
  /// </remarks>
  void Clear();

  /// <summary>
  /// Write the index as a tree
  /// </summary>
  /// <returns>The object id of the tree</returns>
  GitOid WriteTree();

  /// <summary>
  /// Write the index as a tree to the given repository
  /// </summary>
  /// <param name="repository">Repository where to write the tree</param>
  /// <returns>OID of the written tree</returns>
  GitOid WriteTreeTo(IGitRepository repository);

  /// <summary>
  /// Write an existing index object from memory back to disk using an atomic file lock.
  /// </summary>
  void Write();
}

public enum GitAddToIndexOperation
{
  Error = -1,
  Add = 0,
  Skip = 1,
}

public delegate GitAddToIndexOperation GitIndexMatchedPathCallback(string path, string matchedPathspec);

/// <summary>
/// Flags for APIs that add files matching pathspec
/// </summary>
[Flags]
public enum GitIndexAddOption
{
  Default = 0,
  Force = 1 << 0,
  DisablePathspecMatch = 1 << 1,
  CheckPathspec = 1 << 2,
}

/// <summary>
/// Capabilities of system that affect index actions.
/// </summary>
[Flags]
public enum GitIndexCapability
{
  IgnoreCase = 1,
  NoFilemode = 2,
  NoSymlinks = 4,
  FromOwner = -1,
}
