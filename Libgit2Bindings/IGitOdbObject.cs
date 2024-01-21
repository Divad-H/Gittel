namespace Libgit2Bindings;

public interface IGitOdbObject : IDisposable
{
  /// <summary>
  /// The object's id
  /// </summary>
  GitOid Id { get; }

  /// <summary>
  /// The object's type
  /// </summary>
  GitObjectType Type { get; }
  
  /// <summary>
  /// The object's data
  /// </summary>
  byte[] Data { get; }

  /// <summary>
  /// The object's size
  /// </summary>
  UIntPtr Size { get; }

  /// <summary>
  /// Create a copy of an <see cref="IGitOdbObject"/> instance.
  /// </summary>
  /// <returns>the copy</returns>
  IGitOdbObject Duplicate();
}
