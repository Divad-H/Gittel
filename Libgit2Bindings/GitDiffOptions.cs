namespace Libgit2Bindings;

/// <summary>
/// Flags for diff options. A combination of these flags can be passed
/// in via the <see cref="GitDiffOptions.Flags"/> value in the <see cref="GitDiffOptions"/>.
/// </summary>
[Flags]
public enum GitDiffOptionFlags : UInt32
{
  /// <summary>
  /// Normal diff, the default
  /// </summary>
  Normal = 0,

  // Options controlling which files will be in the diff

  /// <summary>
  /// Reverse the sides of the diff
  /// </summary>
  Reverse = 1 << 0,

  /// <summary>
  /// Include ignored files in the diff
  /// </summary>
  IncludeIgnored = 1 << 1,

  /// <summary>
  /// Even with <see cref="IncludeIgnored"/> enabled, an entire ignored directory
  /// will be marked with only a single entry in the diff. This flag adds all
  /// files under the ignored directory as IGNORED entries in the diff.
  /// </summary>
  RecurseIgnoredDirs = 1 << 2,

  /// <summary>
  /// Include untracked files in the diff
  /// </summary>
  IncludeUntracked = 1 << 3,

  /// <summary>
  /// Even with <see cref="IncludeUntracked"/> enabled, an entire untracked directory
  /// will be marked with only a single entry in the diff.
  /// (a la what core Git does in `git status`); this flag adds *all*
  /// files under untracked directories as UNTRACKED entries, too.
  /// </summary>
  RecurseUntrackedDirs = 1 << 4,

  /// <summary>
  /// Include unmodified files in the diff
  /// </summary>
  IncludeUnmodified = 1 << 5,

  /// <summary>
  /// Normally, a type change between files will be converted into a
  /// DELETED record for the old and an ADDED record for the new; this
  /// options enabled the generation of TYPECHANGE delta records.
  /// </summary>
  IncludeTypeChange = 1 << 6,

  /// <summary>
  /// Even with <see cref="IncludeTypeChange"/> enabled, blob->tree changes still
  /// generally show as a DELETED blob. This flag tries to correctly
  /// label blob->tree transitions as TYPECHANGE records with new_file's
  /// mode set to tree. Note: the tree SHA will not be available.
  /// </summary>
  IncludeTypeChangeTrees = 1 << 7,

  /// <summary>
  /// Ignore file mode changes
  /// </summary>
  IgnoreFileMode = 1 << 8,

  /// <summary>
  /// Treat all submodules as unmodified
  /// </summary>
  IgnoreSubmodules = 1 << 9,

  /// <summary>
  /// Use case insensitive filename comparisons
  /// </summary>
  IgnoreCase = 1 << 10,

  /// <summary>
  /// May be combined with <see cref="IgnoreCase"/> to specify that a file
  /// that has changed case will be returned as an add/delete pair.
  /// </summary>
  IncludeCaseChange = 1 << 11,

  /// <summary>
  /// If the pathspec is set in the diff options, this flags indicates
  /// that the paths will be treated as literal paths instead of
  /// fnmatch patterns. Each path in the list must either be a full
  /// path to a file or a directory. (A trailing slash indicates that
  /// the path will _only_ match a directory). If a directory is
  /// specified, all children will be included.
  /// </summary>
  DisablePathspecMatch = 1 << 12,

  /// <summary>
  /// Disable updating of the `binary` flag in delta records.  This is
  /// useful when iterating over a diff if you don't need hunk and data
  /// callbacks and want to avoid having to load file completely.
  /// </summary>
  SkipBinaryCheck = 1 << 13,

  /// <summary>
  /// When diff finds an untracked directory, to match the behavior of
  /// core Git, it scans the contents for IGNORED and UNTRACKED files.
  /// If *all* contents are IGNORED, then the directory is IGNORED; if
  /// any contents are not IGNORED, then the directory is UNTRACKED.
  /// This is extra work that may not matter in many cases.  This flag
  /// turns off that scan and immediately labels an untracked directory
  /// as UNTRACKED (changing the behavior to not match core Git).
  /// </summary>
  EnableFastUntrackedDirs = 1 << 14,

