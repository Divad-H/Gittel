namespace Libgit2Bindings;

/// <summary>
/// Basic type (loose or packed) of any Git object.
/// </summary>
public enum GitObjectType
{
  /// <summary>Object can be any of the following</summary>
  Any = -2,
  /// <summary>Object is invalid</summary>
  Invalid = -1,
  /// <summary>A commit object.</summary>
  Commit = 1,
  /// <summary>A tree (directory listing) object.</summary>
  Tree = 2,
  /// <summary>A file revision object.</summary>
  Blob = 3,
  /// <summary>An annotated tag object.</summary>
  Tag = 4,
  /// <summary>A delta, base is given by an offset.</summary>
  OffsetDelta = 6,
  /// <summary>A delta, base is given by object id.</summary>
  RefDelta = 7,
}
