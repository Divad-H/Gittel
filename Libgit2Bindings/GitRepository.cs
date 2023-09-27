namespace Libgit2Bindings;

internal sealed class GitRepository : IGitRepository
{
  private readonly libgit2.GitRepository _nativeGitRepository;

  public GitRepository(libgit2.GitRepository nativeGitRepository)
  {
    _nativeGitRepository = nativeGitRepository;
  }

  public IGitReference GetHead()
  {
    var res = libgit2.repository.GitRepositoryHead(out var head, _nativeGitRepository);
    CheckLibgit2.Check(res, "Unable to get HEAD");
    return new GitReference(head);
  }

  public string GetPath()
  {
    return libgit2.repository.GitRepositoryPath(_nativeGitRepository);
  }

  public string? GetWorkdir()
  {
    return libgit2.repository.GitRepositoryWorkdir(_nativeGitRepository);
  }

  public string GetCommonDir()
  {
    return libgit2.repository.GitRepositoryCommondir(_nativeGitRepository);
  }

  public void CheckoutHead(CheckoutOptions? options = null)
  {
    libgit2.GitCheckoutOptions nativeOptions = new();
    libgit2.checkout.GitCheckoutOptionsInit(nativeOptions, (uint)libgit2.GitCheckoutOptionsVersion.GIT_CHECKOUT_OPTIONS_VERSION);

    using var callbacks = options?.NotifyCallback is not null || options?.ProgressCallback is not null 
      ? new CheckoutCallbacks(options.NotifyCallback, options.ProgressCallback) : null;

    if (options is not null)
    {
      nativeOptions.CheckoutStrategy = (uint)Mappers.CheckoutStrategyMapper.ToNative(options.Strategy);
      nativeOptions.DisableFilters = options.DisableFilters ? 1 : 0;
      nativeOptions.NotifyFlags = (uint)Mappers.CheckoutNotifyFlagsMapper.ToNative(options.NotifyFlags);

      if (callbacks is not null)
      {
        if (options.NotifyCallback is not null)
        {
          nativeOptions.NotifyCb = CheckoutCallbacks.GitCheckoutNotifyCb;
          nativeOptions.NotifyPayload = callbacks.Payload;
        }
        if (options.ProgressCallback is not null)
        {
          nativeOptions.ProgressCb = CheckoutCallbacks.GitCheckoutProgressCb;
          nativeOptions.ProgressPayload = callbacks.Payload;
        }
      }
    }

    var res = libgit2.checkout.GitCheckoutHead(_nativeGitRepository, nativeOptions);
    CheckLibgit2.Check(res, "Unable to checkout HEAD");
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.repository.GitRepositoryFree(_nativeGitRepository);
      _disposedValue = true;
    }
  }

  ~GitRepository()
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
