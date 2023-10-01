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
  /// Create a new action signature with default user and now timestamp.
  /// </summary>
  /// <remarks>
  /// This looks up the user.name and user.email from the configuration and uses the 
  /// current time as the timestamp, and creates a new signature based on that information.
  /// It will throw an error if either the user.name or user.email are not set.
  /// </remarks>
  /// <returns>new signature</returns>
  IGitSignature DefaultGitSignature();

  /// <summary>
  /// Create a new action signature with a timestamp of 'now'.
  /// </summary>
  /// <param name="name">name of the person</param>
  /// <param name="email">email of the person</param>
  /// <returns>new signature</returns>
  IGitSignature GitSignatureNow(string name, string email);

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

  /// <summary>
  /// Create a new commit from a <see cref="IGitTree"/>
  /// </summary>
  /// <param name="updateRef">
  /// If not null, name of the reference that will be updated to point to this commit. 
  /// If the reference is not direct, it will be resolved to a direct reference. 
  /// Use "HEAD" to update the HEAD of the current branch and make it point to this commit. 
  /// If the reference doesn't exist yet, it will be created. 
  /// If it does exist, the first parent must be the tip of this branch.
  /// </param>
  /// <param name="author">Signature with author and author time of commit</param>
  /// <param name="committer">Signature with committer and * commit time of commit</param>
  /// <param name="messageEncoding">
  /// The encoding for the message in the commit, represented with a standard encoding name. 
  /// E.g. "UTF-8". If null, no encoding header is written and UTF-8 is assumed.
  /// </param>
  /// <param name="message">Full message for this commit</param>
  /// <param name="tree">
  /// An instance of a <see cref="IGitTree"/> object that will be used as the tree for the commit. 
  /// This tree object must also be owned by the given repo.
  /// </param>
  /// <param name="parents">
  /// The parents that will be used for this commit. The parent commits must be owned by the repo.
  /// </param>
  /// <returns>The object id of the new commit</returns>
  GitOid CreateCommit(string? updateRef, IGitSignature author, IGitSignature committer, 
    string? messageEncoding, string message, IGitTree tree, IReadOnlyCollection<IGitCommit>? parents);

  /// <summary>
  /// Lookup a commit object from a repository.
  /// </summary>
  /// <param name="oid">identity of the commit to locate. If the object is an annotated tag it will be peeled back to the commit.</param>
  /// <returns>the looked up commit</returns>
  IGitCommit LookupCommit(GitOid oid);

  /// <summary>
  /// Get the index file for this repository.
  /// </summary>
  /// <returns>The index</returns>
  IGitIndex GetIndex();

  /// <summary>
  /// Lookup a tree object from the repository.
  /// </summary>
  /// <param name="oid">Identity of the tree to locate.</param>
  /// <returns>The looked up tree</returns>
  IGitTree LookupTree(GitOid oid);
}
