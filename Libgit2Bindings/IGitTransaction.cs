namespace Libgit2Bindings;

public interface IGitTransaction : IDisposable
{
  /// <summary>
  /// Commit the changes from the transaction
  /// </summary>
  /// <remarks>
  /// Perform the changes that have been queued. 
  /// The updates will be made one by one, and the first failure will stop the processing.</remarks>
  void Commit();
}
