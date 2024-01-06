namespace Libgit2Bindings;

/// <summary>
/// The results of <see cref="IGitRepository.MergeAnalysis"/> indicate the merge opportunities.
/// </summary>
public enum GitMergeAnalysisResult
{
  /// <summary>
  /// No merge is possible.  (Unused.)
  /// </summary>
  None = 0,
  /// <summary>
  ///A "normal" merge; both HEAD and the given merge input have diverged
	/// from their common ancestor. The divergent commits must be merged.
  /// </summary>
  Normal = (1 << 0),
  /// <summary>
  /// All given merge inputs are reachable from HEAD, meaning the
	/// repository is up-to-date and no merge needs to be performed.
  /// </summary>
  UpToDate = (1 << 1),
  /// <summary>
  /// The given merge input is a fast-forward from HEAD and no merge
	/// needs to be performed.  Instead, the client can check out the
	/// given merge input.
  /// </summary>
  FastForward = (1 << 2),
  /// <summary>
  /// The HEAD of the current repository is "unborn" and does not point to
	/// a valid commit.  No merge can be performed, but the caller may wish
	/// to simply set HEAD to the target commit(s).
  /// </summary>
  Unborn = (1 << 3),
}