  /// <summary>
  /// When diff finds a file in the working directory with stat
  /// information different from the index, but the OID ends up being the
  /// same, write the correct stat information into the index. Note:
  /// without this flag, diff will always leave the index untouched.
  /// </summary>
  UpdateIndex = 1 << 15,

  /// <summary>
  /// Include unreadable files in the diff
  /// </summary>
  IncludeUnreadable = 1 << 16,

  /// <summary>
  /// Include unreadable files in the diff as UNTRACKED.
  /// </summary>
  IncludeUnreadableAsUntracked = 1 << 17,

  // Options controlling how output will be generated

  /// <summary>
  /// Use a heuristic that takes indentation and whitespace into account
  /// which generally can produce better diffs when dealing with ambiguous
  /// diff hunks.
  /// </summary>
  IndentHeuristic = 1 << 18,

  /// <summary>
  /// Ignore blank lines
  /// </summary>
  IgnoreBlankLines = 1 << 19,

  /// <summary>
  /// Treat all files as text, disabling binary attributes & detection
  /// </summary>
  ForceText = 1 << 20,

  /// <summary>
  /// Treat all files as binary, disabling text diffs
  /// </summary>
  ForceBinary = 1 << 21,

  /// <summary>
  /// Ignore all whitespace
  /// </summary>
  IgnoreWhitespace = 1 << 22,

  /// <summary>
  /// Ignore changes in amount of whitespace
  /// </summary>
  IgnoreWhitespaceChange = 1 << 23,

  /// <summary>
  /// Ignore whitespace at end of line
  /// </summary>
  IgnoreWhitespaceEol = 1 << 24,

  /// <summary>
  /// When generating patch text, include the content of untracked
  /// files. This automatically turns on <see cref="IncludeUntracked"/> but
  /// it does not turn on <see cref="RecurseUntrackedDirs"/>. Add that
  /// flag if you want the content of every single UNTRACKED file.
  /// </summary>
  ShowUntrackedContent = 1 << 25,

  /// <summary>
  /// When generating output, include the names of unmodified files if
  /// they are included in the <see cref="IGitDiff"/>. Normally these are skipped in
  /// the formats that list files (e.g. name-only, name-status, raw).
  /// Even with this, these will not be included in patch format.
  /// </summary>
  ShowUnmodified = 1 << 26,

  /// <summary>
  /// Use the "patience diff" algorithm
  /// </summary>
  Patience = 1 << 28,

  /// <summary>
  /// Take extra time to find minimal diff
  /// </summary>
  Minimal = 1 << 29,

  /// <summary>
  /// Include the necessary deflate / delta information so that `git-apply`
  /// can apply given diff information to binary files.
  /// </summary>
  ShowBinary = 1 << 30,
}

public enum IgnoreSubmodulesMode
{
  /// <summary>
  /// Use the submodule configuration, ignore local changes
  /// </summary>
  Unspecified = -1,

  /// <summary>
  /// don't ignore any change - i.e. even an
  /// untracked file, will mark the submodule as dirty.  Ignored files are
  /// still ignored, of course.
  /// </summary>
  None = 0,

  /// <summary>
  /// ignore untracked files; only changes
  /// to tracked files, or the index or the HEAD commit will matter.
  /// </summary>
  Untracked = 2,

  ///<summary>
  /// ignore changes in the working directory,
  /// only considering changes if the HEAD of submodule has moved from the
  /// value in the superproject.
  /// </summary>
  Dirty = 3,

  ///<summary>
  /// never check if the submodule is dirty
  /// </summary>
  All = 4,

}

/// <summary>
/// What type of change is described by a git_diff_delta?
/// <para/>
/// <see cref="Renamed"/> and <see cref="Copied"/> will only show up if you run
/// `git_diff_find_similar()` on the diff object.
/// <para/>
/// <see cref="Typechange"/> only shows up given <see cref="GitDiffOptionFlags.IncludeTypeChange"/>
/// in the option flags(otherwise type changes will be split into ADDED /
/// DELETED pairs).
/// </summary>
public enum GitDeltaType
{
  /// <summary>
  /// no changes
  /// </summary>
  Unmodified = 0,

