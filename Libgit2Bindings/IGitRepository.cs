namespace Libgit2Bindings;

public interface IGitRepository : IDisposable
{
  /// <summary>
  /// Retrieve and resolve the reference pointed at by HEAD.
  /// </summary>
  IGitReference GetHead();

  /// <summary>
  /// Make the HEAD point to the specified reference.
  /// </summary>
  /// <param name="refName">Canonical name of the reference the HEAD should point at</param>
  void SetHead(string refName);

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
  /// Updates files in the working tree to match the content of the index.
  /// </summary>
  /// <param name="index">index to be checked out (or null to use repository index)</param>
  /// <param name="options">specifies checkout options</param>
  void CheckoutIndex(IGitIndex? index = null, CheckoutOptions? options = null);

  /// <summary>
  /// Lookup a branch by its name in a repository.
  /// </summary>
  /// <param name="branchName">Name of the branch to be looked up; this name is validated for consistency</param>
  /// <param name="branchType">Type of the branch</param>
  /// <returns>The branch </returns>
  IGitReference LookupBranch(string branchName, BranchType branchType);

  /// <summary>
  /// Returns an iterable of all requested branches in the repository.
  /// </summary>
  /// <remarks>
  /// See <see cref="GitReferenceBox"/> for information about how to control the lifetime of the iterated branches.
  /// </remarks>
  /// <param name="filterTypes">Filtering flags for the branch listing</param>
  /// <returns>The iterable</returns>
  IEnumerable<GitReferenceBox> LookupBranches(BranchType filterTypes);

  /// <summary>
  /// Find the remote name of a remote-tracking branch
  /// </summary>
  /// <remarks>
  /// This will return the name of the remote whose fetch refspec is matching the given branch. 
  /// E.g. given a branch "refs/remotes/test/master", it will extract the "test" part. If refspecs 
  /// from multiple remotes match, the function will throw an error with code GIT_EAMBIGUOUS.
  /// </remarks>
  /// <param name="completeTrackingBranchName">complete name of the remote tracking branch.</param>
  /// <returns>The remote name</returns>
  string GetRemoteNameFromBranch(string completeTrackingBranchName);

  /// <summary>
  /// Retrieve the upstream merge of a local branch
  /// </summary>
  /// <remarks>
  /// This will return the currently configured "branch.*.merge" for a given branch. This branch must be local.
  /// </remarks>
  /// <param name="fullBranchName">the full name of the branch</param>
  /// <returns>the name of the upsream merge</returns>
  string GetBranchUpstreamMerge(string fullBranchName);

  /// <summary>
  /// Get the upstream name of a branch
  /// </summary>
  /// <remarks>
  /// Given a local branch, this will return its remote-tracking branch information, 
  /// as a full reference name, ie. "feature/nice" would become "refs/remote/origin/feature/nice", 
  /// depending on that branch's configuration.
  /// </remarks>
  /// <param name="localBranchName">reference name of the local branch.</param>
  /// <returns>the name of the upstream</returns>
  string GetBranchUpstreamName(string localBranchName);

  /// <summary>
  /// Retrieve the upstream remote of a local branch
  /// </summary>
  /// <remarks>
  /// This will return the currently configured "branch.*.remote" for a given branch.
  /// This branch must be local.
  /// </remarks>
  /// <param name="fullBranchName">the full name of the branch</param>
  /// <returns>The name of the remote</returns>
  string GetBranchUpstreamRemote(string fullBranchName);

  /// <summary>
  /// Create a new branch pointing at a target commit
  /// </summary>
  /// <param name="branchName">Name for the branch; this name is validated for consistency. 
  /// It should also not conflict with an already existing branch name.</param>
  /// <param name="target">Commit to which this branch should point. This object must belong to this <see cref="IGitRepository"/>.</param>
  /// <param name="force">Overwrite existing branch.</param>
  /// <returns>The branch</returns>
  IGitReference CreateBranch(string branchName, IGitCommit target, bool force);

  /// <summary>
  /// Create a new branch pointing at a target commit
  /// </summary>
  /// <remarks>
  /// This behaves like <see cref="CreateBranch(string, IGitCommit, bool)"/> but takes an annotated commit, 
  /// which lets you specify which extended sha syntax string was 
  /// specified by a user, allowing for more exact reflog messages.
  /// </remarks>
  /// <param name="branchName">Name for the branch; this name is validated for consistency. 
  /// It should also not conflict with an already existing branch name.</param>
  /// <param name="target">Commit to which this branch should point. This object must belong to this <see cref="IGitRepository"/>.</param>
  /// <param name="force">Overwrite existing branch.</param>
  /// <returns>The branch</returns>
  IGitReference CreateBranch(string branchName, IGitAnnotatedCommit target, bool force);

