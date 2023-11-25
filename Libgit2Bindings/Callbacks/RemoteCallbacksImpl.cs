using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;
using System.Runtime.InteropServices;

namespace Libgit2Bindings.Callbacks;

internal class RemoteCallbacksImpl : IDisposable
{
  private readonly GCHandle _gcHandle;

  private readonly TransportMessageHandler? _transportMessage;
  private readonly OperationCompletedHandler? _operationCompleted;
  private readonly CredentialAcquireHandler? _credentialAcquire;
  private readonly TransportCertificateCheckHandler? _certificateCheck;
  private readonly IndexerProgressHandler? _transferProgress;
  private readonly UpdateTipsHandler? _updateTips;
  private readonly PackbuilderProgressHandler? _packProgress;
  private readonly PushTransferProgressHandler? _pushTransferProgress;
  private readonly PushUpdateReferenceHandler? _pushUpdateReference;
  private readonly PushNegotiationHandler? _pushNegotiation;
  private readonly TransportHandler? _transport;
  private readonly RemoteReadyHandler? _remoteReady;

  public RemoteCallbacksImpl(
    TransportMessageHandler? transportMessage = null,
    OperationCompletedHandler? operationCompleted = null,
    CredentialAcquireHandler? credentialAcquire = null,
    TransportCertificateCheckHandler? certificateCheck = null,
    IndexerProgressHandler? transferProgress = null,
    UpdateTipsHandler? updateTips = null,
    PackbuilderProgressHandler? packProgress = null,
    PushTransferProgressHandler? pushTransferProgress = null,
    PushUpdateReferenceHandler? pushUpdateReference = null,
    PushNegotiationHandler? pushNegotiation = null,
    TransportHandler? transport = null,
    RemoteReadyHandler? remoteReady = null)
  {
    _transportMessage = transportMessage;
    _operationCompleted = operationCompleted;
    _credentialAcquire = credentialAcquire;
    _certificateCheck = certificateCheck;
    _transferProgress = transferProgress;
    _updateTips = updateTips;
    _packProgress = packProgress;
    _pushTransferProgress = pushTransferProgress;
    _pushUpdateReference = pushUpdateReference;
    _pushNegotiation = pushNegotiation;
    _transport = transport;
    _remoteReady = remoteReady;

    _gcHandle = GCHandle.Alloc(this);
  }

  public IntPtr Payload => GCHandle.ToIntPtr(_gcHandle);

  public static int GitTransportMessageCb(string message, int len, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (RemoteCallbacksImpl)gcHandle.Target!;
      return callbacks._transportMessage?.Invoke(message) ?? GitOperationContinuation.Continue;
    };

