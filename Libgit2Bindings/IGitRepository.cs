namespace Libgit2Bindings;

public interface IGitRepository : IDisposable
{
  /// <summary>
  /// Retrieve and resolve the reference pointed at by HEAD.
  /// </summary>
  IGitReference GetHead();

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
}
