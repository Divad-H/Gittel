namespace Libgit2Bindings;

/// <summary>
/// Cherry-pick options
/// </summary>
public record CherrypickOptions
{
  /// <summary>
  /// For merge commits, the "mainline" is treated as the parent. 
  /// </summary>
  public int Mainline { get; init; } = 0;

  /// <summary>
  /// Options for the merging
  /// </summary>
  public MergeOptions MergeOptions { get; init; } = new();

  /// <summary>
  /// Options for the checkout
  /// </summary>
  public CheckoutOptions CheckoutOptions { get; init; } = new();
}
