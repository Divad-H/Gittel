using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class RemoteCallbacksMapper
{
  public static libgit2.GitRemoteCallbacks ToNative(this RemoteCallbacks managedCallbacks, 
    DisposableCollection disposables)
  {
    var nativeCallbacks = new libgit2.GitRemoteCallbacks()
      .DisposeWith(disposables);
    try
    {
      libgit2.remote.GitRemoteInitCallbacks(nativeCallbacks, 
               (uint)libgit2.GitRemoteCallbacksVersion.GIT_REMOTE_CALLBACKS_VERSION);

      var remoteCallbacksImpl = new RemoteCallbacksImpl(
        managedCallbacks.TransportMessage,
        managedCallbacks.OperationCompleted,
        managedCallbacks.CredentialAcquire,
        managedCallbacks.CertificateCheck,
        managedCallbacks.TransferProgress,
        managedCallbacks.UpdateTips,
        managedCallbacks.PackProgress,
        managedCallbacks.PushTransferProgress,
        managedCallbacks.PushUpdateReference,
        managedCallbacks.PushNegotiation,
        managedCallbacks.Transport,
        managedCallbacks.RemoteReady)
        .DisposeWith(disposables);

      nativeCallbacks.Payload = remoteCallbacksImpl.Payload;
      nativeCallbacks.Transport = RemoteCallbacksImpl.GitTransportCb;
      nativeCallbacks.Completion = RemoteCallbacksImpl.GitRemoteCompletionCb;
      nativeCallbacks.Credentials = RemoteCallbacksImpl.GitCredentialAcquireCb;
      nativeCallbacks.CertificateCheck = RemoteCallbacksImpl.GitTransportCertificateCheckCb;
      nativeCallbacks.TransferProgress = RemoteCallbacksImpl.GitTransferProgressCb;
      nativeCallbacks.UpdateTips = RemoteCallbacksImpl.GitUpdateTipsCb;
      nativeCallbacks.PackProgress = RemoteCallbacksImpl.GitPackbuilderProgressCb;
      nativeCallbacks.PushTransferProgress = RemoteCallbacksImpl.GitPushTransferProgressCb;
      nativeCallbacks.PushUpdateReference = RemoteCallbacksImpl.GitPushUpdateReferenceCb;
      nativeCallbacks.PushNegotiation = RemoteCallbacksImpl.GitPushNegotiation;
      nativeCallbacks.Transport = RemoteCallbacksImpl.GitTransportCb;
      nativeCallbacks.RemoteReady = RemoteCallbacksImpl.GitRemoteReadyCb;

      return nativeCallbacks;
    }
    catch (Exception)
    {
      nativeCallbacks.Dispose();
      throw;
    }
  }
}
