namespace Libgit2Bindings;

/// <summary>
/// Container for <see cref="IGitReference"/> that allows controlling whether the reference
/// is automatically disposed after the iteration.
/// </summary>
public class GitReferenceBox
{
  internal GitReferenceBox() { }

  internal bool AutoDispose { get; private set; } = true;

  /// <summary>
  /// The actual reference.
  /// </summary>
  public required IGitReference Reference { get; init; }

  /// <summary>
  /// Prevent the reference from being automatically disposed after the iteration.
  /// <para/>
  /// If this is called, the caller is responsible for disposing the reference.
  /// </summary>
  public void DoNotAutoDisposeAfterIteration()
  {
    AutoDispose = false;
  }
}

/// <summary>
/// Extension methods for <see cref="GitReferenceBox"/>.
/// </summary>
public static class GitReferenceBoxExtensions
{
  /// <summary>
  /// Prevent the references from being automatically disposed after the iteration.
  /// </summary>
  /// <remarks>
  /// This will return <see cref="IEnumerable{T}"/> where T is <see cref="IDisposable"/>.
  /// The caller is responsible for disposing all produced references and needs to be aware
  /// of the fact that downstream linq operations could potentially filter out references.
  /// </remarks>
  /// <param name="referenceBoxes">Input enumerable of the reference container</param>
  /// <returns>An iterable of <see cref="IGitReference"/>s that must be disposed by the caller</returns>
  public static IEnumerable<IGitReference> DoNotAutoDisposeAfterIteration(
    this IEnumerable<GitReferenceBox> referenceBoxes)
  {
    return referenceBoxes.Select(referenceBox =>
    {
      referenceBox.DoNotAutoDisposeAfterIteration();
      return referenceBox.Reference;
    });
  }
}
