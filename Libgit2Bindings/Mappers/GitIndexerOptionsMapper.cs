using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Util;

namespace Libgit2Bindings.Mappers;

internal static class GitIndexerOptionsMapper
{
  public static libgit2.GitIndexerOptions ToNative(
    this GitIndexerOptions options, DisposableCollection disposables)
  {
    var callbacks = new RemoteCallbacksImpl(transferProgress: options.ProgressCallback)
      .DisposeWith(disposables);

    return new()
    {
      Version = (UInt32)libgit2.GitIndexerOptionsVersion.GIT_INDEXER_OPTIONS_VERSION,
      ProgressCb = RemoteCallbacksImpl.GitTransferProgressCb,
      ProgressCbPayload = callbacks.Payload,
      Verify = options.Verify ? (byte)1 : (byte)0,
    };
  }
}
