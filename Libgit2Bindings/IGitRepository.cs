namespace Libgit2Bindings;

public interface IGitRepository : IDisposable
{
  /// <summary>
  /// Retrieve and resolve the reference pointed at by HEAD.
  /// </summary>
  IGitReference GetHead();

  /// <summary>
  /// Get the path of this repository
  /// </summary>
  /// <returns>The path of the repository</returns>
  string GetPath();

  /// <summary>
  /// Get the path of the working directory of this repository
  /// </summary>
  /// <returns>the path to the working dir, if it exists</returns>
  string? GetWorkdir();

  /// <summary>
  /// Get the path of the shared common directory for this repository.
  /// <para>If the repository is bare, it is the root directory for the repository. 
  /// If the repository is a worktree, it is the parent repo's gitdir.
  /// Otherwise, it is the gitdir.</para>
  /// </summary>
  /// <returns>the path to the common dir</returns>
  string GetCommonDir();

  /// <summary>
  /// Updates files in the index and the working tree to match the content of the commit pointed at by HEAD
  /// </summary>
  /// <remarks>
  /// Note that this is not the correct mechanism used to switch branches; 
  /// do not change your HEAD and then call this method, that would leave you 
  /// with checkout conflicts since your working directory would then appear to be dirty. 
  /// Instead, checkout the target of the branch and then update HEAD using 
  /// git_repository_set_head to point to the branch you checked out.
  /// </remarks>
  /// <param name="options">specifies checkout options</param>
  void CheckoutHead(CheckoutOptions? options = null);


  /// <summary>
  /// Get the configuration file for this repository.
  /// </summary>
  /// <remarks>
  /// If a configuration file has not been set, the default config set for the repository will be returned, 
  /// including global and system configurations (if they are available).
  /// </remarks>
  /// <returns>
  /// The loaded configuration
  /// </returns>
  IGitConfig GetConfig();
}
