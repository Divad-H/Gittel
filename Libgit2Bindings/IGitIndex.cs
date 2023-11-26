namespace Libgit2Bindings;

public interface IGitIndex : IDisposable
{
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
