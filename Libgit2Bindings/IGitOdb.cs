namespace Libgit2Bindings;

public interface IGitOdb : IDisposable
{
  /// <summary>
  /// Write an object directly into the ODB
  /// </summary>
  /// <remarks>
  /// This method writes a full object straight into the ODB.
  /// For most cases, it is preferred to write objects through a write
  /// stream, which is both faster and less memory intensive, specially
  /// for big objects.
  /// <para/>
  /// This method is provided for compatibility with custom backends
  /// which are not able to support streaming writes
  /// </remarks>
  /// <param name="data">buffer with the data to store</param>
  /// <param name="type">type of the data to store</param>
  /// <returns>the OID result of the write</returns>
  GitOid Write(byte[] data, GitObjectType type);

  /// <summary>
  /// Determine if the given object can be found in the object database.
  /// </summary>
  /// <param name="oid">the object to search for.</param>
  /// <returns>true if the object was found, false otherwise</returns>
  bool Exists(GitOid oid);

  /// <summary>
  /// Determine if the given object can be found in the object database, with extended options.
  /// </summary>
  /// <param name="oid">the object to search for.</param>
  /// <param name="flags">flags affecting the lookup (see <see cref="GitOdbLookupFlags"/>)</param>
  /// <returns>true if the object was found, false otherwise</returns>
  bool Exists(GitOid oid, GitOdbLookupFlags flags);

  /// <summary>
  /// Determine if an object can be found in the object database by an abbreviated object ID.
  /// </summary>
  /// <param name="shortSha">A prefix of the id of the object to read.</param>
  /// <returns>The complete oid if the object was found, null otherwise</returns>
  GitOid? Exists(string shortSha);

  /// <summary>
  /// Determine if an object can be found in the object database by an abbreviated object ID.
  /// </summary>
  /// <param name="shortId">A prefix of the id of the object to read.</param>
  /// <returns>The complete oid if the object was found, null otherwise</returns>
  GitOid? Exists(byte[] shortId);
}

/// <summary>
/// Flags controlling the behavior of ODB lookup operations
/// </summary>
[Flags]
public enum GitOdbLookupFlags
{
  /// <summary>
  /// Don't call <see cref="IGitOdb.Refresh"/> if the lookup fails. Useful when doing
	/// a batch of lookup operations for objects that may legitimately not
	/// exist. When using this flag, you may wish to manually call
	/// <see cref="IGitOdb.Refresh"/> before processing a batch of objects.
  /// </summary>
  NoRefresh = 1 << 0,
}
