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

      if (managedCallbacks.TransportMessage is not null)
      {
        nativeCallbacks.Transport = RemoteCallbacksImpl.GitTransportCb;
      }

      if (managedCallbacks.OperationCompleted is not null)
      {
        nativeCallbacks.Completion = RemoteCallbacksImpl.GitRemoteCompletionCb;
      }

      if (managedCallbacks.CredentialAcquire is not null)
      {
        nativeCallbacks.Credentials = RemoteCallbacksImpl.GitCredentialAcquireCb;
      }

      if (managedCallbacks.CertificateCheck is not null)
      {
        nativeCallbacks.CertificateCheck = RemoteCallbacksImpl.GitTransportCertificateCheckCb;
      }

      if (managedCallbacks.TransferProgress is not null)
      {
        nativeCallbacks.TransferProgress = RemoteCallbacksImpl.GitTransferProgressCb;
      }

      if (managedCallbacks.UpdateTips is not null)
      {
        nativeCallbacks.UpdateTips = RemoteCallbacksImpl.GitUpdateTipsCb;
      }

      if (managedCallbacks.PackProgress is not null)
      {
        nativeCallbacks.PackProgress = RemoteCallbacksImpl.GitPackbuilderProgressCb;
      }

      if (managedCallbacks.PushTransferProgress is not null)
      {
        nativeCallbacks.PushTransferProgress = RemoteCallbacksImpl.GitPushTransferProgressCb;
      }

      if (managedCallbacks.PushUpdateReference is not null)
      {
        nativeCallbacks.PushUpdateReference = RemoteCallbacksImpl.GitPushUpdateReferenceCb;
      }

      if (managedCallbacks.PushNegotiation is not null)
      {
        nativeCallbacks.PushNegotiation = RemoteCallbacksImpl.GitPushNegotiation;
      }

      if (managedCallbacks.Transport is not null)
      {
        nativeCallbacks.Transport = RemoteCallbacksImpl.GitTransportCb;
      }

      if (managedCallbacks.RemoteReady is not null)
      {
        nativeCallbacks.RemoteReady = RemoteCallbacksImpl.GitRemoteReadyCb;
      }

      return nativeCallbacks;
    }
    catch (Exception)
    {
      nativeCallbacks.Dispose();
      throw;
    }
  }
}
