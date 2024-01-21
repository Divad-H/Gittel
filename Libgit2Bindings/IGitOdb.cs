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
  GitOid? Exists(byte[] shortId, UInt16 shortIdLength);

  /// <summary>
  /// Determine if one or more objects can be found in the object database by their abbreviated 
  /// object ID and type.
  /// </summary>
  /// <remarks>
  /// For each abbreviated ID that is unique in the database, and of the given type (if specified), 
  /// the full object ID, object ID length (GIT_OID_SHA1_HEXSIZE) and type will be returned back. 
  /// For IDs that are not found (or are ambiguous), the array entry will be zeroed.
  /// <para/>
  /// Note that since this function operates on multiple objects, the underlying database will not be 
  /// asked to be reloaded if an object is not found (which is unlike other object database operations.)
  /// </remarks>
  /// <param name="shortIds"></param>
  /// <returns></returns>
  IReadOnlyList<(GitOid oid, GitObjectType type)> ExpandIds(
    IEnumerable<(string shortId, GitObjectType type)> shortIds);

  /// <summary>
  /// Determine if one or more objects can be found in the object database by their abbreviated 
  /// object ID and type.
  /// </summary>
  /// <remarks>
  /// For each abbreviated ID that is unique in the database, and of the given type (if specified), 
  /// the full object ID, object ID length (GIT_OID_SHA1_HEXSIZE) and type will be returned back. 
  /// For IDs that are not found (or are ambiguous), the array entry will be zeroed.
  /// <para/>
  /// Note that since this function operates on multiple objects, the underlying database will not be 
  /// asked to be reloaded if an object is not found (which is unlike other object database operations.)
  /// </remarks>
  IReadOnlyList<(GitOid oid, GitObjectType type)> ExpandIds(
    IEnumerable<(byte[] shortId, UInt16 shortIdLength, GitObjectType type)> shortIds);

  /// <summary>
  /// List all objects available in the database
  /// </summary>
  /// <remarks>
  /// The callback will be called for each object available in the database. Note that the objects are 
  /// likely to be returned in the index order, which would make accessing the objects in that order 
  /// inefficient.
  /// </remarks>
  void ForEachOid(Func<GitOid, GitOperationContinuation> callback);

  /// <summary>
  /// Get the number of ODB backend objects
  /// </summary>
  /// <returns>number of backends in the ODB</returns>
  UIntPtr GetNumBackends();

  /// <summary>
  /// Lookup an ODB backend object by index
  /// </summary>
  /// <param name="index">index into object database backend list</param>
  /// <returns>ODB backend at pos</returns>
  IGitOdbBackend GetBackend(UIntPtr index);

  /// <summary>
  /// Add a custom backend to an existing Object DB
  /// </summary>
  /// <remarks>
  /// The backends are checked in relative ordering, based on the value of the priority parameter.
  /// </remarks>
  /// <param name="backend">a <see cref="IGitOdbBackend"/> instance</param>
  /// <param name="priority">Value for ordering the backends queue</param>
  void AddBackend(IGitOdbBackend backend, int priority);

  /// <summary>
  /// Add an on-disk alternate to an existing Object DB.
  /// </summary>
  /// <remarks>
  /// Note that the added path must point to an objects, not to a full repository, to use it as an 
  /// alternate store.
  /// <para/>
  /// Alternate backends are always checked for objects after all the main backends have been exhausted.
  /// <para/>
  /// Writing is disabled on alternate backends.
  /// </remarks>
  /// <param name="path">path to the objects folder for the alternate</param>
  void AddAlternativeOnDisk(string path);

  /// <summary>
  /// Read the header of an object from the database, without reading its full contents.
  /// </summary>
  /// <remarks>
  /// The header includes the length and the type of an object.
  /// <para/>
  /// Note that most backends do not support reading only the header of an object, so the whole 
  /// object will be read and then the header will be returned.
  /// </remarks>
  /// <param name="oid">identity of the object to read.</param>
  /// <returns>The length and type of the object</returns>
  (UIntPtr size, GitObjectType type) ReadHeader(GitOid oid);

  /// <summary>
  /// Read an object from the database.
  /// </summary>
  /// <remarks>
  /// This method queries all available ODB backends trying to read the given OID.
  /// </remarks>
  /// <param name="oid">identity of the object to read.</param>
  /// <returns>the read object</returns>
  IGitOdbObject Read(GitOid oid);

  /// <summary>
  /// Read an object from the database, given a prefix of its identifier.
  /// </summary>
  /// <remarks>
  /// This method queries all available ODB backends trying to match the 'len' first hexadecimal 
  /// characters of the 'shortId'. The remaining (GIT_OID_SHA1_HEXSIZE-len)*4 bits of 'shortId' 
  /// must be 0s. 'len' must be at least GIT_OID_MINPREFIXLEN, and the prefix must be long enough 
  /// to identify a unique object in all the backends; the method will fail otherwise.
  /// </remarks>
  /// <param name="shortId">a prefix of the id of the object to read.</param>
  /// <param name="length">the length of the prefix</param>
  /// <returns>the read object</returns>
  IGitOdbObject ReadPrefix(byte[] shortId, UIntPtr length);

  /// <summary>
  /// Read an object from the database, given a prefix of its identifier.
  /// </summary>
  /// <param name="shortSha"></param>
  /// <returns>the read object</returns>
  IGitOdbObject ReadPrefix(string shortSha);

  /// <summary>
  /// Refresh the object database to load newly added files.
  /// </summary>
  /// <remarks>
  /// If the object databases have changed on disk while the library is running, this function will force 
  /// a reload of the underlying indexes.
  /// <para/>
  /// Use this function when you're confident that an external application has tampered with the ODB.
  /// <para/>
  /// NOTE that it is not necessary to call this function at all.The library will automatically attempt 
  /// to refresh the ODB when a lookup fails, to see if the looked up object exists on disk but hasn't 
  /// been loaded yet.
  /// </remarks>
  void Refresh();

  /// <summary>
  /// Open a stream to write an object into the ODB
  /// </summary>
  /// <remarks>
  /// The type and final length of the object must be specified when opening the stream.
  /// <para/>
  /// The returned stream will be of type GIT_STREAM_WRONLY, and it won't be effective until 
  /// <see cref="IOdbStream.FinalizeWrite"/> is called and returns without an error
  /// </remarks>
  /// <param name="size">final size of the object that will be written</param>
  /// <param name="type">type of the object that will be written</param>
  /// <returns>the stream</returns>
  IOdbStream OpenWriteStream(UIntPtr size, GitObjectType type);

  /// <summary>
  /// Open a stream to read an object from the ODB
  /// </summary>
  /// <remarks>
  /// Note that most backends do not support streaming reads because they store their objects as 
  /// compressed/delta'ed blobs.
  /// <para/>
  /// It's recommended to use <see cref="Read(GitOid)"/> instead, which is assured to work on all backends.
  /// </remarks>
  /// <param name="oid">oid of the object the stream will read from</param>
  /// <returns>the stream, the length of the object and the type of the object</returns>
  (IOdbStream stream, UIntPtr size, GitObjectType type) OpenReadStream(GitOid oid);
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
