using Libgit2Bindings.Util;
using System.Runtime.InteropServices;

namespace Libgit2Bindings.Callbacks;

internal sealed class CloneCallbacksImpl : IDisposable
{
  private readonly GCHandle _gcHandle;

  private readonly RepositoryCreateHandler? _repositoryCreate;
  private readonly RemoteCreateHandler? _remoteCreate;

  public CloneCallbacksImpl(RepositoryCreateHandler? repositoryCreate, RemoteCreateHandler? remoteCreate)
  {
    _repositoryCreate = repositoryCreate;
    _remoteCreate = remoteCreate;

    _gcHandle = GCHandle.Alloc(this);
  }

  public IntPtr Payload => GCHandle.ToIntPtr(_gcHandle);

  public unsafe static int GitRemoteCreateCb(IntPtr @out, IntPtr repo, string name, string url, IntPtr payload)
  {
    Func<int> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (CloneCallbacksImpl)gcHandle.Target!;
      using var managedRepo = new GitRepository(libgit2.GitRepository.__CreateInstance(repo), true);
      if (callbacks._remoteCreate is null)
      {
        return (int)libgit2.GitErrorCode.GIT_EUSER;
      }
      var res = callbacks._remoteCreate(out var remote, managedRepo, name, url);
      if (res == 0)
      {
        using var managedRemote = remote as GitRemote;
        if (managedRemote is null)
          return (int)libgit2.GitErrorCode.GIT_EUSER;
        managedRemote.ReleaseNativeInstance();
        void** ptr = (void**)@out;
        *ptr = (void*)managedRemote.NativeGitRemote.__Instance;
      }
      managedRepo.ReleaseNativeInstance();
      return res;
    };

    return func.ExecuteInTryCatch(nameof(CloneOptions.RemoteCreateCallback));
  }

  public unsafe static int GitRepositoryCreateCb(IntPtr @out, string path, int bare, IntPtr payload)
  {
    Func<int> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (CloneCallbacksImpl)gcHandle.Target!;
      if (callbacks._repositoryCreate is null)
      {
        return (int)libgit2.GitErrorCode.GIT_EUSER;
      }
      var res = callbacks._repositoryCreate(out var repository, path, bare != 0);
      if (res == 0)
      {
        if (repository is not GitRepository managedRepository)
          return (int)libgit2.GitErrorCode.GIT_EUSER;
        void** ptr = (void**)@out;
        *ptr = (void*)managedRepository.NativeGitRepository.__Instance;
      }
      return res;
    };

    return func.ExecuteInTryCatch(nameof(CloneOptions.RepositoryCreateCallback));
  }

  public void Dispose()
  {
    _gcHandle.Free();
  }
}