  /// <summary>
  /// Cherry-pick the given commit, producing changes in the index and working directory.
  /// </summary>
  /// <remarks>
  /// This operation does not create a commit.
  /// </remarks>
  /// <param name="commit">the commit to cherry-pick</param>
  /// <param name="options">the cherry-pick options (or null for defaults)</param>
  void Cherrypick(IGitCommit commit, CherrypickOptions? options = null);

  /// <summary>
  /// Cherry-picks the given commit against the given "our" commit,
  /// producing an index that reflects the result of the cherry-pick.
  /// </summary>
  /// <remarks>
  /// The returned index must be diposed
  /// </remarks>
  /// <param name="cherrypickCommit">the commit to cherry-pick</param>
  /// <param name="ourCommit">the commit to cherry-pick against (eg, HEAD)</param>
  /// <param name="mainline">the parent of the `cherrypickCommit`, if it is a merge</param>
  /// <param name="options">the merge options (or null for defaults)</param>
  /// <returns>the index result</returns>
  IGitIndex CherrypickCommit(
    IGitCommit cherrypickCommit, IGitCommit ourCommit, UInt32 mainline, MergeOptions? options = null);

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
    string message, IGitTree tree, IReadOnlyCollection<IGitCommit>? parents);

  /// <summary>
  /// Create a commit and return the commit object content
  /// </summary>
  /// <param name="author">Signature with author and author time of commit</param>
  /// <param name="committer">Signature with committer and * commit time of commit</param>
  /// <param name="message">Full message for this commit</param>
  /// <param name="tree">
  /// An instance of a <see cref="IGitTree"/> object that will be used as the tree for the commit. 
  /// This tree object must also be owned by the given repo.
  /// </param>
  /// <param name="parents">
  /// The parents that will be used for this commit. The parent commits must be owned by the repo.
  /// </param>
  /// <returns>the commit object content</returns>
  byte[] CreateCommitObject(IGitSignature author, IGitSignature committer,
    string message, IGitTree tree, IReadOnlyCollection<IGitCommit>? parents);

  /// <summary>
  /// Create a commit object from the given buffer and signature
  /// </summary>
  /// <remarks>
  /// Given the unsigned commit object's contents, its signature and the header field in which to store 
  /// the signature, attach the signature to the commit and write it into the given repository.
  /// </remarks>
  /// <param name="commitContent">the content of the unsigned commit object</param>
  /// <param name="signature">the signature to add to the commit. Leave `null` to create a commit 
  /// without adding a signature field.</param>
  /// <param name="signatureField">which header field should contain this signature. Leave `null` 
  /// for the default of "gpgsig"</param>
  /// <returns>the resulting commit id</returns>
  GitOid CreateCommitWithSignature(string commitContent, string? signature, string? signatureField);

  /// <summary>
  /// Extract the signature from a commit
  /// </summary>
  /// <param name="commitId">the commit from which to extract the data</param>
  /// <param name="signatureField">the name of the header field containing the signature block; 
  /// pass `null` to extract the default 'gpgsig'</param>
  /// <returns>A tuple containing the signature block and signed data; 
  /// this is the commit contents minus the signature block</returns>
  (byte[] Signature, byte[] SignedData) ExtractCommitSignature(GitOid commitId, string? signatureField);

  /// <summary>
  /// Lookup a commit object from a repository.
  /// </summary>
  /// <param name="oid">identity of the commit to locate. If the object is an annotated tag it will be peeled back to the commit.</param>
  /// <returns>the looked up commit</returns>
  IGitCommit LookupCommit(GitOid oid);

  /// <summary>
  /// Lookup a commit object from a repository, given a prefix of its identifier (short id).
  /// </summary>
  /// <param name="shortId">identity of the commit to locate. 
  /// If the object is an annotated tag it will be peeled back to the commit.</param>
  /// <returns>the looked up commit</returns>
  IGitCommit LookupCommitPrefix(byte[] shortId);

  /// <summary>
  /// Lookup a commit object from a repository, given a prefix of its identifier (short id).
  /// </summary>
  /// <param name="shortSha">identity of the commit to locate. 
  /// If the object is an annotated tag it will be peeled back to the commit.</param>
  /// <returns>the looked up commit</returns>
  IGitCommit LookupCommitPrefix(string shortSha);

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

  /// <summary>
  /// Create a new mailmap instance from a repository, loading mailmap files based on the repository's configuration.
  /// </summary>
  /// <remarks>
  /// Mailmaps are loaded in the following order: 
  /// <para>1. '.mailmap' in the root of the repository's working directory, if present.</para>
  /// <para>2. The blob object identified by the 'mailmap.blob' config entry, if set.
  /// [NOTE: 'mailmap.blob' defaults to 'HEAD:.mailmap' in bare repositories]</para>
  /// <para>3. The path in the 'mailmap.file' config entry, if set.</para>
  /// </remarks>
  /// <returns>the new mailmap</returns>
  IGitMailmap GetMailmap();

  /// <summary>
  /// Check if a repository is bare
  /// </summary>
  /// <returns>true if the repository is bare, false otherwise.</returns>
  bool IsBare();

  /// <summary>
  /// Add a remote with the default fetch refspec to the repository's configuration.
  /// </summary>
  /// <param name="name">the remote's name</param>
  /// <param name="url">the remote's url</param>
  /// <returns>the resulting remote</returns>
  IGitRemote CreateRemote(string name, string url);

  /// <summary>
  /// Get the information for a particular remote
  /// </summary>
  /// <param name="name">the remote's name</param>
  /// <returns>the new remote object</returns>
  IGitRemote LookupRemote(string name);

  /// <summary>
  /// Creates a <see cref="IGitAnnotatedCommit"/> from the given fetch head data.
  /// </summary>
  /// <param name="branchName">name of the (remote) branch</param>
  /// <param name="remoteUrl">url of the remote</param>
  /// <param name="Id">the commit object id of the remote branch</param>
  /// <returns>The annotated commit</returns>
  IGitAnnotatedCommit GetAnnotatedCommitFromFetchhead(string branchName, string remoteUrl, GitOid Id);

  /// <summary>
  /// Creates a <see cref="IGitAnnotatedCommit"/> from the given reference.
  /// </summary>
  /// <param name="gitReference">reference to use to lookup the <see cref="IGitAnnotatedCommit"/></param>
  /// <returns>The annotated commit</returns>
  IGitAnnotatedCommit GetAnnotatedCommitFromRef(IGitReference gitReference);

  /// <summary>
  /// Creates a <see cref="IGitAnnotatedCommit"/> from the given refspec.
  /// </summary>
  /// <remarks>
  /// See man gitrevisions, or http://git-scm.com/docs/git-rev-parse.html#_specifying_revisions 
  /// for information on the syntax accepted.
  /// </remarks>
  /// <param name="refspec">the extended sha syntax string to use to lookup the commit</param>
  /// <returns>The annotated commit</returns>
  IGitAnnotatedCommit GetAnnotatedCommitFromRevspec(string refspec);

  /// <summary>
  /// Creates a <see cref="IGitAnnotatedCommit"/> from the given id.
  /// </summary>
  /// <param name="id">the commit object id to lookup</param>
  /// <returns>The annotated commit</returns>
  IGitAnnotatedCommit AnnotatedCommitLookup(GitOid id);

  /// <summary>
  /// Create a diff between a tree and the working directory.
  /// </summary>
  /// <remarks>
  /// The tree you provide will be used for the "OldFile" side of the delta, 
  /// and the working directory will be used for the "NewFile" side.
  /// <para/>
  /// This is not the same as git diff<treeish> or git diff-index<treeish>.
  /// Those commands use information from the index, whereas this function 
  /// strictly returns the differences between the tree and the files in the 
  /// working directory, regardless of the state of the index.
  /// Use git_diff_tree_to_workdir_with_index to emulate those commands.
  /// <para/>
  /// To see difference between this and git_diff_tree_to_workdir_with_index, 
  /// consider the example of a staged file deletion where the file has then 
  /// been put back into the working dir and further modified.The tree-to-workdir 
  /// diff for that file is 'modified', but git diff would show status 'deleted' 
  /// since there is a staged delete.
  /// </remarks>
  /// <param name="oldTree">A <see cref="IGitTree"/> object to diff from, or null for empty tree.</param>
  /// <param name="options">Structure with options to influence diff or null for defaults.</param>
  /// <returns>The diff</returns>
  IGitDiff DiffTreeToWorkdir(IGitTree? oldTree, GitDiffOptions? options = null);

  /// <summary>
  /// Lookup a blob object from a repository.
  /// </summary>
  /// <param name="oid">identity of the blob to locate.</param>
  /// <returns>the looked up blob</returns>
  IGitBlob LookupBlob(GitOid oid);

  /// <summary>
  /// Lookup a blob object from a repository, given a prefix of its identifier (short id).
  /// </summary>
  /// <param name="id">identity of the blob to locate.</param>
  /// <returns>the looked up blob</returns>
  IGitBlob LookupBlobByPrefix(byte[] shortId);

  /// <summary>
  /// Lookup a blob object from a repository, given a prefix of its identifier (short id).
  /// </summary>
  /// <param name="id">identity of the blob to locate.</param>
  /// <returns>the looked up blob</returns>
  IGitBlob LookupBlobByPrefix(string shortSha);

  /// <summary>
  /// Write an in-memory buffer to the ODB as a blob
  /// </summary>
  /// <param name="data">data to be written into the blob</param>
  /// <returns>the id of the written blob</returns>
  GitOid CreateBlob(byte[] data);

  /// <summary>
  /// Read a file from the filesystem and write its content to the Object Database as a loose blob
  /// </summary>
  /// <param name="path">file from which the blob will be created</param>
  /// <returns>the id of the written blob</returns>
  GitOid CreateBlobFromDisk(string path);

  /// <summary>
  /// Create a stream to write a new blob into the object db
  /// </summary>
  /// <remarks>
  /// This function may need to buffer the data on disk and will in general not be the 
  /// right choice if you know the size of the data to write. If you have data in memory, 
  /// use <see cref="CreateBlob(byte[])"/>. If you do not, but know the size of the 
  /// contents (and don't want/need to perform filtering), use git_odb_open_wstream().
  /// <para/>
  /// Call <see cref="AbstractGitWriteStream.Commit()"/> to commit the write to the object 
  /// db and get the object id. This operation will close the stream.
  /// <para/>
  /// If the hintpath parameter is filled, it will be used to determine what git filters 
  /// should be applied to the object before it is written to the object database.
  /// <para/>
  /// </remarks>
  /// <param name="hintpath">If not null, will be used to select data filters to apply onto the 
  /// content of the blob to be created.</param>
  /// <returns>the stream into which to write</returns>
  AbstractGitWriteStream CreateBlobFromStream(string? hintpath);

  /// <summary>
  /// Read a file from the working folder of a repository and write it to 
  /// the Object Database as a loose blob
  /// </summary>
  /// <param name="relativePath">file from which the blob will be created, relative 
  /// to the repository's working dir</param>
  /// <returns>the id of the written blob</returns>
  GitOid CreateBlobFromWorkdir(string relativePath);

  /// <summary>
  /// Get the blame for a single file
  /// </summary>
  /// <param name="path">path to file to consider</param>
  /// <param name="options">options for the blame operation</param>
  /// <returns>the blame object</returns>
  IGitBlame BlameFile(string path, GitBlameOptions? options = null);

  /// <summary>
  /// Lookup a reference to one of the objects in a repository.
  /// </summary>
  /// <remarks>
  /// The 'type' parameter must match the type of the object in the odb; 
  /// the method will fail otherwise.The special value <see cref="GitObjectType.Any"/> 
  /// may be passed to let the method guess the object's type.
  /// </remarks>
  /// <param name="oid">the unique identifier for the object</param>
  /// <param name="type">the type of the object</param>
  /// <returns>the looked-up object</returns>
  IGitObject LookupObject(GitOid oid, GitObjectType type);

  /// <summary>
  /// Lookup a reference to one of the objects in a repository, given a prefix of its identifier (short id).
  /// </summary>
  /// <remarks>
  /// The 'type' parameter must match the type of the object in the odb; 
  /// the method will fail otherwise.The special value <see cref="GitObjectType.Any"/> 
  /// may be passed to let the method guess the object's type.
  /// </remarks>
  /// <param name="shortId">a short identifier for the object</param>
  /// <param name="type">the type of the object</param>
  /// <returns>The looked-up object</returns>
  IGitObject LookupObjectByPrefix(string shortId, GitObjectType type);

  /// <summary>
  /// Describe a commit
  /// </summary>
  /// <remarks>
  /// Perform the describe operation on the current commit and the worktree. 
  /// After performing describe on HEAD, a status is run and the description 
  /// is considered to be dirty if there are.
  /// </remarks>
  /// <param name="options">the lookup options (or null for defaults)</param>
  /// <returns>The describe result</returns>
  IGitDescribeResult DescribeWorkdir(GitDescribeOptions? options = null);
}
