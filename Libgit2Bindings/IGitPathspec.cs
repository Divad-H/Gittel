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

  /// <summary>
  /// Match a pathspec against the working directory of a repository.
  /// </summary>
  /// <remarks>
  /// This matches the pathspec against the current files in the working directory of the repository. 
  /// <para/>
  /// It is an error to invoke this on a bare repo. This handles git ignores (i.e. ignored files will 
  /// not be considered to match the pathspec unless the file is tracked in the index).
  /// </remarks>
  /// <param name="repo">The repository in which to match; bare repo is an error</param>
  /// <param name="flags">combination of <see cref="GitPathspecFlags"/> options to control match</param>
  /// <returns>list of all matched filenames</returns>
  IGitPathspecMathList MatchWorkdir(IGitRepository repo, GitPathspecFlags flags);

  /// <summary>
  /// Match a pathspec against entries in an index.
  /// </summary>
  /// <remarks>
  /// This matches the pathspec against the files in the repository index.
  /// </remarks>
  /// <param name="index">The index to match against</param>
  /// <param name="flags">combination of <see cref="GitPathspecFlags"/> options to control match</param>
  /// <returns>list of all matched filenames</returns>
  IGitPathspecMathList MatchIndex(IGitIndex index, GitPathspecFlags flags);


  /// <summary>
  /// Match a pathspec against files in a tree.
  /// </summary>
  /// <remarks>
  /// This matches the pathspec against the files in the given tree.
  /// </remarks>
  /// <param name="tree">The root-level tree to match against</param>
  /// <param name="flags">combination of <see cref="GitPathspecFlags"/> options to control match</param>
  /// <returns>list of all matched filenames</returns>
  IGitPathspecMathList MatchTree(IGitTree tree, GitPathspecFlags flags);

  /// <summary>
  /// Match a pathspec against files in a diff list.
  /// </summary>
  /// <remarks>
  /// This matches the pathspec against the files in the given diff list.
  /// </remarks>
  /// <param name="diff">A generated diff list</param>
  /// <param name="flags">combination of <see cref="GitPathspecFlags"/> options to control match</param>
  /// <returns>list of all matched filenames</returns>
  IGitPathspecMathList MatchDiff(IGitDiff diff, GitPathspecFlags flags);
}
