namespace Libgit2Bindings;

/// <summary>
/// Flags controlling the behavior of <see cref="IGitRepository.ApplyDiff"/>
/// </summary>
[Flags]
public enum GitApplyFlags
{
  /// <summary>
  /// Don't actually make changes, just test that the patch applies.
	/// This is the equivalent of `git apply --check`.
  /// </summary>
  Check = (1 << 0),
}

public enum GitApplyLocation
{
  /// <summary>
  /// Apply the patch to the workdir, leaving the index untouched.
	/// This is the equivalent of git apply with no location argument.
  /// </summary>
  Workdir = 0,
  /// <summary>
  /// Apply the patch to the index, leaving the working directory
	/// untouched.  This is the equivalent of `git apply --cached`.
  /// </summary>
  Index = 1,
  /// <summary>
  /// Apply the patch to both the working directory and the index.
	/// This is the equivalent of `git apply --index`.
  /// </summary>
  Both = 2,
}

/// <summary>
/// Callback that will be made per delta (file) when applying a patch.
/// </summary>
/// <param name="delta">
/// The delta (file) that is being applied.
/// </param>
/// <returns>Whether to continue</returns>
public delegate GitOperationContinuation ApplyDeltaCallback(GitDiffDelta delta);
/// <summary>
/// Callback that will be made per hunk when applying a patch.
/// </summary>
/// <param name="hunk">
/// The hunk that is being applied.
/// </param>
/// <returns>Whether to continue</returns>
public delegate GitOperationContinuation ApplyHunkCallback(GitDiffHunk hunk);

/// <summary>
/// Apply options structure
/// <para/>
/// <see cref="IGitRepository.ApplyDiff"/>
/// </summary>
public sealed record GitApplyOptions
{
  /// <summary>
  /// When applying a patch, callback that will be made per delta (file).
  /// </summary>
  public ApplyDeltaCallback? DeltaCallback { get; init; }
  /// <summary>
  /// When applying a patch, callback that will be made per hunk.
  /// </summary>
  public ApplyHunkCallback? HunkCallback { get; init; }
  /// <summary>
  /// Bitmask of <see cref="GitApplyFlags"/>
  /// </summary>
  public GitApplyFlags Flags { get; init; }
}
