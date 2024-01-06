namespace Libgit2Bindings;

/// <summary>
/// The user's stated preference for merges.
/// </summary>
public enum GitMergePreference
{
  /// <summary>
  /// No configuration was found that suggests a preferred behavior for
  /// merge.
  /// </summary>
  None = 0,

  /// <summary>
  /// There is a `merge.ff=false` configuration setting, suggesting that
	/// the user does not want to allow a fast-forward merge.
  /// </summary>
  NoFastForward = (1 << 0),

  /// <summary>
  /// There is a `merge.ff=only` configuration setting, suggesting that
	/// the user only wants fast-forward merges.
  /// </summary>
  FastForwardOnly = (1 << 1),
}
