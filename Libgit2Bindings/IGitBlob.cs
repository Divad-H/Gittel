using libgit2;

namespace Libgit2Bindings;

public interface IGitBlob : IDisposable
{
  /// <summary>
  /// Determine if the blob content is most certainly binary or not.
  /// </summary>
  /// <remarks>
  /// The heuristic used to guess if a file is binary is taken from core git: 
  /// Searching for NUL bytes and looking for a reasonable ratio of printable 
  /// to non-printable characters among the first 8000 bytes.
  /// </remarks>
  /// <returns>true if the content of the blob is detected as binary; false otherwise.</returns>
  bool IsBinary();

  /// <summary>
  /// Create an in-memory copy of a blob.
  /// </summary>
  /// <returns>the copy of the object</returns>
  IGitBlob Duplicate();

  /// <summary>
  /// Get a buffer with the filtered content of a blob.
  /// </summary>
  /// <remarks>
  /// This applies filters as if the blob was being checked out to the working directory 
  /// under the specified filename. This may apply CRLF filtering or other types of changes 
  /// depending on the file attributes set for the blob and the content detected in it.
  /// </remarks>
  /// <param name="asPath">Path used for file attribute lookups, etc.</param>
  /// <param name="options">Options to use for filtering the blob</param>
  /// <returns>the filtered content of a blob</returns>
  byte[] Filter(string asPath, GitBlobFilterOptions? options);

  /// <summary>
  /// Get the id of a blob.
  /// </summary>
  /// <returns>SHA1 hash for this blob.</returns>
  GitOid Id();

  /// <summary>
  /// Get the repository that contains the blob.
  /// </summary>
  /// <returns>Repository that contains this blob.</returns>
  IGitRepository Owner();

  /// <summary>
  /// Get the raw content of a blob.
  /// </summary>
  /// <returns>the raw content of a blob</returns>
  byte[] RawContent();
}
