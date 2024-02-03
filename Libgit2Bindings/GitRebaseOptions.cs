namespace Libgit2Bindings;

/// <summary>
/// Rebase options
/// <para/>
/// Use to tell the rebase machinery how to operate.
/// </summary>
public sealed record GitRebaseOptions
{
  /// <summary>
  /// Used by <see cref="IGitRepository.StartRebase"/>, this will instruct other clients working on this 
  /// rebase that you want a quiet rebase experience, which they may choose to provide in an application-
  /// specific manner. This has no effect upon libgit2 directly, but is provided for interoperability 
  /// between Git tools.
  /// </summary>
  public bool Quiet { get; init; }

  /// <summary>
  /// Used by <see cref="IGitRepository.StartRebase"/>, this will begin an in-memory rebase, which will 
  /// allow callers to step through the rebase operations and commit the rebased changes, but will not 
  /// rewind HEAD or update the repository to be in a rebasing state. This will not interfere with the 
  /// working directory (if there is one).
  /// </summary>
  public bool InMemory { get; init; }

  /// <summary>
  /// Used by <see cref="IGitRebase.Finish"/>, this is the name of the notes reference used to rewrite 
  /// notes for rebased commits when finishing the rebase; if null, the contents of the configuration 
  /// option `notes.rewriteRef` is examined, unless the configuration option `notes.rewrite.rebase` is 
  /// set to false. If `notes.rewriteRef` is also null, notes will not be rewritten.
  /// </summary>
  public string? RewriteNotesRef { get; init; }

  /// <summary>
  /// Options to control how trees are merged during <see cref="IGitRebase.Next"/>.
  /// </summary>
  public MergeOptions? MergeOptions { get; init; }

  /// <summary>
  /// Options to control how files are written during <see cref="IGitRepository.StartRebase"/>, 
  /// <see cref="IGitRebase.Next"/> and <see cref="IGitRebase.Abort"/>. Note that a minimum strategy of 
  /// <see cref="CheckoutStrategy.Safe"/> is defaulted in <see cref="IGitRepository.StartRebase"/> and 
  /// <see cref="IGitRepository.Next"/>, and a minimum strategy of <see cref="CheckoutStrategy.Force"/> 
  /// is defaulted in <see cref="IGitRebase.Abort"/> to match git semantics.
  /// </summary>
  public CheckoutOptions? CheckoutOptions { get; init; }

  /// <summary>
  /// Optional callback that allows users to override commit
	/// creation in <see cref="IGitRebase.Commit"/>. If specified, users can
  /// create their own commit and provide the commit ID, which
	/// may be useful for signing commits or otherwise customizing
  /// the commit creation.
  /// </summary>
  /// <remarks>
	/// If this callback returns <see cref="GitOperationContinuationWithPassthrough.Passthrough"/>, then
  /// <see cref="IGitRebase.Commit"/> will continue to create the commit.
  /// </remarks>
  public GitRebaseCommitCreateCallback? CommitCreateCallback { get; init; }
}

/// <summary>
/// Commit creation callback: used when a function is going to create
/// commits(for example, in <see cref="IGitRepository.StartRebase"/>) to allow callers to
/// override the commit creation behavior. For example, users may
/// wish to sign commits by providing this information to a buffer, 
/// signing that buffer, then calling <see cref="IGitRepository.CreateCommitWithSignature"/>.  
/// The resultant commit id should be returned.
/// </summary>
/// <param name="createdCommitId">GitOid that this callback will populate with the object
/// id of the commit that is created</param>
/// <param name="author">the author name and time of the commit</param>
/// <param name="committer">the committer name and time of the commit</param>
/// <param name="messageEncoding">the encoding of the given message, or null to assume UTF8</param>
/// <param name="message">the commit message</param>
/// <param name="tree">the tree to be committed</param>
/// <param name="parents">the commit parents</param>
/// <returns>Operation continuation. Return <see cref="GitOperationContinuationWithPassthrough.Continue"/>
/// if a commit has been created. Return <see cref="GitOperationContinuationWithPassthrough.Passthrough"/>
/// if the callback has not created a commit and wants the calling function to create the commit as
/// if no callback had been specified</returns>
public delegate GitOperationContinuationWithPassthrough GitRebaseCommitCreateCallback(
  out GitOid? createdCommitId,
  IGitSignature author, IGitSignature committer, string message,
  IGitTree tree, IReadOnlyCollection<IGitCommit> parents);
