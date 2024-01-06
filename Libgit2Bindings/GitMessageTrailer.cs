namespace Libgit2Bindings;

/// <summary>
/// Represents a single git message trailer.
/// </summary>
public sealed record GitMessageTrailer
{
  /// <summary>
  /// The key of the trailer.
  /// </summary>
  public required byte[] Key { get; init; }
  /// <summary>
  /// The value of the trailer.
  /// </summary>
  public required byte[] Value { get; init; }
}