  /// <summary>
  /// entry does not exist in old version
  /// </summary>
  Added = 1,

  /// <summary>
  /// entry does not exist in new version
  /// </summary>
  Deleted = 2,

  /// <summary>
  /// entry content changed between old and new
  /// </summary>
  Modified = 3,

  /// <summary>
  /// entry was renamed between old and new
  /// </summary>
  Renamed = 4,

  /// <summary>
  /// entry was copied from another old entry
  /// </summary>
  Copied = 5,

  /// <summary>
  /// entry is ignored item in workdir
  /// </summary>
  Ignored = 6,

  /// <summary>
  /// entry is untracked item in workdir
  /// </summary>
  Untracked = 7,

  /// <summary>
  /// type of entry changed between old and new
  /// </summary>
  Typechange = 8,

  /// <summary>
  /// entry is unreadable
  /// </summary>
  Unreadable = 9,

  /// <summary>
  /// entry in the index is conflicted
  /// </summary>
  Conflicted = 10,
}

/// <summary>
/// Description of changes to one entry.
/// <para/>
/// A `delta` is a file pair with an old and new revision.The old version
/// may be absent if the file was just created and the new version may be
/// absent if the file was deleted.A diff is mostly just a list of deltas.
/// <para/>
/// When iterating over a diff, this will be passed to most callbacks and
/// you can use the contents to understand exactly what has changed.
/// <para/>
/// The `OldFile` represents the "from" side of the diff and the `NewFile`
/// represents to "to" side of the diff.  What those means depend on the
/// function that was used to generate the diff and will be documented below.
/// You can also use the <see cref="GitDiffOptionFlags.Reverse"/> flag to flip it around.
/// <para/>
/// Although the two sides of the delta are named "OldFile" and "NewFile",
/// they actually may correspond to entries that represent a file, a symbolic
/// link, a submodule commit id, or even a tree (if you are tracking type
/// changes or ignored/untracked directories).
/// <para/>
/// Under some circumstances, in the name of efficiency, not all fields will
/// be filled in, but we generally try to fill in as much as possible.One
/// example is that the "flags" field may not have either the <see cref="GitDiffFlags.Binary"/> or the
/// <see cref="GitDiffFlags.NotBinary"/> flag set to avoid examining file contents if you do not pass
/// in hunk and/or line callbacks to the diff foreach iteration function.It
/// will just use the git attributes for those files.
/// <para/>
/// The similarity score is zero unless you call `git_diff_find_similar()`
/// which does a similarity analysis of files in the diff.  Use that
/// function to do rename and copy detection, and to split heavily modified
/// files in add/delete pairs.  After that call, deltas with a status of
/// <see cref="GitDeltaType.Renamed"/> or <see cref="GitDeltaType.Copied"/> 
/// will have a similarity score between 0 and 100 indicating how similar 
/// the old and new sides are.
/// <para/>
/// If you ask `git_diff_find_similar` to find heavily modified files to
/// break, but to not* actually* break the records, then <see cref="GitDeltaType.Modified"/>
/// records may have a non-zero similarity score if the self-similarity is
/// below the split threshold.  To display this value like core Git, invert
/// the score (a la `printf("M%03d", 100 - delta->similarity)`).
/// </summary>
/// <param name="Status"></param>
/// <param name="Flags"></param>
/// <param name="Similarity">for <see cref="GitDeltaType.Renamed"/> and <see cref="GitDeltaType.Copied"/>, value 0-100</param>
/// <param name="NFiles">number of files in this delta</param>
/// <param name="OldFile"></param>
/// <param name="NewFile"></param>
public record GitDiffDelta(
  GitDeltaType Status,
  GitDiffFlags Flags,
  UInt16 Similarity,
  UInt16 NFiles,
  GitDiffFile? OldFile,
  GitDiffFile? NewFile);

