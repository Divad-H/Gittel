namespace Libgit2Bindings
{
  public enum RemoteOperationContinuation
  {
    Continue,
    Stop,
  }

  public enum RemoteCompletionType
  {
    Download,
    Indexing,
    Error,
  }

  public record GitIndexerProgress(
    UInt32 TotalObjects, 
    UInt32 IndexedObjects, 
    UInt32 ReceivedObjects, 
    UInt32 LocalObjects,
    UInt32 TotalDeltas,
    UInt32 IndexedDeltas,
    UInt64 ReceivedBytes);

  public enum PackbuilderStage
  {
    AddingObjects,
    Deltafication,
  }

  public record GitPushUpdate(
    string SourceReferenceName,
    string DestinationReferenceName,
    GitOid sourceTarget,
    GitOid destinationdTarget);

  public enum GitRemoteDirection
  {
    Fetch,
    Push,
  }

  /// <summary>
  /// Callback for messages received by the transport.
  /// </summary>
  /// <param name="message">The message from the transport</param>
  /// <returns>Whether to continue the operation.</returns>
  public delegate RemoteOperationContinuation TransportMessageHandler(string message);
  /// <summary>
  /// Completion is called when different parts of the download
  /// process are done(currently unused).
  /// </summary>
  /// <param name="type">part of the download</param>
  /// <returns>Whether to continue the operation</returns>
  public delegate RemoteOperationContinuation OperationCompletedHandler(RemoteCompletionType type);
  /// <summary>
  /// Credential acquisition callback.
  /// </summary>
  /// <param name="credential">The newly created credential object.</param>
  /// <param name="url">The resource for which we are demanding a credential.</param>
  /// <param name="usernameFromUrl">
  /// The username that was embedded in a &quot;user@host&quot;
  /// remote url, or NULL if not included.
  /// </param>
  /// <param name="allowedTypes">A bitmask stating which credential types are OK to return.</param>
  /// <returns>
  /// 0 for success,&lt;0 to indicate an error, &gt; 0 to indicate
  /// no credential was acquired
  /// </returns>
  public delegate int CredentialAcquireHandler(out IGitCredential? credential, string url, string usernameFromUrl, GitCredentialType allowedTypes);
  /// <summary>
  /// Callback for the user's custom certificate checks.
  /// </summary>
  /// <param name="certificate">The host certificate</param>
  /// <param name="valid">Whether the libgit2 checks (OpenSSL or WinHTTP) think
  /// this certificate is valid</param>
  /// <param name="host">Hostname of the host libgit2 connected to</param>
  /// <returns>
  /// 0 to proceed with the connection,&lt;0 to fail the connection
  /// or &gt; 0 to indicate that the callback refused to act and that
  /// the existing validity determination should be honored
  /// </returns>
  public delegate int TransportCertificateCheckHandler(IGitCertificate certificate, bool valid, string host);
  /// <summary>
  /// Type for progress callbacks during indexing.
  /// </summary>
  /// <param name="stats">Record containing information about the state of the transfer</param>
  /// <returns>Whether to continue the operation.</returns>
  public delegate RemoteOperationContinuation IndexerProgressHandler(GitIndexerProgress stats);
  /// <summary>
  /// Each time a reference is updated locally, this function
  /// will be called with information about it.
  /// </summary>
  /// <param name="refname">The name of the updated reference</param>
  /// <param name="oldOid">The old object id</param>
  /// <param name="newOid">The new object id</param>
  /// <returns>Whether to continue the operation</returns>
  public delegate RemoteOperationContinuation UpdateTipsHandler(string refname, GitOid a, GitOid b);
  /// <summary>
  /// Function to call with progress information during pack
  /// building.Be aware that this is called inline with pack
	/// building operations, so performance may be affected.
  /// </summary>
  /// <param name="stage">The current stage of packbuilding</param>
  /// <param name="current">current object</param>
  /// <param name="total">total objects</param>
  /// <returns>Whether to continue the operation</returns>
  public delegate RemoteOperationContinuation PackbuilderProgressHandler(PackbuilderStage stage, UInt32 current, UInt32 total);
  /// <summary>
  /// Function to call with progress information during the upload portion of a push.
  /// Be aware that this is called inline with pack 
  /// building operations, so performance may be affected.
  /// </summary>
  /// <param name="current">current object</param>
  /// <param name="total">total objects</param>
  /// <param name="bytes">Bytes transferred</param>
  /// <returns>Whether to continue the operation</returns>
  public delegate RemoteOperationContinuation PushTransferProgressHandler(UInt32 current, UInt32 total, UInt64 bytes);
  /// <summary>
  /// Callback used to inform of the update status from the remote.
  /// </summary>
  /// <remarks>
  /// Called for each updated reference on push. 
  /// If status is not null, the update was rejected by the remote server and status contains the reason given.
  /// </remarks>
  /// <param name="referenceName">efname specifying to the remote ref</param>
  /// <param name="status">status message sent from the remote</param>
  /// <returns>Whether to continue the operation</returns>
  public delegate RemoteOperationContinuation PushUpdateReferenceHandler(string referenceName, string? status);
  /// <summary>
  /// Called once between the negotiation step and the upload. 
  /// It provides information about what updates will be performed.
  /// </summary>
  /// <param name="updates">an array containing the updates which will be sent as commands to the destination.</param>
  /// <returns>Whether to continue the operation</returns>
  public delegate RemoteOperationContinuation PushNegotiationHandler(IReadOnlyCollection<GitPushUpdate> updates);
  /// <summary>
  /// Create the transport to use for this operation. Leave null to auto-detect.
  /// </summary>
  /// <returns>Whether to continue the operation</returns>
  public delegate RemoteOperationContinuation TransportHandler(out IGitTransport? transport, IGitRemote remote);
  /// <summary>
  /// Callback when the remote is ready to connect.
  /// </summary>
  /// <remarks>
  /// Callback invoked immediately before we attempt to connect to the given url. 
  /// Callers may change the URL before the connection by calling
  /// <see cref="IGitRemote.SetUrl"/> in the callback.
  /// </remarks>
  /// <param name="remote">The remote to be connected</param>
  /// <param name="direction">The direction of the operation</param>
  /// <returns>Whether to continue the operation</returns>
  public delegate RemoteOperationContinuation RemoteReadyHandler(IGitRemote remote, GitRemoteDirection direction);
}