    return func.ExecuteInTryCatch(nameof(RemoteCallbacks.TransportMessage));
  }

  public static int GitRemoteCompletionCb(libgit2.GitRemoteCompletionT type, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (RemoteCallbacksImpl)gcHandle.Target!;
      var managedType = type.ToManaged();
      var res = callbacks._operationCompleted?.Invoke(managedType);
      return res ?? GitOperationContinuation.Continue;
    };

    return func.ExecuteInTryCatch(nameof(RemoteCallbacks.OperationCompleted));
  }

  public unsafe static int GitCredentialAcquireCb(IntPtr cred, string url, string usernameFromUrl, uint allowedTypes, IntPtr payload)
  {
    Func<int> func = () =>
    {
      void** outPtr = (void**)cred;
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);

      var callbacks = (RemoteCallbacksImpl)gcHandle.Target!;
      if (callbacks._credentialAcquire is null)
      {
        *outPtr = (void*)IntPtr.Zero;
        return (int)libgit2.GitErrorCode.GIT_EUSER;
      }

      var nativeAllowedTypes = (libgit2.GitCredentialT)allowedTypes;
      var managedAllowedTypes = nativeAllowedTypes.ToManaged();
      var res = callbacks._credentialAcquire(out var credential, url, usernameFromUrl, managedAllowedTypes);

      using (credential)
      {
        if (credential is not GitCredential managedCredential)
        {
          *outPtr = (void*)IntPtr.Zero;
          return (int)libgit2.GitErrorCode.GIT_EUSER;
        }

        *outPtr = (void*)managedCredential.NativeGitCredential.__Instance;
        managedCredential.Release();
        return res;
      }
    };

    return func.ExecuteInTryCatch(nameof(RemoteCallbacks.CredentialAcquire));
  }

  public static int GitTransportCertificateCheckCb(IntPtr cert, int valid, string host, IntPtr payload)
  {
    Func<int> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);

      var callbacks = (RemoteCallbacksImpl)gcHandle.Target!;
      if (callbacks._certificateCheck is null)
      {
        return (int)libgit2.GitErrorCode.GIT_EUSER;
      }

      using var managedCert = new GitCertificate(libgit2.GitCert.__CreateInstance(cert));
      var res = callbacks._certificateCheck(managedCert, valid != 0, host);
      return res;
    };

    return func.ExecuteInTryCatch(nameof(RemoteCallbacks.CertificateCheck));
  }

  public static int GitTransferProgressCb(IntPtr stats, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);

      var callbacks = (RemoteCallbacksImpl)gcHandle.Target!;
      if (callbacks._transferProgress is null)
      {
        return GitOperationContinuation.Continue;
      }

      var managedStats = GitIndexerProgressMapper.FromNativePtr(stats);
      if (managedStats is null)
      {
        return GitOperationContinuation.Continue;
      }

      return callbacks._transferProgress(managedStats);
    };

    return func.ExecuteInTryCatch(nameof(RemoteCallbacks.TransferProgress));
  }

  public static int GitUpdateTipsCb(string refname, IntPtr a, IntPtr b, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);

      var callbacks = (RemoteCallbacksImpl)gcHandle.Target!;
      if (callbacks._updateTips is null)
      {
        return GitOperationContinuation.Continue;
      }

      var oldOid = GitOidMapper.FromNativePtr(a);
      var newOid = GitOidMapper.FromNativePtr(b);

      return callbacks._updateTips(refname, oldOid, newOid);
    };

    return func.ExecuteInTryCatch(nameof(RemoteCallbacks.UpdateTips));
  }

  public static int GitPackbuilderProgressCb(int stage, uint current, uint total, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);

      var callbacks = (RemoteCallbacksImpl)gcHandle.Target!;
      if (callbacks._packProgress is null)
      {
        return GitOperationContinuation.Continue;
      }

      var managedStage = ((libgit2.GitPackbuilderStageT)stage).ToManaged();
      return callbacks._packProgress(managedStage, current, total);
    };

    return func.ExecuteInTryCatch(nameof(RemoteCallbacks.PackProgress));
  }

  public static int GitPushTransferProgressCb(uint current, uint total, UIntPtr bytes, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);

      var callbacks = (RemoteCallbacksImpl)gcHandle.Target!;
      if (callbacks._pushTransferProgress is null)
      {
        return GitOperationContinuation.Continue;
      }

      return callbacks._pushTransferProgress(current, total, bytes);
    };

    return func.ExecuteInTryCatch(nameof(RemoteCallbacks.PushTransferProgress));
  }

  public static int GitPushUpdateReferenceCb(string refname, string status, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);

      var callbacks = (RemoteCallbacksImpl)gcHandle.Target!;
      if (callbacks._pushUpdateReference is null)
      {
        return GitOperationContinuation.Continue;
      }

      return callbacks._pushUpdateReference(refname, status);
    };

    return func.ExecuteInTryCatch(nameof(RemoteCallbacks.PushUpdateReference));
  }

  public static int GitPushNegotiation(IntPtr updates, UIntPtr len, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);

      var callbacks = (RemoteCallbacksImpl)gcHandle.Target!;
      if (callbacks._pushNegotiation is null)
      {
        return GitOperationContinuation.Cancel;
      }

      List<GitPushUpdate> managedUpdates = new();
      for (int i = 0; i < (int)len; i++)
      {
        var updatePtr = Marshal.ReadIntPtr(updates, i * IntPtr.Size);
        var managedUpdate = GitPushUpdateMapper.FromNativePtr(updatePtr);
        managedUpdates.Add(managedUpdate);
      }

      return callbacks._pushNegotiation(managedUpdates);
    };

    return func.ExecuteInTryCatch(nameof(RemoteCallbacks.PushNegotiation));
  }

  public unsafe static int GitTransportCb(IntPtr @out, IntPtr owner, IntPtr payload)
  {
    Func<int> func = () =>
    {
      void** outPtr = (void**)@out;
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);

      var callbacks = (RemoteCallbacksImpl)gcHandle.Target!;
      if (callbacks._transport is null)
      {
        return (int)libgit2.GitErrorCode.GIT_EUSER;
      }

      using var managedRemote = new GitRemote(libgit2.GitRemote.__CreateInstance(owner), false);

      var res = callbacks._transport(out var managedTransport, managedRemote);
      if (res == GitOperationContinuation.Continue)
      {
        *outPtr = (void*)managedRemote.NativeGitRemote.__Instance;
        return (int)libgit2.GitErrorCode.GIT_OK;
      }
      return (int)libgit2.GitErrorCode.GIT_EUSER;
    };

    return func.ExecuteInTryCatch(nameof(RemoteCallbacks.Transport));
  }

  public static int GitRemoteReadyCb(IntPtr remote, int direction, IntPtr payload)
  {
    Func<GitOperationContinuation> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);

      var callbacks = (RemoteCallbacksImpl)gcHandle.Target!;
      if (callbacks._remoteReady is null)
      {
        return GitOperationContinuation.Continue;
      }

      using var managedRemote = new GitRemote(libgit2.GitRemote.__CreateInstance(remote), false);
      var remoteDirection = GitRemoteDirectionMapper.FromNative((libgit2.GitDirection)direction);

      return callbacks._remoteReady(managedRemote, remoteDirection);
    };

    return func.ExecuteInTryCatch(nameof(RemoteCallbacks.RemoteReady));
  }

  public void Dispose()
  {
    _gcHandle.Free();
  }
}
