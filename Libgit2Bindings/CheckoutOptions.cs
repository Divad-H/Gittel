using System.Collections.Immutable;

namespace Libgit2Bindings;

/// <summary>
/// Checkout behavior flags
/// </summary>
[Flags]
public enum CheckoutStrategy
{
  /// <summary>
  /// Default is a dry run, no acual updates.
  /// </summary>
  None = 0,
  /// <summary>
  /// Allow safe updates that cannot overwrite uncommitted data.
  /// If the uncommitted changes don't conflict with the checked out files,
	/// the checkout will still proceed, leaving the changes intact.
	///
	/// Mutually exclusive with <see cref="Force"/>.
	/// <see cref="Force"/> takes precedence over <see cref="Safe"/>.
  /// </summary>
  Safe = 1 << 0,
  /// <summary>
  /// Allow all updates to force working directory to look like the index
  /// 
  /// Mutually exclusive with <see cref="Safe"/>.
	/// <see cref="Force"/> takes precedence over <see cref="Safe"/>.
  /// </summary>
  Force = 1 << 1,
  /// <summary>
  /// Allow checkout to recreate missing files
  /// </summary>
  RecreateMissing = 1 << 2,
  /// <summary>
  /// Allow checkout to make safe updates even if conflicts are found
  /// </summary>
  AllowConflicts = 1 << 3,
  /// <summary>
  /// Remove untracked files not in index (that are not ignored)
  /// </summary>
  RemoveUntracked = 1 << 4,
  /// <summary>
  /// Remove ignored files not in index
  /// </summary>
  RemoveIgnored = 1 << 5,
  /// <summary>
  /// Only update existing files, don't create new ones
  /// </summary>
  UpdateOnly = 1 << 6,
  /// <summary>
  /// Normally checkout updates index entries as it goes; this stops that.
  /// Implies <see cref="DontWriteIndex"/>.
  /// </summary>
  DontUpdateIndex = 1 << 7,
  /// <summary>
  /// Don't refresh index/config/etc before doing checkout
  /// </summary>
  NoRefresh = 1 << 8,
  /// <summary>
  /// Allow checkout to skip unmerged files
  /// </summary>
  SkipUnmerged = 1 << 9,
  /// <summary>
  /// For unmerged files, checkout stage 2 from index
  /// </summary>
  UseOurs = 1 << 10,
  /// <summary>
  /// For unmerged files, checkout stage 3 from index
  /// </summary>
  UseTheirs = 1 << 11,
  /// <summary>
  /// Treat pathspec as simple list of exact match file paths
  /// </summary>
  DisablePathspecMatch = 1 << 12,
  /// <summary>
  /// Ignore directories in use, they will be left empty
  /// </summary>
  SkipLockedDirectories = 1 << 13,
  /// <summary>
  /// Don't overwrite ignored files that exist in the checkout target.
  /// </summary>
  DontOverwriteIgnored = 1 << 14,
  /// <summary>
  /// Write normal merge files for conflicts.
  /// </summary>
  ConflictStyleMerge = 1 << 15,
  /// <summary>
  /// Include common ancestor data in diff3 format files for conflicts.
  /// </summary>
  ConflictStyleDiff3 = 1 << 16,
  /// <summary>
  /// Don't overwrite existing files or folders.
  /// </summary>
  DontRemoveExisting = 1 << 17,
  /// <summary>
  /// Normally checkout writes the index upon completion; this prevents that.
  /// </summary>
  DontWriteIndex = 1 << 18,
  /// <summary>
  /// Show what would be done by a checkout.  Stop after sending
	/// notifications; don't update the working directory or index.
  /// </summary>
  DryRun = 1 << 19,
  /// <summary>
  /// Include common ancestor data in diff3 format files for conflicts.
  /// </summary>
  ConflictStyleZDiff3 = 1 << 20,
  /// <summary>
  /// Recursively checkout submodules with same options (NOT IMPLEMENTED)
  /// </summary>
  //UpdateSubmodules = 1 << 21,
  /// <summary>
  /// Recursively checkout submodules if HEAD moved in super repo (NOT IMPLEMENTED)
  /// </summary>
  //UpdateSubmodulesIfChanged = 1 << 22,
}

