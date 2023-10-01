namespace Libgit2Bindings.Util;

internal static class GittelObjects
{
  public static T? Downcast<T>(object? instance) where T : class
  {
    return instance is null ? null : instance as T 
      ?? throw new ArgumentException($"{typeof(T).Name} must be created by Gittel");
  }

  public static T DowncastNonNull<T>(object instance) where T : class
  {
    return (instance ?? throw new ArgumentNullException(typeof(T).Name)) as T 
      ?? throw new ArgumentException($"{typeof(T).Name} must be created by Gittel");
  }
}
