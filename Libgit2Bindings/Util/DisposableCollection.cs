namespace Libgit2Bindings.Util;

internal class DisposableCollection : IDisposable
{
  private readonly List<IDisposable> _disposables = new();

  public void Add(IDisposable disposable)
  {
    _disposables.Add(disposable);
  }

  public void Dispose()
  {
    foreach (var disposable in _disposables.Reverse<IDisposable>())
    {
      disposable.Dispose();
    }
  }
}

internal static class DisposableCollectionExtensions
{
  public static T DisposeWith<T>(this T disposable, DisposableCollection disposables) where T : IDisposable
  {
    disposables.Add(disposable);
    return disposable;
  }
}
