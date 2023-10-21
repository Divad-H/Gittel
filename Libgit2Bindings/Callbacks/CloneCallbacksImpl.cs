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

  public unsafe static int GitRemoteCreateCb(IntPtr @out, IntPtr repo, string name, string url, IntPtr payload)
  {
    try
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (CloneCallbacksImpl)gcHandle.Target!;
      using var managedRepo = new GitRepository(libgit2.GitRepository.__CreateInstance(repo));
      if (callbacks._remoteCreate is null)
      {
        return -1;
      }
      var res = callbacks._remoteCreate(out var remote, managedRepo, name, url);
      if (res == 0)
      {
        var managedRemote = remote as GitRemote;
        if (managedRemote is null)
          return -1;
        void** ptr = (void**)@out;
        *ptr = (void*)managedRemote.NativeGitRemote.__Instance;
      }
      return res;
    }
    catch (Exception)
    {
      return -1;
    }
  }

  public unsafe static int GitRepositoryCreateCb(IntPtr @out, string path, int bare, IntPtr payload)
  {
    try
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (CloneCallbacksImpl)gcHandle.Target!;
      if (callbacks._repositoryCreate is null)
      {
        return -1;
      }
      var res = callbacks._repositoryCreate(out var repository, path, bare != 0);
      if (res == 0)
      {
        var managedRepository = repository as GitRepository;
        if (managedRepository is null)
          return -1;
        void** ptr = (void**)@out;
        *ptr = (void*)managedRepository.NativeGitRepository.__Instance;
      }
      return res;
    }
    catch (Exception)
    {
      return -1;
    }
  }

  public void Dispose()
  {
    _gcHandle.Free();
  }
}
