namespace Libgit2Bindings;
/// <summary>
/// Annotated commits, the input to merge and rebase. 
/// </summary>
public interface IGitAnnotatedCommit : IDisposable
{
  /// <summary>
  /// Gets the commit ID that this annotated commit refers to.
  /// </summary>
  /// <returns>commit id</returns>
  public GitOid GetId();

  /// <summary>
  /// Get the refname
  /// </summary>
  /// <returns>the ref name</returns>
  public string GetRefName();
}
