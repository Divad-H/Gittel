namespace Libgit2Bindings;

/// <summary>
/// Options for bypassing the git-aware transport on clone. Bypassing
/// it means that instead of a fetch, libgit2 will copy the object
/// database directory instead of figuring out what it needs, which is
/// faster. If possible, it will hardlink the files to save space.
/// </summary>
public enum CloneLocal
{
  /// <summary>
  /// Auto-detect (default), libgit2 will bypass the git-aware
  /// transport for local paths, but use a normal fetch for
  /// `file://` urls.
  /// </summary>
  LocalAuto = 0,
  /// <summary>Bypass the git-aware transport even for a `file://` url.</summary>
  Local = 1,
  /// <summary>Do no bypass the git-aware transport</summary>
  NoLocal = 2,
  /// <summary>
  /// Bypass the git-aware transport, but do not try to use
  /// hardlinks.
  /// </summary>
  LocalNoLinks = 3,
}

/// <summary>
/// Callback for customizing the creation of a repository when cloning.
/// </summary>
/// <param name="repository">The resulting repository</param>
/// <param name="path">path in which to create the repository</param>
/// <param name="bare">whether the repository is bare. This is the value from the <see cref="CloneOptions"/></param>
/// <returns>0, or a negative value to indicate an error</returns>
public delegate int RepositoryCreateHandler(out IGitRepository? repository, string path, bool bare);

/// <summary>
/// Callback for customizing the creation of a remote when cloning.
/// </summary>
/// <param name="remote">the resulting remote</param>
/// <param name="repository">the repository in which to create the remote</param>
/// <param name="name">the remote's name</param>
/// <param name="url">the remote's url</param>
/// <returns>0, or a negative value to indicate an error</returns>
public delegate int RemoteCreateHandler(out IGitRemote? remote, IGitRepository repository, string name, string url);

public class CloneOptions
{
  /// <summary>
  /// These options are passed to the checkout step.
  /// To disable checkout, set the <see cref="CheckoutStrategy"/> to <see cref="CheckoutStrategy.None"/>.
  /// </summary>
  public CheckoutOptions CheckoutOptions { get; init; } = new();
  /// <summary>
  /// Options which control the fetch, including callbacks. The callbacks are used for reporting
  /// fetch progress, and for acquiring credentials in the event they are needed.
  /// </summary>
  public FetchOptions FetchOptions { get; init; } = new();
  /// <summary>
  /// Set to false to create a standard repo, or true for a bare repo
  /// </summary>
  public bool Bare { get; init; } = false;
  /// <summary>
  /// Whether to use a fetch or copy the object database.
  /// </summary>
  public CloneLocal CloneLocal { get; init; } = CloneLocal.LocalAuto;
  /// <summary>
  /// The name of the branch to checkout. null means use the remote's default branch.
  /// </summary>
  public string? CheckoutBranch { get; init; } = null;
  /// <summary>
  /// A callback used to create the new repository into which to clone. If null, 
  /// the <see cref="Bare"/> field will be used to determine whether to create a bare repository.
  /// </summary>
  public RepositoryCreateHandler? RepositoryCreateCallback { get; init; } = null;

  /// <summary>
  /// A callback used to create the <see cref="IGitRemote"/>, prior to its being 
  /// used to perform the clone operation. See the documentation for <see cref="RemoteCreateHandler"/> 
  /// for details. This parameter may be null, indicating that GitClone should provide default behavior.
  /// </summary>
  public RemoteCreateHandler? RemoteCreateCallback { get; init; } = null;
}
