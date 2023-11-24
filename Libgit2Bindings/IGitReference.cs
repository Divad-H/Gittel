namespace Libgit2Bindings;

[Flags]
public enum BranchType
{
  LocalBranch = 1 << 0,
  RemoteBranch = 1 << 1,
  All = 3,
}

public interface IGitReference : IDisposable
{
  public static string LocalBranchPrefix => "refs/heads/";

  /// <summary>
  /// Get the branch name
  /// </summary>
  /// <returns>the abbreviated reference name</returns>
  string BranchName();

  /// <summary>
  /// Delete an existing branch reference.
  /// </summary>
  /// <remarks>
  /// Note that if the deletion succeeds, the reference object will not be valid anymore, 
  /// and should be disposed immediately by the user.
  /// <para/>
  /// The reference must be representing a branch
  /// </remarks>
  void DeleteBranch();

  /// <summary>
  /// Move/rename an existing local branch reference.
  /// </summary>
  /// <remarks>
  /// The new branch name will be checked for validity.
  /// </remarks>
  /// <param name="newBranchName">
  /// Target name of the branch once the move is performed; this name is validated for consistency.
  /// </param>
  /// <param name="force">Overwrite existing branch.</param>
  void MoveBranch(string newBranchName, bool force);

  /// <summary>
  /// Determine if any HEAD points to the current branch
  /// </summary>
  /// <remarks>
  /// This will iterate over all known linked repositories 
  /// (usually in the form of worktrees) 
  /// and report whether any HEAD is pointing at the current branch.
  /// </remarks>
  /// <returns>true, if the branch is checked out</returns>
  bool IsBranchCheckedOut();

  /// <summary>
  /// Determine if HEAD points to this branch
  /// </summary>
  /// <returns>true, if the branch is checked out</returns>
  bool IsBranchHead();

  /// <summary>
  /// Get the upstream of a branch
  /// </summary>
  /// <remarks>
  /// this will return a new reference object corresponding to its remote tracking branch.
  /// The reference must be a local branch.
  /// </remarks>
  /// <returns>the retrieved reference.</returns>
  IGitReference GetUpstream();

  /// <summary>
  /// Set a branch's upstream branch
  /// </summary>
  /// <remarks>
  /// This will update the configuration to set the branch named branchName as the upstream of branch. 
  /// Pass a null name to unset the upstream information.
  /// </remarks>
  /// <param name="branchName">remote-tracking or local branch to set as upstream.</param>
  void SetUpstream(string? branchName);
}
