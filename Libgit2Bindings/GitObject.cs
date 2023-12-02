using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal class GitObject(libgit2.GitObject nativeGitObject) : IGitObject
{
  public libgit2.GitObject NativeGitObject { get; } = nativeGitObject;

  public GitOid Id
  {
    get
    {
      using var id = libgit2.@object.GitObjectId(NativeGitObject);
      return GitOidMapper.FromNative(id);
    }
  }

  public GitObjectType Type => libgit2.@object.GitObjectType(NativeGitObject).ToManaged();

  public IGitRepository Owner
  {
    get
    {
      var res = libgit2.@object.GitObjectOwner(NativeGitObject);
      return new GitRepository(res, true);
    }
  }

  public string ShortId
  {
    get
    {
      var res = libgit2.@object.GitObjectShortId(out var buf, NativeGitObject);
      using (buf)
      {
        CheckLibgit2.Check(res, "Unable to get short id");
        return StringUtil.ToString(buf);
      }
    }
  }

  public IGitObject Duplicate()
  {
    var res = libgit2.@object.GitObjectDup(out var nativeGitObject, NativeGitObject);
    CheckLibgit2.Check(res, "Unable to duplicate object");
    return new GitObject(nativeGitObject);
  }

  public IGitObject LookupByPath(string path, GitObjectType type)
  {
    var res = libgit2.@object.GitObjectLookupBypath(
      out var nativeGitObject, NativeGitObject, path, type.ToNative());
    CheckLibgit2.Check(res, "Unable to lookup object by path '{0}'", path);
    return new GitObject(nativeGitObject);
  }

  public IGitObject Peel(GitObjectType type)
  {
    var res = libgit2.@object.GitObjectPeel(out var nativeGitObject, NativeGitObject, type.ToNative());
    CheckLibgit2.Check(res, "Unable to peel object");
    return new GitObject(nativeGitObject);
  }

  public IGitDescribeResult DescribeCommit(GitDescribeOptions? options)
  {
    var res = libgit2.describe.GitDescribeCommit(
      out var nativeDescribeResult, NativeGitObject, options?.ToNative());
    CheckLibgit2.Check(res, "Unable to describe commit");
    return new GitDescribeResult(nativeDescribeResult);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.@object.GitObjectFree(NativeGitObject);
      _disposedValue = true;
    }
  }

  ~GitObject()
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
