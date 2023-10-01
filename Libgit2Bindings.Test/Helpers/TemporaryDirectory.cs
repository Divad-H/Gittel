namespace Libgit2Bindings.Test.Helpers;

internal class TemporaryDirectory : IDisposable
{
  public string DirectoryPath { get; }

  public TemporaryDirectory()
  {
    DirectoryPath = GenerateTemporaryPath();
    Directory.CreateDirectory(DirectoryPath);
  }

  private static string GenerateTemporaryPath()
  {
    const string TempPath = "Temp";
    return Path.Combine(TempPath, Path.GetRandomFileName());
  }

  public void Dispose()
  {
    DeleteDirectory(DirectoryPath);
  }

  private static void RemoveReadOnlyFlagsAndDeleteDirectory(string path)
  {
    var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

    foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
    {
      info.Attributes = FileAttributes.Normal;
    }

    directory.Delete(true);
  }

  private static void DeleteDirectory(string path)
  {
    if (!Directory.Exists(path))
    {
      return;
    }

    const int maxAttempts = 5;
    const int initialTimeout = 16;
    for (int attempt = 0; attempt < maxAttempts; ++attempt)
    {
      try
      {
        RemoveReadOnlyFlagsAndDeleteDirectory(path);
        return;
      }
      catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
      {
        if (attempt == maxAttempts - 1)
        {
          System.Diagnostics.Trace.TraceError("Failed to delete directory '{0}': {1}", path, ex);
          return;
        }
        Thread.Sleep(initialTimeout * (int)Math.Pow(2, attempt));
      }
    }
  }
}