/// <summary>
/// Checkout notification flags
///
/// Checkout will invoke an options notification callback(`notify_cb`) for
/// certain cases - you pick which ones via `notify_flags`:
///
/// Returning a non-zero value from this callback will cancel the checkout.
/// The non-zero return value will be propagated back and returned by the
/// git_checkout_... call.
///
/// Notification callbacks are made prior to modifying any files on disk,
/// so canceling on any notification will still happen prior to any files
/// being modified.
/// </summary>
[Flags]
public enum CheckoutNotifyFlags
{
  None = 0,
  /// <summary>
  /// Invokes checkout on conflicting paths.
  /// </summary>
  Conflict = 1 << 0,
  /// <summary>
  /// Notifies about "dirty" files, i.e. those that do not need an update
	/// but no longer match the baseline.  Core git displays these files when
	/// checkout runs, but won't stop the checkout.
  /// </summary>
  Dirty = 1 << 1,
  /// <summary>
  /// Sends notification for any file changed.
  /// </summary>
  Updated = 1 << 2,
  /// <summary>
  /// Notifies about untracked files.
  /// </summary>
  Untracked = 1 << 3,
  /// <summary>
  /// Notifies about ignored files.
  /// </summary>
  Ignored = 1 << 4,
  All = 0x0FFFF,
}

/// <summary>
/// Checkout notification callback function
/// </summary>
public delegate GitOperationContinuation CheckoutNotifyHandler(CheckoutNotifyFlags why, string path, DiffFile? baseline, DiffFile? target, DiffFile? workdir);
public delegate void CheckoutProgressHandler(string? path, UInt64 completedSteps, UInt64 totalSteps);

public record PerformanceData(UInt64 MkdirCalls, UInt64 StatCalls, UInt64 ChmodCalls);
public delegate void PerformanceDataHandler(PerformanceData data);

public record CheckoutOptions
{
  /// <summary>
  /// Default will be a safe checkout 
  /// </summary>
  public CheckoutStrategy Strategy { get; init; } = CheckoutStrategy.Safe;
  /// <summary>
  /// Don't apply filters like CRLF conversion 
  /// </summary>
  public bool DisableFilters { get; init; }

  /// <summary>
  /// <see cref="CheckoutNotifyFlags"/>
  /// </summary>
  public CheckoutNotifyFlags NotifyFlags { get; init; } = CheckoutNotifyFlags.None;
  /// <summary>
  /// Optional callback to get notifications on specific file states.
  /// <see cref="CheckoutNotifyFlags"/>
  /// </summary>
  public CheckoutNotifyHandler? NotifyCallback { get; init; }
  /// <summary>
  /// Optional callback to notify the consumer of checkout progress.
  /// </summary>
  public CheckoutProgressHandler? ProgressCallback { get; init; }
  /// <summary>
  /// A list of wildmatch patterns or paths.
	///
	/// By default, all paths are processed.If you pass an array of wildmatch
	/// patterns, those will be used to filter which paths should be taken into
	/// account.
  ///
  /// Use <see cref="CheckoutStrategy.DisablePathspecMatch"/> to treat as a simple list.
  /// </summary>
  public IReadOnlyCollection<string>? Paths { get; init; }

  /// <summary>
  /// The expected content of the working directory; defaults to HEAD. 
  /// If the working directory does not match this baseline information, that will produce a checkout conflict.
  /// </summary>
  public IGitTree? Baseline { get; init; }

  /// <summary>
  /// Like <see cref="Baseline"/> above, though expressed as an index. This option overrides <see cref="Baseline"/>.
  /// </summary>
  public IGitIndex? BaselineIndex { get; init; }

  /// <summary>
  /// alternative checkout path to workdir
  /// </summary>
  public string? TargetDirectory { get; init; }

  /// <summary>
  /// the name of the common ancestor side of conflicts
  /// </summary>
  public string? AncestorLabel { get; init; }

  /// <summary>
  /// the name of the "our" side of conflicts
  /// </summary>
  public string? OurLabel { get; init; }

  /// <summary>
  /// the name of the "their" side of conflicts
  /// </summary>
  public string? TheirLabel { get; init; }

  /// <summary>
  /// Optional callback to notify the consumer of performance data. 
  /// </summary>
  public PerformanceDataHandler? PerformanceDataCallback { get; init; }
}
  
