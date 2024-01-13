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
}
