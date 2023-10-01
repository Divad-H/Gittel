﻿using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Mappers;

namespace Libgit2Bindings;

internal sealed class GitRepository : IGitRepository
{
  private readonly libgit2.GitRepository _nativeGitRepository;
  public libgit2.GitRepository NativeGitRepository => _nativeGitRepository;

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

  public IGitSignature DefaultGitSignature()
  {
    var res = libgit2.signature.GitSignatureDefault(out var signature, _nativeGitRepository);
    CheckLibgit2.Check(res, "Unable to get default signature");
    return new GitSignature(signature);
  }

  public IGitSignature GitSignatureNow(string name, string email)
  {
    var res = libgit2.signature.GitSignatureNow(out var signature, name, email);
    CheckLibgit2.Check(res, "Unable to create signature");
    return new GitSignature(signature);
  }

  public IGitConfig GetConfig()
  {
    var res = libgit2.repository.GitRepositoryConfig(out var config, _nativeGitRepository);
    CheckLibgit2.Check(res, "Unable to get config");
    return new GitConfig(config);
  }

  public GitOid CreateCommit(
    string? updateRef, IGitSignature author, IGitSignature committer, 
    string? messageEncoding, string message, IGitTree tree, IReadOnlyCollection<IGitCommit>? parents)
  {
    var managedAuthor = author as GitSignature ?? throw new ArgumentException($"{nameof(author)} must be created by Gittel");
    var managedCommitter = committer as GitSignature ?? throw new ArgumentException($"{nameof(committer)} must be created by Gittel");
    var managedTree = tree as GitTree ?? throw new ArgumentException($"{nameof(tree)} must be created by Gittel");
    var nativeParents = parents?
      .Select(p => p as GitCommit ?? throw new ArgumentException($"{nameof(parents)} must be created by Gittel"))
      .Select(parents => parents.NativeGitCommit)
      .ToArray();
    var data = new libgit2.GitOid.__Internal();
    using var commitOid = libgit2.GitOid.__CreateInstance(data);
    var res = libgit2.commit.GitCommitCreate(
      commitOid, _nativeGitRepository, updateRef, managedAuthor.NativeGitSignature,
      managedCommitter.NativeGitSignature, messageEncoding, message, managedTree.NativeGitTree,
      (UInt64)(nativeParents?.Length ?? 0), nativeParents);
    CheckLibgit2.Check(res, "Unable to create commit");
    return GitOidMapper.FromNative(commitOid);
  }

  public IGitIndex GetIndex()
  {
    var res = libgit2.repository.GitRepositoryIndex(out var nativeIndex, _nativeGitRepository);
    CheckLibgit2.Check(res, "Unable to get index");
    return new GitIndex(nativeIndex);
  }

  public IGitTree LookupTree(GitOid oid)
  {
    var res = libgit2.tree.GitTreeLookup(out var nativeTree, _nativeGitRepository, GitOidMapper.ToNative(oid));
    CheckLibgit2.Check(res, "Unable to lookup tree");
    return new GitTree(nativeTree);
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
