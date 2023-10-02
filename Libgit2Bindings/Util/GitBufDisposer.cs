namespace Libgit2Bindings.Util;

internal class GitBufDisposer : IDisposable
{
  private readonly libgit2.GitBuf _buf;
  public GitBufDisposer(libgit2.GitBuf buf)
  {
    _buf = buf;
  }

  public void Dispose()
  {
    libgit2.buffer.GitBufDispose(_buf);
    _buf.Dispose();
  }
}

internal static class GitBufExtensions
{
  public static IDisposable GetDisposer(this libgit2.GitBuf buf)
  {
    return new GitBufDisposer(buf);
  }
}
