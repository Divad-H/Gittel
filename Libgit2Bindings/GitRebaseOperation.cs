namespace Libgit2Bindings
{
  /// <summary>
  /// A rebase operation
  /// <para/>
  /// Describes a single instruction/operation to be performed during the rebase.
  /// </summary>
  public sealed record GitRebaseOperation
  {
    /// <summary>
    /// The type of rebase operation. 
    /// </summary>
    public GitRebaseOperationType Type { get; init; }
    /// <summary>
    /// The commit ID being cherry-picked. This will be populated for all operations except 
    /// those of type <see cref="GitRebaseOperationType.Exec"/>.
    /// </summary>
    public GitOid? Id { get; init; }
    /// <summary>
    /// The executable the user has requested be run. This will only be populated for 
    /// operations of type <see cref="GitRebaseOperationType.Exec"/>.
    /// </summary>
    public string? Exec { get; init; }
  }

  /// <summary>
  /// Type of rebase operation in-progress after calling <see cref="IGitRebase.Next"/>
  /// </summary>
  public enum GitRebaseOperationType
  {
    /// <summary>
    /// The given commit is to be cherry-picked. The client should commit
    /// the changes and continue if there are no conflicts.
    /// </summary>
    Pick = 0,
    /// <summary>
    /// The given commit is to be cherry-picked, but the client should prompt
    /// the user to provide an updated commit message.
    /// </summary>
    Reword = 1,
    /// <summary>
    /// The given commit is to be cherry-picked, but the client should stop
    /// to allow the user to edit the changes before committing them.
    /// </summary>
    Edit = 2,
    /// <summary>
    /// The given commit is to be squashed into the previous commit. The
    /// commit message will be merged with the previous message.
    /// </summary>
    Squash = 3,
    /// <summary>
    /// The given commit is to be squashed into the previous commit. The
    /// commit message from this commit will be discarded.
    /// </summary>
    Fixup = 4,
    /// <summary>
    /// No commit will be cherry-picked. The client should run the given
    /// command and (if successful) continue.
    /// </summary>
    Exec = 5,
  }
}
