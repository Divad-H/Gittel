namespace Libgit2Bindings;

/// <summary>
/// Normalization options for reference lookup
/// </summary>
[Flags]
public enum GitReferenceFormat
{
  /// <summary>
  /// No particular normalization.
  /// </summary>
  Normal = 0,

  /// <summary>
  /// Control whether one-level refnames are accepted
	/// (i.e., refnames that do not contain multiple /-separated
	/// components). Those are expected to be written only using
  /// uppercase letters and underscore(FETCH_HEAD, ...)
  /// </summary>
  AllowOneLevel = 1 << 0,

  /// <summary>
  /// Interpret the provided name as a reference pattern for a
  /// refspec (as used with remote repositories). If this option
  /// is enabled, the name is allowed to contain a single * (<star>)
  /// in place of a one full pathname component
  /// (e.g., foo/<star>/bar but not foo/bar<star>).
  /// </summary>
  RefspecPattern = 1 << 1,

  /// <summary>
  /// Interpret the name as part of a refspec in shorthand form
  /// so the `ONELEVEL` naming rules aren't enforced and 'master'
  /// becomes a valid name.
  /// </summary>
  RefspecShorthand = 1 << 2,
}
