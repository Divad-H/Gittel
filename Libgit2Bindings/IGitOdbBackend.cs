namespace Libgit2Bindings;


public interface IGitOdbBackend : IDisposable
{
  /*
   * When not using a custom allocator, it is not possible to allocate memory that must be returned
   * for example in the Read function.
   * 
  /// <summary>
  /// Result of <see cref="Read"/> and <see cref="ReadPrefix"/>
  /// </summary>
  public sealed record ReadResult
  {
    /// <summary>
    /// The Oid (if found)
    /// </summary>
    public GitOid? Oid { get; init; }
    /// <summary>
    /// The raw content of the object
    /// </summary>
    public byte[]? Data { get; init; }
    /// <summary>
    /// The type of the object
    /// </summary>
    public GitObjectType Type { get; init; }
  }

  /// <summary>
  /// returns a buffer with the raw data for an object id
  /// </summary>
  /// <param name="oid">The oid of the object to be read</param>
  /// <returns>The content of the object and its type</returns>
  Func<GitOid, ReadResult>? Read { get; }

  /// <summary>
  /// returns a buffer with the raw data for an object id.
  /// The oid given must be so that the remaining(GIT_OID_SHA1_HEXSIZE - len)*4 bits are 0s.
  /// </summary>
  /// <param name="oid">The oid of the object to be read</param>
  /// <returns>The content of the object and its type</returns>
  Func<GitOid, ReadResult>? ReadPrefix { get; }

  /// <summary>
  /// Result of <see cref="ReadHeader"/>
  /// </summary>
  public sealed record ReadHeaderResult
  {
    /// <summary>
    /// The size of the object
    /// </summary>
    public UIntPtr Size { get; init; }
    /// <summary>
    /// The type of the object
    /// </summary>
    public GitObjectType Type { get; init; }
  }

  /// <summary>
  /// Read the size and type of an object from the backend
  /// </summary>
  /// <param name="oid">The oid of the object to be read</param>
  /// <returns>The size of the object and its type</returns>
  Func<GitOid, ReadHeaderResult>? ReadHeader { get; }

  /// <summary>
  /// Write an object directly into the backend
  /// </summary>
  /// <param name="oid">The id of the object has already been calculated and is passed in.</param>
  /// <param name="type">The type of the object</param>
  /// <param name="data">The content of the object</param>
  Action<GitOid, GitObjectType, byte[]>? Write { get; }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="objectSize"></param>
  /// <param name="objectType"></param>
  /// <returns></returns>
  Func<UInt64, GitObjectType, IOdbStream>? WriteStream { get; }

  public sealed record ReadStreamResult
  {
    public required IOdbStream Stream { get; init; }
    public UIntPtr Size { get; init; }
    public GitObjectType Type { get; init; }
  }

  Func<GitOid, ReadStreamResult>? ReadStream { get; }

  Func<GitOid, bool>? Exists { get; }

  Func<GitOid, UIntPtr, bool>? ExistsPrefix { get; }

  Action? Refresh { get; }

  Action<Func<GitOid, GitOperationContinuation>>? ForEach { get; }

  Func<IGitOdb, IndexerProgressHandler?>? WritePack { get; }

  Action? WriteMIdx { get; }

  Action<GitOid>? Freshen { get; }
  */
}
