using System.IO;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;

namespace Libgit2Bindings;

[Flags]
public enum BranchType
{
  LocalBranch = 1 << 0,
  RemoteBranch = 1 << 1,
  All = 3,
}

/// <summary>
/// Basic type of any Git reference.
/// </summary>
public enum GitReferenceType
{
  /// <summary>
  /// Invalid reference
  /// </summary>
  Invalid = 0,
  /// <summary>
  /// A reference that points at an object id
  /// </summary>
  Direct = 1,
  /// <summary>
  /// A reference that points at another reference
  /// </summary>
  Symbolic = 2,
  All = Direct | Symbolic,
}

public interface IGitReference : IDisposable
{
  public static string LocalBranchPrefix => "refs/heads/";

  /// <summary>
  /// Check if a reference is a local branch.
  /// </summary>
  /// <remarks>
  /// true when the reference lives in the refs/heads namespace; false otherwise.
  /// </remarks>
  bool IsBranch { get; }

  /// <summary>
  /// Check if a reference is a note
  /// </summary>
  /// <remarks>
  /// true when the reference lives in the refs/notes namespace; false otherwise.
  /// </remarks>
  bool IsNote { get; }

  /// <summary>
  /// Check if a reference is a remote tracking branch
  /// </summary>
  /// <remarks>
  /// true when the reference lives in the refs/remotes namespace; false otherwise.
  /// </remarks>
  bool IsRemote { get; }

  /// <summary>
  /// Check if a reference is a tag
  /// </summary>
  /// <remarks>
  /// true when the reference lives in the refs/tags namespace; false otherwise.
  /// </remarks>
  bool IsTag { get; }

  /// <summary>
  /// Get the full name of a reference.
  /// </summary>
  string Name { get; }

  /// <summary>
  /// Get the type of a reference.
  /// </summary>
  GitReferenceType Type { get; }

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

  /// <summary>
  /// Get full name to the reference pointed to by a symbolic reference.
  /// </summary>
  /// <remarks>
  /// Only available if the reference is symbolic.
  /// </remarks>
  /// <returns>The name if available, null otherwise</returns>
  string? GetSymbolicTarget();

  /// <summary>
  /// Get the target OID of a direct reference
  /// </summary>
  /// <remarks>
  /// Only available if the reference is direct (i.e. an object id reference, not a symbolic one).
  /// <para/>
  /// To find the OID of a symbolic ref, call <see cref="Resolve"/> and then this function
  /// (or maybe use <see cref="IGitRepository.ReferenceNameToId"/> to directly resolve a reference 
  /// name all the way through to an OID).
  /// </remarks>
  /// <returns>the oid if available, null otherwise</returns>
  GitOid? GetTarget();

  /// <summary>
  /// Conditionally create a new reference with the same name as the given reference but a 
  /// different OID target. The reference must be a direct reference, otherwise this will fail.
  /// </summary>
  /// <remarks>
  /// The new reference will be written to disk, overwriting the given reference.
  /// </remarks>
  /// <param name="targetId">The new target OID for the reference</param>
  /// <param name="logMessage">The one line long message to be appended to the reflog</param>
  /// <returns>the newly created reference</returns>
  IGitReference SetTarget(GitOid targetId, string logMessage);

  /// <summary>
  /// Create a new reference with the same name as the given reference but a different symbolic target. 
  /// The reference must be a symbolic reference, otherwise this will fail.
  /// </summary>
  /// <remarks>
  /// The new reference will be written to disk, overwriting the given reference.
  /// <para/>
  /// The target name will be checked for validity.
  /// <para/>
  /// The message for the reflog will be ignored if the reference does not belong in the standard set 
  /// (HEAD, branches and remote-tracking branches) and it does not have a reflog.
  /// </remarks>
  /// <param name="target">The new target for the reference</param>
  /// <param name="logMessage">The one line long message to be appended to the reflog</param>
  /// <returns>the newly created reference</returns>
  IGitReference SetSymbolicTarget(string target, string logMessage);

  /// <summary>
  /// Resolve a symbolic reference to a direct reference.
  /// </summary>
  /// <remarks>
  /// This method iteratively peels a symbolic reference until it resolves to a 
  /// direct reference to an OID.
  /// <param/>
  /// If a direct reference is passed as an argument, a copy of that reference is 
  /// returned. This copy must be disposed too
  /// </remarks>
  /// <returns>the peeled reference</returns>
  IGitReference Resolve();

  /// <summary>
  /// Recursively peel reference until object of the specified type is found.
  /// </summary>
  /// <remarks>
  /// If you pass <see cref="GitObjectType.Any"/> as the target type, then the object will be 
  /// peeled until a non-tag object is met.
  /// </remarks>
  /// <param name="type">The type of the requested object</param>
  /// <returns>the peeled <see cref="IGitObject"/></returns>
  IGitObject Peel(GitObjectType type);

  /// <summary>
  /// Return the peeled OID target of this reference.
  /// </summary>
  /// <remarks>
  /// This peeled OID only applies to direct references that point to a hard Tag object: 
  /// it is the result of peeling such Tag.
  /// </remarks>
  /// <returns>the oid if available, null otherwise</returns>
  GitOid? PeelTarget();

  /// <summary>
  /// Compare two references.
  /// </summary>
  /// <param name="reference">The reference to compare to</param>
  /// <returns>true, if the references are the same</returns>
  bool EqualsTo(IGitReference reference);

  /// <summary>
  /// Delete an existing reference.
  /// </summary>
  /// <remarks>
  /// This method works for both direct and symbolic references. The reference will be immediately 
  /// removed on disk but the memory will not be freed. Callers must dispose the object.
  /// <para/>
  /// This function will throw an error if the reference has changed from the time it was looked up.
  /// </remarks>
  void DeleteFromDisk();

  /// <summary>
  /// Create a copy of an existing reference.
  /// </summary>
  /// <returns>the copy</returns>
  IGitReference Duplicate();

  /// <summary>
  /// Get the repository where a reference resides.
  /// </summary>
  /// <returns>the repo</returns>
  IGitRepository GetOwner();

  /// <summary>
  /// Rename an existing reference.
  /// </summary>
  /// <remarks>
  /// This method works for both direct and symbolic references.
  /// <para/>
  /// The new name will be checked for validity.
  /// <para/>
  /// If the force flag is not enabled, and there's already a reference with the given name, 
  /// the renaming will fail.
  /// <para/>
  /// IMPORTANT: The user needs to write a proper reflog entry if the reflog is enabled for 
  /// the repository. We only rename the reflog if it exists.
  /// </remarks>
  /// <param name="newName">The new name for the reference</param>
  /// <param name="force">Overwrite an existing reference</param>
  /// <param name="logMessage">The one line long message to be appended to the reflog</param>
  IGitReference Rename(string newName, bool force, string logMessage);

  /// <summary>
  /// Get the reference's short name
  /// </summary>
  /// <remarks>
  /// This will transform the reference name into a name "human-readable" version. If no 
  /// shortname is appropriate, it will return the full name.
  /// </remarks>
  /// <returns>the human-readable version of the name</returns>
  string GetShorthand();
}
