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
  /// <returns>the abbreviated reference nam</returns>
  string BranchName();
}
