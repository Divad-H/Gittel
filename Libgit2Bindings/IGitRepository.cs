﻿namespace Libgit2Bindings;

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
  /// Create a new direct reference.
  /// </summary>
  /// <remarks>
  /// A direct reference (also called an object id reference) refers directly to a specific object id 
  /// (a.k.a. OID or SHA) in the repository. The id permanently refers to the object (although the 
  /// reference itself can be moved). For example, in libgit2 the direct ref "refs/tags/v0.17.0" 
  /// refers to OID 5b9fac39d8a76b9139667c26a63e6b3f204b3977.
  /// <para/>
  /// The direct reference will be created in the repository and written to the disk.
  /// <para/>
  /// Valid reference names must follow one of two patterns:
  /// <para/>
  /// Top-level names must contain only capital letters and underscores, and must begin and end with 
  /// a letter. (e.g. "HEAD", "ORIG_HEAD"). 2. Names prefixed with "refs/" can be almost anything. You 
  /// must avoid the characters '~', '^', ':', '\', '?', '[', and '*', and the sequences ".." and "@{" 
  /// which have special meaning to revparse.
  /// <para/>
  /// This function will return an error if a reference already exists with the given name unless force 
  /// is true, in which case it will be overwritten.
  /// <para/>
  /// The message for the reflog will be ignored if the reference does not belong in the standard set 
  /// (HEAD, branches and remote-tracking branches) and it does not have a reflog.
  /// </remarks>
  /// <param name="name">The name of the reference</param>
  /// <param name="id">The object id pointed to by the reference.</param>
  /// <param name="force">Overwrite existing references</param>
  /// <param name="logMessage">The one line long message to be appended to the reflog</param>
  /// <returns>the newly created reference</returns>
  IGitReference CreateReference(string name, GitOid id, bool force, string? logMessage);

  /// <summary>
  /// Conditionally create new direct reference
  /// </summary>
  /// <remarks>
  /// A direct reference (also called an object id reference) refers directly to a specific object 
  /// id (a.k.a. OID or SHA) in the repository. The id permanently refers to the object (although 
  /// the reference itself can be moved). For example, in libgit2 the direct ref "refs/tags/v0.17.0" 
  /// refers to OID 5b9fac39d8a76b9139667c26a63e6b3f204b3977.
  /// <para/>
  /// The direct reference will be created in the repository and written to the disk. 
  /// <para/>
  /// Valid reference names must follow one of two patterns:
  /// <para/>
  /// Top-level names must contain only capital letters and underscores, and must begin and end with 
  /// a letter. (e.g. "HEAD", "ORIG_HEAD"). 2. Names prefixed with "refs/" can be almost anything.
  /// You must avoid the characters '~', '^', ':', '\', '?', '[', and '*', and the sequences ".." 
  /// and "@{" which have special meaning to revparse.
  /// <para/>
  /// This function will return an error if a reference already exists with the given name unless force 
  /// is true, in which case it will be overwritten.
  /// <para/>
  /// The message for the reflog will be ignored if the reference does not belong in the standard 
  /// set (HEAD, branches and remote-tracking branches) and it does not have a reflog.
  /// <para/>
  /// It will throw if the reference's value at the time of updating does not match the one passed 
  /// through currentId (i.e. if the ref has changed since the user read it).
  /// </remarks>
  /// <param name="name">The name of the reference</param>
  /// <param name="id">The object id pointed to by the reference.</param>
  /// <param name="force">Overwrite existing references</param>
  /// <param name="currentId">The expected value of the reference at the time of update</param>
  /// <param name="logMessage">The one line long message to be appended to the reflog</param>
  /// <returns>the newly created reference</returns>
  IGitReference CreateMatchingReference(string name, GitOid id, bool force, GitOid currentId, string? logMessage);

  /// <summary>
  /// Create a new symbolic reference.
  /// </summary>
  /// <remarks>
  /// A symbolic reference is a reference name that refers to another reference name. If the other name 
  /// moves, the symbolic name will move, too. As a simple example, the "HEAD" reference might refer to 
  /// "refs/heads/master" while on the "master" branch of a repository.
  /// <para/>
  /// The symbolic reference will be created in the repository and written to the disk.
  /// <para/>
  /// Valid reference names must follow one of two patterns:
  /// <para/>
  /// Top-level names must contain only capital letters and underscores, and must begin and end with a 
  /// letter. (e.g. "HEAD", "ORIG_HEAD"). 2. Names prefixed with "refs/" can be almost anything.You 
  /// must avoid the characters '~', '^', ':', '\', '?', '[', and '*', and the sequences ".." and "@{" 
  /// which have special meaning to revparse.
  /// <para/>
  /// This function will return an error if a reference already exists with the given name unless force 
  /// is true, in which case it will be overwritten.
  /// <para/>
  /// The message for the reflog will be ignored if the reference does not belong in the standard set 
  /// (HEAD, branches and remote-tracking branches) and it does not have a reflog.
  /// </remarks>
  /// <param name="name">The name of the reference</param>
  /// <param name="target">The target of the reference</param>
  /// <param name="force">Overwrite existing references</param>
  /// <param name="logMessage">The one line long message to be appended to the reflog</param>
  /// <returns>The newly created reference</returns>
  IGitReference CreateSymbolicReference(string name, string target, bool force, string? logMessage);

  /// <summary>
  /// Create a new symbolic reference.
  /// </summary>
  /// <remarks>
  /// A symbolic reference is a reference name that refers to another reference name. If the other name 
  /// moves, the symbolic name will move, too. As a simple example, the "HEAD" reference might refer to 
  /// "refs/heads/master" while on the "master" branch of a repository.
  /// <para/>
  /// The symbolic reference will be created in the repository and written to the disk.
  /// <para/>
  /// Valid reference names must follow one of two patterns:
  /// <para/>
  /// Top-level names must contain only capital letters and underscores, and must begin and end with a 
  /// letter. (e.g. "HEAD", "ORIG_HEAD"). 2. Names prefixed with "refs/" can be almost anything.You 
  /// must avoid the characters '~', '^', ':', '\', '?', '[', and '*', and the sequences ".." and "@{" 
  /// which have special meaning to revparse.
  /// <para/>
  /// This function will return an error if a reference already exists with the given name unless force 
  /// is true, in which case it will be overwritten.
  /// <para/>
  /// The message for the reflog will be ignored if the reference does not belong in the standard set 
  /// (HEAD, branches and remote-tracking branches) and it does not have a reflog.
  /// <para/>
  /// It will throw if the reference's value at the time of updating does not match the one passed 
  /// through currentTarget (i.e. if the ref has changed since the user read it).
  /// </remarks>
  /// <param name="name">The name of the reference</param>
  /// <param name="target">The target of the reference</param>
  /// <param name="force">Overwrite existing references</param>
  /// <param name="currentTarget">The expected value of the reference at the time of update</param>
  /// <param name="logMessage">The one line long message to be appended to the reflog</param>
  /// <returns>The newly created reference</returns>
  IGitReference CreateMatchingSymbolicReference(
    string name, string target, bool force, string? currentTarget, string? logMessage);

  /// <summary>
  /// Lookup a reference by DWIMing its short name
  /// </summary>
  /// <remarks>
  /// Apply the git precedence rules to the given shorthand to determine which reference the user is referring to.
  /// </remarks>
  /// <param name="shorthand">the short name for the reference</param>
  /// <returns>the reference</returns>
  IGitReference LookupReferenceDwim(string shorthand);

  /// <summary>
  /// Lookup a reference by name in a repository.
  /// </summary>
  /// <remarks>The name will be checked for validity.</remarks>
  /// <param name="name">the long name for the reference (e.g. HEAD, refs/heads/master, refs/tags/v0.1.0, ...)</param>
  /// <returns>the looked-up reference</returns>
  IGitReference LookupReference(string name);

  /// <summary>
  /// Lookup a reference by name and resolve immediately to OID.
  /// </summary>
  /// <remarks>
  /// This function provides a quick way to resolve a reference name straight through to the object 
  /// id that it refers to.
  /// <para/>
  /// The name will be checked for validity.
  /// </remarks>
  /// <param name="name">
  /// The long name for the reference (e.g. HEAD, refs/heads/master, refs/tags/v0.1.0, ...)
  /// </param>
  /// <returns>The oid</returns>
  GitOid ReferenceNameToOid(string name);

  /// <summary>
  /// Ensure there is a reflog for a particular reference.
  /// </summary>
  /// <remarks>
  /// Make sure that successive updates to the reference will append to its log.
  /// </remarks>
  /// <param name="refName">the reference's name</param>
  void EnsureReferenceHasLog(string refName);

  /// <summary>
  /// Check if a reflog exists for the specified reference.
  /// </summary>
  /// <param name="refName">the reference's name</param>
  /// <returns>false when no reflog can be found, true when it exists</returns>
  bool ReferenceHasLog(string refName);

  /// <summary>
  /// Perform a callback on each reference in the repository.
  /// </summary>
  /// <param name="callback">Function which will be called for every listed ref</param>
  void ForEachReference(Func<IGitReference, GitOperationContinuation> callback);

  /// <summary>
  /// Create an iterable for the repo's references
  /// </summary>
  /// <remarks>
  /// Note that the references will be owned by the repository and should be disposed when no longer needed.
  /// </remarks>
  /// <returns>The IEnumerable to iterate over references</returns>
  IEnumerable<IGitReference> EnumerateReferences();

  /// <summary>
  /// Create an iterable for the repo's references that match the specified glob
  /// </summary>
  /// <remarks>
  /// Note that the references will be owned by the repository and should be disposed when no longer needed.
  /// </remarks>
  /// <param name="glob">the glob to match against the reference names</param>
  /// <returns>The IEnumerable to iterate over references</returns>
  IEnumerable<IGitReference> EnumerateReferences(string glob);

  /// <summary>
  /// Create an iterable for the repo's reference names
  /// </summary>
  /// <remarks>
  /// This function is provided for convenience in case only the names are interesting as it 
  /// avoids the allocation of the <see cref="IGitReference"/>
  /// </remarks>
  /// <returns>The IEnumerable to iterate over reference names</returns>
  IEnumerable<string> EnumerateReferenceNames();

  /// <summary>
  /// Create an iterable for the repo's reference names that match the specified glob
  /// </summary>
  /// <remarks>
  /// This function is provided for convenience in case only the names are interesting as it 
  /// avoids the allocation of the <see cref="IGitReference"/>
  /// </remarks>
  /// <param name="glob">the glob to match against the reference names</param>
  /// <returns>The IEnumerable to iterate over reference names</returns>
  IEnumerable<string> EnumerateReferenceNames(string glob);

  /// <summary>
  /// Fill a list with all the references that can be found in a repository.
  /// </summary>
  /// <returns>the reference names</returns>
  IReadOnlyCollection<string> ReferenceList();

  /// <summary>
  /// Perform a callback on each reference in the repository whose name matches the given pattern.
  /// </summary>
  /// <remarks>
  /// his function acts like <see cref="ForEachReference(Func{IGitReference, GitOperationContinuation})"/> 
  /// with an additional pattern match being applied to the reference name before issuing the callback 
  /// function. See that function for more information.
  /// <para/>
  /// The pattern is matched using fnmatch or "glob" style where a '*' matches any sequence of letters, 
  /// a '?' matches any letter, and square brackets can be used to define character ranges(such as "[0-9]" 
  /// for digits).
  /// </remarks>
  /// <param name="glob">Pattern to match (fnmatch-style) against reference name.</param>
  /// <param name="callback">Function which will be called for every listed ref</param>
  void ForEachReferenceName(string glob, Func<string, GitOperationContinuation> callback);

  /// <summary>
  /// Perform a callback on the fully-qualified name of each reference.
  /// </summary>
  /// <param name="callback">Function which will be called for every listed ref name</param>
  void ForEachReferenceName(Func<string, GitOperationContinuation> callback);

  /// <summary>
  /// Delete an existing reference by name
  /// </summary>
  /// <remarks>
  /// This method removes the named reference from the repository without looking at its old value.
  /// </remarks>
  /// <param name="name">The reference to remove</param>
  void RemoveReference(string name);

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
  /// Updates files in the index and working tree to match the content of the tree pointed at by the treeish
  /// </summary>
  /// <param name="treeish"></param>
  /// <param name="options"></param>
  void CheckoutTree(IGitObject? treeish, CheckoutOptions? options = null);

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
  /// Merges the given commit(s) into HEAD, writing the results into the working directory. 
  /// Any changes are staged for commit and any conflicts are written to the index. Callers 
  /// should inspect the repository's index after this completes, resolve any conflicts and 
  /// prepare a commit.
  /// </summary>
  /// <remarks>
  /// For compatibility with git, the repository is put into a merging state. Once the commit 
  /// is done (or if the user wishes to abort), you should clear this state by calling 
  /// <see cref="CleanupState()"/>.
  /// </remarks>
  /// <param name="theirHeads">the heads to merge into</param>
  /// <param name="mergeOptions">merge options</param>
  /// <param name="checkoutOptions">checkout options</param>
  void Merge(IEnumerable<IGitAnnotatedCommit> theirHeads, 
    MergeOptions? mergeOptions = null, CheckoutOptions? checkoutOptions = null);

  /// <summary>
  /// Merge two commits, producing a <see cref="IGitIndex"/> that reflects the result of the merge. 
  /// The index may be written as-is to the working directory or checked out. If the index is to be 
  /// converted to a tree, the caller should resolve any conflicts that arose as part of the merge.
  /// </summary>
  /// <param name="ourCommit">the commit that reflects the destination tree</param>
  /// <param name="theirCommit">the commit to merge in to `our_commit`</param>
  /// <param name="mergeOptions">the merge tree options (or null for defaults)</param>
  /// <returns>the index result</returns>
  IGitIndex MergeCommits(IGitCommit ourCommit, IGitCommit theirCommit, MergeOptions? mergeOptions = null);

  /// <summary>
  /// Merge two trees, producing a <see cref="IGitIndex"/> that reflects the result of the merge. 
  /// The index may be written as-is to the working directory or checked out. If the index is to 
  /// be converted to a tree, the caller should resolve any conflicts that arose as part of the merge.
  /// </summary>
  /// <param name="ancestorTree">the common ancestor between the trees (or null if none)</param>
  /// <param name="ourTree">the tree that reflects the destination tree</param>
  /// <param name="theirTree">the tree to merge in to `our_tree`</param>
  /// <param name="mergeOptions">the merge tree options (or null for defaults)</param>
  /// <returns>the index result</returns>
  IGitIndex MergeTrees(
    IGitTree? ancestorTree, IGitTree ourTree, IGitTree theirTree, MergeOptions? mergeOptions = null);

  /// <summary>
  /// Merge two files as they exist in the index, using the given common ancestor as the baseline, 
  /// producing a <see cref="GitMergeFileResult"/> that reflects the merge result.
  /// </summary>
  /// <param name="ancestor">The index entry for the ancestor file (stage level 1)</param>
  /// <param name="ours">The index entry for our file (stage level 2)</param>
  /// <param name="theirs">The index entry for their file (stage level 3)</param>
  /// <param name="options">The merge file options or null</param>
  /// <returns>The merge file results</returns>
  GitMergeFileResult MergeFilesFromIndex(
       GitIndexEntry ancestor, GitIndexEntry ours, GitIndexEntry theirs, GitMergeFileOptions? options = null);

  /// <summary>
  /// Analyzes the given branch(es) and determines the opportunities for merging them into the 
  /// HEAD of the repository.
  /// </summary>
  /// <param name="theirHeads">the heads to merge into</param>
  /// <returns>analysis enumeration and one of the <see cref="GitMergePreference"/> flag</returns>
  (GitMergeAnalysisResult analysis, GitMergePreference preference) MergeAnalysis(
    IEnumerable<IGitAnnotatedCommit> theirHeads);

  /// <summary>
  /// Analyzes the given branch(es) and determines the opportunities for merging them into a reference.
  /// </summary>
  /// <param name="ourRef">the reference to perform the analysis from</param>
  /// <param name="theirHeads">the heads to merge into</param>
  /// <returns>analysis enumeration and one of the <see cref="GitMergePreference"/> flag</return
  (GitMergeAnalysisResult analysis, GitMergePreference preference) MergeAnalysisForRef(
    IGitReference ourRef,
    IEnumerable<IGitAnnotatedCommit> theirHeads);

  /// <summary>
  /// Find a merge base between two commits
  /// </summary>
  /// <param name="one">one of the commits</param>
  /// <param name="two">the other commit</param>
  /// <returns>the OID of a merge base between 'one' and 'two'</returns>
  GitOid GetMergeBase(GitOid one, GitOid two);

  /// <summary>
  /// Find a merge base given a list of commits
  /// </summary>
  /// <param name="commits">oids of the commits</param>
  /// <returns>the OID of a merge base considering all the commits</returns>
  GitOid GetMergeBase(IEnumerable<GitOid> commits);

  /// <summary>
  /// Find a merge base in preparation for an octopus merge
  /// </summary>
  /// <param name="commits">oids of the commits</param>
  /// <returns>the OID of a merge base considering all the commits</returns>
  GitOid GetMergeBaseOctopus(IEnumerable<GitOid> commits);

  /// <summary>
  /// Find merge bases between two commits
  /// </summary>
  /// <param name="one">one of the commits</param>
  /// <param name="two">the other commit</param>
  /// <returns>the resulting ids</returns>
  IReadOnlyList<GitOid> GetMergeBases(GitOid one, GitOid two);

  /// <summary>
  /// Find merge bases given a list of commits
  /// </summary>
  /// <param name="commits">oids of the commits</param>
  /// <returns>the resulting ids</returns>
  IReadOnlyList<GitOid> GetMergeBases(IEnumerable<GitOid> commits);

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
  IGitCommit LookupCommitPrefix(byte[] shortId, UInt16 shortIdLength);

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
  /// Create a diff between the repository index and the workdir directory.
  /// </summary>
  /// <remarks>
  /// This matches the `git diff` command.  See the note below on
  /// <see cref="DiffTreeToWorkdir(IGitTree?, GitDiffOptions?)"/> for a discussion 
  /// of the difference between `git diff` and `git diff HEAD` and how to emulate 
  /// a `git diff<treeish>`
  /// using libgit2.
  /// <para/>
  /// The index will be used for the "old_file" side of the delta, and the
  /// working directory will be used for the "new_file" side of the delta.
  /// <para/>
  /// If you pass null for the index, then the existing index of the `repo`
  /// will be used.In this case, the index will be refreshed from disk
  /// (if it has changed) before the diff is generated.
  /// </remarks>
  /// <param name="index"> The index to diff from; repo index used if null.</param>
  /// <param name="options">Options structure with options to influence diff or 
  /// null for defaults.</param>
  /// <returns>The diff</returns>
  IGitDiff DiffIndexToWorkdir(IGitIndex? index, GitDiffOptions? options = null);

  /// <summary>
  /// Create a diff with the difference between two index objects.
  /// </summary>
  /// <remarks>
  /// The first index will be used for the "old_file" side of the delta and the second 
  /// index will be used for the "new_file" side of the delta.
  /// </remarks>
  /// <param name="oldIndex">A <see cref="IGitIndex"/> object to diff from.</param>
  /// <param name="newIndex">A <see cref="IGitIndex"/> object to diff to.</param>
  /// <param name="options">Structure with options to influence diff or null for defaults.</param>
  /// <returns>The diff</returns>
  IGitDiff DiffIndexToIndex(IGitIndex? oldIndex, IGitIndex? newIndex, GitDiffOptions? options = null);

  /// <summary>
  /// Create a diff with the difference between two tree objects.
  /// </summary>
  /// <remarks>
  /// This is equivalent to git diff <old-tree> <new-tree>
  /// <para/>
  /// The first tree will be used for the "old_file" side of the 
  /// delta and the second tree will be used for the "new_file" 
  /// side of the delta.You can pass null to indicate an empty tree, 
  /// although it is an error to pass null for both the oldTree and newTree.
  /// </remarks>
  /// <param name="oldTree">
  /// A <see cref="IGitTree"/> object to diff from, or null for empty tree.
  /// </param>
  /// <param name="newTree">
  /// A <see cref="IGitTree"/> object to diff to, or null for empty tree.
  /// </param>
  /// <param name="options">Structure with options to influence diff or null for defaults.</param>
  /// <returns>The diff</returns>
  IGitDiff DiffTreeToTree(IGitTree? oldTree, IGitTree? newTree, GitDiffOptions? options = null);

  /// <summary>
  /// Create a diff between a tree and the working directory using index data to account for 
  /// staged deletes, tracked files, etc.
  /// </summary>
  /// <remarks>
  /// This emulates git diff <tree> by diffing the tree to the index and the index to the 
  /// working directory and blending the results into a single diff that includes staged deleted,
  /// etc.
  /// </remarks>
  /// <param name="oldTree">
  /// A <see cref="IGitTree"/> object to diff from, or null for empty tree.
  /// </param>
  /// <param name="options">Structure with options to influence diff or null for defaults.</param>
  /// <returns>The diff</returns>
  IGitDiff DiffTreeToWorkdirWithIndex(IGitTree? oldTree, GitDiffOptions? options = null);

  /// <summary>
  /// Apply a <see cref="IGitDiff"/> to the given repository, making changes directly 
  /// in the working directory, the index, or both.
  /// </summary>
  /// <param name="diff">the diff to apply</param>
  /// <param name="location">the location to apply (workdir, index or both)</param>
  /// <param name="options">the options for the apply (or null for defaults)</param>
  void ApplyDiff(IGitDiff diff, GitApplyLocation location, GitApplyOptions? options = null);

  /// <summary>
  /// Apply a <see cref="IGitDiff"/> to a <see cref="IGitTree"/>, and return the resulting 
  /// image as an index.
  /// </summary>
  /// <param name="preimage">the tree to apply the diff to</param>
  /// <param name="diff">the diff to apply</param>
  /// <param name="options">the options for the apply (or null for defaults)</param>
  /// <returns>the postimage of the application</returns>
  IGitIndex ApplyDiffToTree(IGitTree preimage, IGitDiff diff, GitApplyOptions? options = null);

  /// <summary>
  /// Lookup a blob object from a repository.
  /// </summary>
  /// <param name="oid">identity of the blob to locate.</param>
  /// <returns>the looked up blob</returns>
  IGitBlob LookupBlob(GitOid oid);

  /// <summary>
  /// Lookup a blob object from a repository, given a prefix of its identifier (short id).
  /// </summary>
  /// <param name="shortId">identity of the blob to locate.</param>
  /// <param name="shortIdLength">Length in hex chars (4 bytes)</param>
  /// <returns>the looked up blob</returns>
  IGitBlob LookupBlobByPrefix(byte[] shortId, UInt16 shortIdLength);

  /// <summary>
  /// Lookup a blob object from a repository, given a prefix of its identifier (short id).
  /// </summary>
  /// <param name="shortSha">identity of the blob to locate.</param>
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

  /// <summary>
  /// Count the number of unique commits between two commit objects
  /// </summary>
  /// <remarks>
  /// There is no need for branches containing the commits to have any upstream relationship, 
  /// but it helps to think of one as a branch and the other as its upstream, the ahead and 
  /// behind values will be what git would report for the branches.
  /// </remarks>
  /// <param name="local">the commit for local</param>
  /// <param name="upstream">the commit for upstream</param>
  /// <returns>The number of commits that the local commit is ahead and behind the upstream</returns>
  AheadBehind GraphAheadBehind(GitOid local, GitOid upstream);

  /// <summary>
  /// Determine if a commit is the descendant of another commit.
  /// </summary>
  /// <remarks>
  /// Note that a commit is not considered a descendant of itself, in contrast 
  /// to git merge-base --is-ancestor.
  /// </remarks>
  /// <param name="commit">a previously loaded commit</param>
  /// <param name="ancestor">a potential ancestor commit</param>
  /// <returns>true if the given commit is a descendant of the potential ancestor, 
  /// false if not.</returns>
  bool GraphDescendantOf(GitOid commit, GitOid ancestor);

  /// <summary>
  /// Determine if a commit is reachable from any of a list of commits by following parent edges.
  /// </summary>
  /// <param name="commit">a previously loaded commit</param>
  /// <param name="descendants">oids of the descendant commits</param>
  /// <returns>
  /// true if the given commit is an ancestor of any of the given potential descendants, false if not
  /// </returns>
  bool GraphIsReachableFromAny(GitOid commit, IEnumerable<GitOid> descendants);

  /// <summary>
  /// Add ignore rules for a repository.
  /// </summary>
  /// <remarks>
  /// Excludesfile rules (i.e. .gitignore rules) are generally read from .gitignore 
  /// files in the repository tree or from a shared system file only if a "core.excludesfile" 
  /// config value is set. The library also keeps a set of per-repository internal ignores 
  /// that can be configured in-memory and will not persist. This function allows you to add 
  /// to that internal rules list.
  /// </remarks>
  /// <param name="rules">
  /// Text of rules, the contents to add on a .gitignore file. It is okay to have multiple 
  /// rules in the text; if so, each rule should be terminated with a newline.
  /// </param>
  void AddIgnoreRule(string rules);

  /// <summary>
  /// Clear ignore rules that were explicitly added.
  /// </summary>
  /// <remarks>
  /// Resets to the default internal ignore rules. This will not turn off rules in .gitignore 
  /// files that actually exist in the filesystem.
  /// <para/>
  /// The default internal ignores ignore ".", ".." and ".git" entries.
  /// </remarks>
  void ClearInternalIgnoreRules();

  /// <summary>
  /// Test if the ignore rules apply to a given path.
  /// </summary>
  /// <remarks>
  /// This function checks the ignore rules to see if they would apply to the given file. 
  /// This indicates if the file would be ignored regardless of whether the file is already 
  /// in the index or committed to the repository.
  /// <para/>
  /// One way to think of this is if you were to do "git check-ignore --no-index" on the 
  /// given file, would it be shown or not?
  /// </remarks>
  /// <param name="path">
  /// the file to check ignores for, relative to the repo's workdir.
  /// </param>
  /// <returns>
  /// boolean returning false if the file is not ignored, true if it is
  /// </returns>
  bool IgnorePathIsIgnored(string path);

  /// <summary>
  /// Get the Object Database for this repository.
  /// </summary>
  /// <remarks>
  /// If a custom ODB has not been set, the default database for the repository will be returned 
  /// (the one located in .git/objects).
  /// </remarks>
  /// <returns></returns>
  IGitOdb GetOdb();

  /// <summary>
  /// Remove all the metadata associated with an ongoing command like merge, revert, cherry-pick, etc. 
  /// For example: MERGE_HEAD, MERGE_MSG, etc.
  /// </summary>
  void CleanupState();

  /// <summary>
  /// Add a note for an object
  /// </summary>
  /// <param name="noteRef">
  /// canonical name of the reference to use (optional); defaults to "refs/notes/commits"
  /// </param>
  /// <param name="author">signature of the notes commit author</param>
  /// <param name="committer">signature of the notes commit committer</param>
  /// <param name="oid">OID of the git object to decorate</param>
  /// <param name="note">Content of the note to add for object oid</param>
  /// <param name="force">Overwrite existing note</param>
  /// <returns>the OID of the note</returns>
  GitOid CreateNote(string? noteRef, IGitSignature author, IGitSignature committer, GitOid oid, 
    string note, bool force = false);

  /// <summary>
  /// Add a note for an object from a commit
  /// </summary>
  /// <remarks>
  /// This function will create a notes commit for a given object,
  /// the commit is a dangling commit, no reference is created.
  /// </remarks>
  /// <param name="parent">
  /// parent note or null if this shall start a new notes tree
  /// </param>
  /// <param name="author">signature of the notes commit author</param>
  /// <param name="committer">signature of the notes commit committer</param>
  /// <param name="oid">OID of the git object to decorate</param>
  /// <param name="note">Content of the note to add for object oid</param>
  /// <param name="force">Overwrite existing note</param>
  /// <returns>The commit oid and the note oid</returns>
  (GitOid CommitOid, GitOid BlobOid) CreateNoteCommit(IGitCommit? parent, IGitSignature author, 
     IGitSignature committer, GitOid oid, string note, bool force = false);

  /// <summary>
  /// Read the note for an object
  /// </summary>
  /// <param name="noteRef">
  /// canonical name of the reference to use (optional); defaults to "refs/notes/commits"
  /// </param>
  /// <param name="oid">OID of the git object to read the note from</param>
  /// <returns>the read note</returns>
  IGitNote ReadNote(string? noteRef, GitOid oid);

  /// <summary>
  /// Read the note for an object from a note commit
  /// </summary>
  /// <param name="commit">the notes commit object</param>
  /// <param name="oid">OID of the git object to read the note from</param>
  /// <returns>the read note</returns>
  IGitNote ReadNoteCommit(IGitCommit commit, GitOid oid);

  /// <summary>
  /// Remove the note for an object
  /// </summary>
  /// <param name="noteRef">
  /// canonical name of the reference to use (optional); defaults to "refs/notes/commits"
  /// </param>
  /// <param name="author">signature of the notes commit author</param>
  /// <param name="committer">signature of the notes commit committer</param>
  /// <param name="oid">OID of the git object to remove the note from</param>
  void RemoveNote(string? noteRef, IGitSignature author, IGitSignature committer, GitOid oid);

  /// <summary>
  /// Remove the note for an object
  /// </summary>
  /// <param name="notesCommit">the notes commit object</param>
  /// <param name="author">signature of the notes commit author</param>
  /// <param name="committer">signature of the notes commit committer</param>
  /// <param name="oid">OID of the git object to remove the note from</param>
  /// <returns>
  /// the new notes commit
  /// <para/>
  /// When removing a note a new tree containing all notes sans the note to be removed is created 
  /// and a new commit pointing to that tree is also created. In the case where the resulting tree 
  /// is an empty tree a new commit pointing to this empty tree will be returned.
  /// </returns>
  GitOid RemoveNoteCommit(IGitCommit notesCommit, IGitSignature author, IGitSignature committer, GitOid oid);

  /// <summary>
  /// Loop over all the notes within a specified namespace and issue a callback for each one.
  /// </summary>
  /// <param name="noteRef">Reference to read from (optional); defaults to "refs/notes/commits".</param>
  /// <param name="callback">Callback to invoke per found annotation.</param>
  void ForeachNote(string? noteRef, GitNoteForeachCallback callback);

  /// <summary>
  /// Get the default notes reference for a repository
  /// </summary>
  string DefaultNoteRef { get; }

  /// <summary>
  /// Initializes a rebase operation to rebase the changes in branch relative to upstream onto another branch. 
  /// To begin the rebase process, call <see cref="IGitRebase.Next"/>.
  /// </summary>
  /// <param name="branch">The terminal commit to rebase, or null to rebase the current branch</param>
  /// <param name="upstream">The commit to begin rebasing from, or null to rebase all reachable commits</param>
  /// <param name="onto">The branch to rebase onto, or null to rebase onto the given upstream</param>
  /// <param name="options">Options to specify how rebase is performed, or null</param>
  /// <returns>the rebase object</returns>
  IGitRebase StartRebase(
    IGitAnnotatedCommit? branch, IGitAnnotatedCommit? upstream, IGitAnnotatedCommit? onto, 
    GitRebaseOptions? options);

  /// <summary>
  /// Opens an existing rebase that was previously started by either an invocation of <see cref="StartRebase"/> 
  /// or by another client.
  /// </summary>
  /// <param name="options">Options to specify how rebase is performed</param>
  /// <returns>the rebase object</returns>
  IGitRebase OpenRebase(GitRebaseOptions? options);
}
