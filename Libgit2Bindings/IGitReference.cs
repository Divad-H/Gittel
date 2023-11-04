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
}
