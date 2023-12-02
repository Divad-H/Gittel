namespace Libgit2Bindings;

/// <summary>
/// Describe format options structure
/// </summary>
public record GitDescribeFormatOptions
{
  /// <summary>
  /// Size of the abbreviated commit id to use. This value is the
	/// lower bound for the length of the abbreviated string. The
  /// default is 7.
  /// </summary>
  public UInt32 AbbreviatedSize { get; init; } = 7;

  /// <summary>
  /// Set to use the long format even when a shorter name could be used.
  /// </summary>
  public bool AlwaysUseLongFormat { get; init; } = false;

  /// <summary>
  /// If the workdir is dirty and this is set, this string will
	/// be appended to the description string.
  /// </summary>
  public string ? DirtySuffix { get; init; } = null;
}

public interface IGitDescribeResult : IDisposable
{
  public string Format(GitDescribeFormatOptions? options = null);
}
