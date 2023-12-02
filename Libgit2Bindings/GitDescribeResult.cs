using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal class GitDescribeResult(
  libgit2.GitDescribeResult nativeGitDescribeResult, bool ownsNativeInstance = true) : IGitDescribeResult
{
  public libgit2.GitDescribeResult NativeGitDescribeResult { get; } = nativeGitDescribeResult;
  private bool _ownsNativeInstance = ownsNativeInstance;

  public string Format(GitDescribeFormatOptions? options)
  {
    var res = libgit2.describe.GitDescribeFormat(
      out var buf, NativeGitDescribeResult, options?.ToNative());
    using (buf)
    {
      CheckLibgit2.Check(res, "Unable to format describe result");
      return StringUtil.ToString(buf);
    }
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (_ownsNativeInstance)
      {
        libgit2.describe.GitDescribeResultFree(NativeGitDescribeResult);
      }
      _disposedValue = true;
    }
  }

  ~GitDescribeResult()
  {
    Dispose(disposing: false);
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
  #endregion
}
