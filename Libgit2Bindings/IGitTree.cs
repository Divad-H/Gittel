namespace Libgit2Bindings;

public interface IGitTree : IDisposable
{
  /// <summary>
  /// Get the id of a tree.
  /// </summary>
  /// <returns>object identity for the tree.</returns>
  public GitOid GetId();
}
