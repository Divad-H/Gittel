using System.Reactive.Disposables;

namespace Gittel.Util;

public static class ObservableExtensions
{
  /// <summary>
  /// Disposes the disposable when the compositeDisposable is disposed.
  /// </summary>
  /// <typeparam name="T">
  /// The concrete type of the disposable
  /// </typeparam>
  /// <param name="disposable">
  /// The disposable to be disposed
  /// </param>
  /// <param name="compositeDisposable">
  /// The compositeDisposable that will dispose the disposable
  /// </param>
  /// <returns>
  /// The concretely typed disposable
  /// </returns>
  public static T DisposeWith<T>(this T disposable, CompositeDisposable compositeDisposable) where T : IDisposable
  {
    compositeDisposable.Add(disposable);
    return disposable;
  }
}
