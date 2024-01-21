namespace Libgit2Bindings;

/// <summary>
/// A stream to read and write data to an object database
/// </summary>
public interface IOdbStream
{
  /// <summary>
  /// Write at most `len` bytes into `buffer` and advance the stream.
  /// </summary>
  /// <param name="buffer">The buffer to write to</param>
  /// <param name="length">The maximum number of bytes to write to buffer</param>
  void Read(byte[] buffer, int length);

  /// <summary>
  /// Write `len` bytes from `buffer` into the stream.
  /// </summary>
  /// <param name="buffer">The buffer to read from</param>
  /// <param name="length">The number of bytes to write to the stream</param>
  void Write(byte[] buffer, int length);

  /// <summary>
  /// Store the contents of the stream as an object with the id specified in `oid`.
  /// </summary>
  /// <remarks>
  /// This method might not be invoked if:
  /// <para/>
	/// - an error occurs earlier with the `write` callback,
  /// <para/>
	/// - the object referred to by `oid` already exists in any backend, or
  /// <para/>
	/// - the final number of received bytes differs from the size declared when opening the stream
  /// </remarks>
  /// <param name="oid">The id of the object</param>
  void FinalizeWrite(GitOid oid);
}
