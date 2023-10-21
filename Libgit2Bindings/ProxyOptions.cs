namespace Libgit2Bindings;

/// <summary>
/// The type of proxy to use.
/// </summary>
public enum GitProxyType
{
  /// <summary>
  /// Do not attempt to connect through a proxy
	/// <para/>
	/// If libgit2 is built against libcurl, it itself may attempt to connect
	/// to a proxy if the environment variables specify it.
  /// </summary>
  None,
  /// <summary>
  /// Try to auto-detect the proxy from the git configuration.
  /// </summary>
  Auto,
  /// <summary>
  /// Connect via the URL given in the options
  /// </summary>
  Specified,
}

public record ProxyOptions
{
  /// <summary>
  /// The type of proxy to use, by URL, auto-detect.
  /// </summary>
  public GitProxyType Type { get; init; } = GitProxyType.Auto;

  /// <summary>
  /// The URL of the proxy.
  /// </summary>
  public string? Url { get; init; }

  /// <summary>
  /// This will be called if the remote host requires
	/// authentication in order to connect to it.
  /// <para/>
  /// Returning <see cref="CallbackResults.Passthrough"/> will make libgit2 behave as
	/// though this field isn't set.
  /// </summary>
  public CredentialAcquireHandler? CredentialAcquire { get; init; }

  /// <summary>
  /// If cert verification fails, this will be called to let the
	/// user make the final decision of whether to allow the
	/// connection to proceed.Returns 0 to allow the connection
	/// or a negative value to indicate an error
  /// </summary>
  public TransportCertificateCheckHandler? CertificateCheck { get; init; }
}
