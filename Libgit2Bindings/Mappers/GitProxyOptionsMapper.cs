using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class GitProxyOptionsMapper
{
  public static libgit2.GitProxyOptions ToNative(this ProxyOptions proxyOptions,
    DisposableCollection disposables)
  {
    var nativeOptions = new libgit2.GitProxyOptions();
    libgit2.proxy.GitProxyOptionsInit(nativeOptions,
      (uint)libgit2.GitProxyOptionsVersion.GIT_PROXY_OPTIONS_VERSION);

    nativeOptions.Type = proxyOptions.Type.ToNative();
    nativeOptions.Url = proxyOptions.Url;

    if (proxyOptions.CredentialAcquire is not null || proxyOptions.CertificateCheck is not null)
    {
      using var callbacks = new RemoteCallbacksImpl(
        credentialAcquire: proxyOptions.CredentialAcquire,
        certificateCheck: proxyOptions.CertificateCheck)
        .DisposeWith(disposables);

      nativeOptions.Payload = callbacks.Payload;
      if (proxyOptions.CredentialAcquire is not null)
      {
        nativeOptions.Credentials = RemoteCallbacksImpl.GitCredentialAcquireCb;
      }
      if (proxyOptions.CertificateCheck is not null)
      {
        nativeOptions.CertificateCheck = RemoteCallbacksImpl.GitTransportCertificateCheckCb;
      }
    }

    return nativeOptions;
  }

  public static libgit2.GitProxyT ToNative(this GitProxyType gitProxyType)
  {
    return gitProxyType switch
    {
      GitProxyType.None => libgit2.GitProxyT.GIT_PROXY_NONE,
      GitProxyType.Auto => libgit2.GitProxyT.GIT_PROXY_AUTO,
      GitProxyType.Specified => libgit2.GitProxyT.GIT_PROXY_SPECIFIED,
      _ => throw new ArgumentOutOfRangeException(nameof(gitProxyType), gitProxyType, null)
    };
  }
}
