using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class GitMergeFileInputMapper
{
  public static unsafe libgit2.GitMergeFileInput ToNative(this GitMergeFileInput managedFile, 
    DisposableCollection disposables)
  {
    var res = new libgit2.GitMergeFileInput();
    try
    {
      res.Version = (int)libgit2.GitMergeFileInputVersion.GIT_MERGE_FILE_INPUT_VERSION;

      var buffer = new PinnedBuffer(managedFile.FileContent)
        .DisposeWith(disposables);

      res.Size = (UInt64)buffer.Length;
      ((libgit2.GitMergeFileInput.__Internal*)res.__Instance)->ptr = buffer.Pointer;
      res.Path = managedFile.Path;
      res.Mode = managedFile.Mode;

      return res;
    }
    catch(Exception)
    {
      res.Dispose();
      throw;
    }
  }
}