/// <summary>
/// Diff notification callback function.
/// <para/>
/// The callback will be called for each file, just before the <see cref="GitDiffDelta"/>
/// gets inserted into the diff.
/// </summary>
/// <param name="diffSoFar">The diff until now</param>
/// <param name="delta"></param>
/// <param name="mathedPathspec"></param>
/// <returns>
/// Return 0 to insert the delta into the diff; a positive value will skip; a negative value will abort.
/// </returns>
public delegate int DiffNotifyHandler(IGitDiff diffSoFar, GitDiffDelta delta, string? mathedPathspec);

/// <summary>
/// Diff progress callback.
/// <para/>
/// Called before each file comparison.
/// </summary>
/// <param name="diffSoFar">The diff being generated.</param>
/// <param name="oldPath">The path to the old file or null.</param>
/// <param name="newPath">The path to the new file or null.</param>
/// <returns>Whether to abort the diff process.</returns>
public delegate GitOperationContinuation DiffProgressHandler(IGitDiff diffSoFar, string? oldPath, string? newPath);

public record GitDiffOptions
{
  /// <summary>
  /// A combination of <see cref="GitDiffOptionFlags"/> values.
  /// Defaults to <see cref="GitDiffOptionFlags.Normal"/>
  /// </summary>
  public GitDiffOptionFlags Flags { get; init; } = GitDiffOptionFlags.Normal;

  // options controlling which files are in the diff

  /// <summary>
  /// Overrides the submodule ignore setting for all submodules in the diff.
  /// </summary>
  public IgnoreSubmodulesMode IgnoreSubmodules { get; init; } = IgnoreSubmodulesMode.Unspecified;

  /// <summary>
  /// An array of paths / fnmatch patterns to constrain diff.
  /// All paths are included by default.
  /// </summary>
  public IReadOnlyCollection<string>? Pathspec { get; init; }

  /// <summary>
  /// An optional callback function, notifying the consumer of changes to
  /// the diff as new deltas are added.
  /// </summary>
  public DiffNotifyHandler? Notify { get; init; }

  /// <summary>
  /// An optional callback function, notifying the consumer of which files
  /// are being examined as the diff is generated.
  /// </summary>
  public DiffProgressHandler? Progress { get; init; }

  /// <summary>
  /// The number of unchanged lines that define the boundary of a hunk
  /// (and to display before and after the actual changes).
  /// </summary>
  public UInt32 ContextLines { get; init; } = 3;

  /// <summary>
  /// The maximum number of unchanged lines between hunk boundaries before
  /// the hunks will be merged into one.Defaults to 0.
  /// </summary>
  public UInt32 InterhunkLines { get; init; } = 0;

  /// <summary>
  /// The object ID type to emit in diffs; this is used by functions
  /// that operate without a repository - namely `git_diff_buffers`,
  /// or `git_diff_blobs` and `git_diff_blob_to_buffer` when one blob
  /// is `NULL`.
  ///
  /// This may be omitted(set to `0`). If a repository is available,
  /// the object ID format of the repository will be used.If no
  /// repository is available then the default is `GIT_OID_SHA`.
  ///
  /// If this is specified and a repository is available, then the
  /// specified `oid_type` must match the repository's object ID
  /// format.
  /// </summary>
  public GitOidType OidType { get; init; } = 0;

  /// <summary>
  /// The abbreviation length to use when formatting object ids.
  /// Defaults to the value of 'core.abbrev' from the config, or 7 if unset.
  /// </summary>
  public UInt16 IdAbbrev { get; init; } = 0;

  /// <summary>
  /// A size (in bytes) above which a blob will be marked as binary
  /// automatically; pass a negative value to disable.
  /// Defaults to 512MB.
  /// </summary>
  public Int64 MaxSize { get; init; } = 0;

  /// <summary>
  /// The virtual "directory" prefix for old file names in hunk headers.
  /// Default is "a".
  /// </summary>
  public string? OldPrefix { get; init; }

  /// <summary>
  /// The virtual "directory" prefix for new file names in hunk headers.
  /// Default is "b".
  /// </summary>
  public string? NewPrefix { get; init; }
}
