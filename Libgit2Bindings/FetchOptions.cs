using libgit2;

namespace Libgit2Bindings;

public enum GitFetchPruneOptions
{
  /// <summary>
  /// Use the setting from the configuration
  /// </summary>
  Unspecified,
  /// <summary>
  /// Force pruning on
  /// </summary>
  Prune,
  /// <summary>
  /// Force pruning off
  /// </summary>
  NoPrune
}

public enum GitRemoteTagDownloadOptions
{
  /// <summary>
  /// Use the setting from the configuration
  /// </summary>
  Unspecified,
  /// <summary>
  /// Ask the server for tags pointing to objects we're already downloading
  /// </summary>
  Auto,
  /// <summary>
  /// Don't ask for any tags beyond the refspecs
  /// </summary>
  None,
  /// <summary>
  /// Ask for all the tags
  /// </summary>
  All
}

public enum GitRemoteRedirectOptions
{
  /// <summary>
  /// the `http.followRedirects` configuration setting will be consulted.
  /// </summary>
  Unspecified = 0,
  /// <summary>
  /// Do not follow any off-site redirects at any stage of
	/// the fetch or push.
  /// </summary> 
  RedirectNone = 1,
  /// <summary>
  /// Allow off-site redirects only upon the initial request.
	/// This is the default.
  /// </summary>
  RedirectInitial = 2,
  /// <summary>
  /// Allow redirects at any stage in the fetch or push.
  /// </summary>
  RedirectAll = 3
}

public record FetchOptions
{
  public const int GitFetchDepthFull = 0;
  public const int GitFetchDepthUnshallow = 2147483647;

  /// <summary>
  /// Callbacks to use for the fetch operation
  /// </summary>
  public RemoteCallbacks Callbacks { get; init; } = new();

  /// <summary>
  /// Whether to perform a prune after the fetch
  /// </summary>
  public GitFetchPruneOptions Prune { get; init; } = GitFetchPruneOptions.Unspecified;

  /// <summary>
  /// Whether to write the results to FETCH_HEAD.
  /// </summary>
  public bool UpdateFetchHead { get; init; } = true;

  /// <summary>
  /// Determines how to behave regarding tags on the remote, such
	/// as auto-downloading tags for objects we're downloading or
	/// downloading all of them.
	///
	/// The default is to auto-follow tags.
  /// </summary>
  public GitRemoteTagDownloadOptions DownloadTags { get; init; } = GitRemoteTagDownloadOptions.Auto;

  /// <summary>
  /// Proxy options to use, by default no proxy is used.
  /// </summary>
  public ProxyOptions? ProxyOptions { get; init; }

  /// <summary>
  /// Depth of the fetch to perform, or `<see cref="GitFetchDepthFull"/>`
	/// (or `0`) for full history, or `<see cref="GitFetchDepthUnshallow"/>`
	/// to "unshallow" a shallow repository.
  /// <para/>
  /// The default is full(`<see cref="GitFetchDepthFull"/>` or `0`).
  /// </summary>
  public int Depth { get; init; } = GitFetchDepthFull;

  /// <summary>
  /// Whether to allow off-site redirects.  If this is not
	/// specified, the `http.followRedirects` configuration setting
	/// will be consulted.
  /// </summary>
  public GitRemoteRedirectOptions FollowRedirects { get; init; } = GitRemoteRedirectOptions.Unspecified;

  /// <summary>
  /// Extra headers for this fetch operation
  /// </summary>
  public IReadOnlyCollection<string>? CustomHeaders { get; init; }
}
