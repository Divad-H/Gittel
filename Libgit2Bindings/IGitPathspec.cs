namespace Libgit2Bindings;

/// <summary>
/// Options controlling how pathspec match should be executed
/// </summary>
[Flags]
public enum GitPathspecFlags
{
  /// <summary>
  /// Default behavior.
  /// </summary>
  Default = 0,
  /// <summary>
  /// forces match to ignore case; otherwise match will use native case sensitivity of platform filesystem
  /// </summary>
  IgnoreCase = (1 << 0),
  /// <summary>
  /// forces case sensitive match; otherwise match will use native case sensitivity of platform filesystem
  /// </summary>
  UseCase = (1 << 1),
  /// <summary>
  /// disables glob patterns and just uses simple string comparison for matching
  /// </summary>
  NoGlob = (1 << 2),
  /// <summary>
  /// means the match functions throw error code GIT_ENOTFOUND if no matches are found; 
  /// otherwise no matches is still success but the entrycount will be 0
  /// </summary>
  NoMatchError = (1 << 3),
  /// <summary>
  /// means that the MatchList should track which patterns matched which files so that at the end of
  /// the match we can identify patterns that did not match any files.
  /// </summary>
  FindFailures = (1 << 4),
  /// <summary>
  /// means that the MatchList does not need to keep the actual matching filenames. Use this to
  /// just test if there were any matches at all or in combination with <see cref="FindFailures"/> 
  /// to validate a pathspec.
  /// </summary>
  FailureOnly = (1 << 5),
}

/// <summary>
/// Compiled pathspec
/// </summary>
public interface IGitPathspec : IDisposable
{
  /// <summary>
  /// Try to match a path against a pathspec
  /// </summary>
  /// <param name="flags">options to control match <see cref="GitPathspecFlags"/></param>
  /// <param name="path">The pathname to attempt to match</param>
  /// <returns>True if the path mathes the spec, false otherwise</returns>
  bool MatchesPath(GitPathspecFlags flags, string path);
}
