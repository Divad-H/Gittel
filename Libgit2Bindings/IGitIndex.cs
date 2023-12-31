namespace Libgit2Bindings;

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
  /// Get index on-disk version.
  /// </summary>
  /// <remarks>
  /// Valid return values are 2, 3, or 4. If 3 is returned, an index with version 2 may be written 
  /// instead, if the extension data in version 3 is not necessary.
  /// </remarks>
  UInt32 Version { get; }

  /// <summary>
  /// Set index capabilities flags.
  /// </summary>
  /// <remarks>
  /// If you pass <see cref="GitIndexCapability.FromOwner"/> for the caps, then capabilities will be
  /// read from the config of the owner object, looking at core.ignorecase, core.filemode, core.symlinks.
  /// </remarks>
  /// <param name="capabilities"></param>
  void SetCapabilities(GitIndexCapability capabilities);

  /// <summary>
  /// Set index on-disk version.
  /// </summary>
  /// <remarks>
  /// Valid values are 2, 3, or 4. If 2 is given, git_index_write may write an index with version 
  /// 3 instead, if necessary to accurately represent the index.
  /// </remarks>
  /// <param name="version">The new version number</param>
  void SetVersion(UInt32 version);

  /// <summary>
  /// Determine if the index contains entries representing file conflicts.
  /// </summary>
  /// <returns>true if at least one conflict is found, false otherwise.</returns>
  bool HasConflicts();

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
  /// Get one of the entries in the index
  /// </summary>
  /// <param name="index">the position of the entry</param>
  /// <returns>an existing index object</returns>
  GitIndexEntry GetEntry(UInt64 index);

  /// <summary>
  /// Add or update an index entry from an in-memory struct
  /// </summary>
  /// <remarks>
  /// If a previous index entry exists that has the same path and stage as the given 
  /// 'entry', it will be replaced. Otherwise, the 'entry' will be added.
  /// <para/>
  /// A full copy(including the 'path' string) of the given 'entry' will be inserted on the index.
  /// </remarks>
  /// <param name="entry">new entry object</param>
  void Add(GitIndexEntry entry);

  /// <summary>
  /// Add or update an index entry from a buffer in memory
  /// </summary>
  /// <remarks>
  /// This method will create a blob in the repository that owns the index and then add the index 
  /// entry to the index. The path of the entry represents the position of the blob relative to 
  /// the repository's root folder.
  /// <para/>
  /// If a previous index entry exists that has the same path as the given 'entry', it will be 
  /// replaced.Otherwise, the 'entry' will be added.
  /// <para/>
  /// This forces the file to be added to the index, not looking at gitignore rules.Those rules can 
  /// be evaluated through the git_status APIs (in status.h) before calling this.
  /// <para/>
  /// If this file currently is the result of a merge conflict, this file will no longer be marked 
  /// as conflicting.The data about the conflict will be moved to the "resolve undo" (REUC) section.
  /// </remarks>
  /// <param name="entry">filename to add</param>
  /// <param name="buffer">data to be written into the blob</param>
  void AddFromBuffer(GitIndexEntry entry, byte[] buffer);

  /// <summary>
  /// Add or update index entries to represent a conflict. Any staged entries that exist at the 
  /// given paths will be removed.
  /// </summary>
  /// <remarks>
  /// The entries are the entries from the tree included in the merge. Any entry may be null to 
  /// indicate that that file was not present in the trees during the merge. For example, 
  /// ancestorEntry may be null to indicate that a file was added in both branches and must be resolved.
  /// </remarks>
  /// <param name="ancestorEntry"></param>
  /// <param name="ourEntry"></param>
  /// <param name="theirEntry"></param>
  void AddConflict(GitIndexEntry? ancestorEntry, GitIndexEntry? ourEntry, GitIndexEntry? theirEntry);

  /// <summary>
  /// Remove all conflicts in the index (entries with a stage greater than 0).
  /// </summary>
  void CleanupConflicts();

  /// <summary>
  /// Get the index entries that represent a conflict of a single file.
  /// </summary>
  /// <param name="path">path to search</param>
  /// <returns></returns>
  ConflictEntries GetConflict(string path);

  /// <summary>
  /// Get the checksum of the index
  /// </summary>
  /// <remarks>
  /// This checksum is the SHA-1 hash over the index file (except the last 20 bytes which are the 
  /// checksum itself). In cases where the index does not exist on-disk, it will be zeroed out.
  /// </remarks>
  /// <returns>The checksum of the index</returns>
  GitOid GetChecksum();

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
