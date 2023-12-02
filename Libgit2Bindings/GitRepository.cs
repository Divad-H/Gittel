using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal sealed class GitRepository : IGitRepository
{
  private readonly libgit2.GitRepository _nativeGitRepository;
  private bool _ownsNativeInstance;
  public libgit2.GitRepository NativeGitRepository => _nativeGitRepository;

  public GitRepository(libgit2.GitRepository nativeGitRepository, bool ownsNativeInstance)
  {
    _nativeGitRepository = nativeGitRepository;
    _ownsNativeInstance = ownsNativeInstance;
  }

  public void ReleaseNativeInstance()
  {
    _ownsNativeInstance = false;
  }

  public IGitReference GetHead()
  {
    var res = libgit2.repository.GitRepositoryHead(out var head, _nativeGitRepository);
    CheckLibgit2.Check(res, "Unable to get HEAD");
    return new GitReference(head);
  }

  public void SetHead(string refName)
  {
    var res = libgit2.repository.GitRepositorySetHead(_nativeGitRepository, refName);
    CheckLibgit2.Check(res, "Unable to set HEAD");
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
    using var scope = new DisposableCollection();
    using var nativeOptions = CheckoutOptionsMapper.ToNative(options, scope);

    var res = libgit2.checkout.GitCheckoutHead(_nativeGitRepository, nativeOptions);
    CheckLibgit2.Check(res, "Unable to checkout HEAD");
  }

  public void CheckoutIndex(IGitIndex? index = null, CheckoutOptions? options = null)
  {
    using var scope = new DisposableCollection();
    using var nativeOptions = CheckoutOptionsMapper.ToNative(options, scope);

    var managedIndex = GittelObjects.Downcast<GitIndex>(index);
    var res = libgit2.checkout.GitCheckoutIndex(
      _nativeGitRepository, managedIndex?.NativeGitIndex, nativeOptions);
    CheckLibgit2.Check(res, "Unable to checkout index");
  }

  public IGitReference LookupBranch(string branchName, BranchType branchType)
  {
    var branchTypeNative = BranchTypeMapper.ToNative(branchType);
    var res = libgit2.branch.GitBranchLookup(out var branch, _nativeGitRepository, branchName, branchTypeNative);
    CheckLibgit2.Check(res, "Unable to lookup branch '{0}'", branchName);
    return new GitReference(branch);
  }

  public IEnumerable<GitReferenceBox> LookupBranches(BranchType filterTypes)
  {
    var res = libgit2.branch.GitBranchIteratorNew(
      out var iterator, _nativeGitRepository, BranchTypeMapper.ToNative(filterTypes));
    CheckLibgit2.Check(res, "Unable to create branch iterator");
    try
    {
      while (true)
      {
        libgit2.GitBranchT branchType = 0;
        res = libgit2.branch.GitBranchNext(out var branch, ref branchType, iterator);
        if (res == (int)libgit2.GitErrorCode.GIT_ITEROVER)
        {
          break;
        }
        CheckLibgit2.Check(res, "Unable to iterate over branches");
        var referenceBox = new GitReferenceBox() { Reference = new GitReference(branch) };
        try
        {
          yield return referenceBox;
        }
        finally
        {
          if (referenceBox.AutoDispose)
          {
            referenceBox.Reference.Dispose();
          }
        }
      }
    }
    finally
    {
      libgit2.branch.GitBranchIteratorFree(iterator);
    }
  }

  public string GetRemoteNameFromBranch(string completeTrackingBranchName)
  {
    var res = libgit2.branch.GitBranchRemoteName(out var remoteName, _nativeGitRepository, completeTrackingBranchName);
    using (remoteName.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to get remote name from branch '{0}'", completeTrackingBranchName);
      return StringUtil.ToString(remoteName);
    }
  }

  public string GetBranchUpstreamMerge(string fullBranchName)
  {
    var res = libgit2.branch.GitBranchUpstreamMerge(
      out var upstreamName, _nativeGitRepository, fullBranchName);
    using (upstreamName.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to get upstream merge for branch '{0}'", fullBranchName);
      return StringUtil.ToString(upstreamName);
    }
  }

  public string GetBranchUpstreamName(string localBranchName)
  {
    var res = libgit2.branch.GitBranchUpstreamName(
      out var upstreamName, _nativeGitRepository, localBranchName);
    using (upstreamName.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to get upstream name for branch '{0}'", localBranchName);
      return StringUtil.ToString(upstreamName);
    }
  }

  public string GetBranchUpstreamRemote(string fullBranchName)
  {
    var res = libgit2.branch.GitBranchUpstreamRemote(
      out var upstreamRemote, _nativeGitRepository, fullBranchName);
    using (upstreamRemote.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to get upstream remote for branch '{0}'", fullBranchName);
      return StringUtil.ToString(upstreamRemote);
    }
  }

  public IGitReference CreateBranch(string branchName, IGitCommit target, bool force)
  {
    var concreteTarget = GittelObjects.DowncastNonNull<GitCommit>(target);
    var res = libgit2.branch.GitBranchCreate(
      out var branch, _nativeGitRepository, branchName, concreteTarget.NativeGitCommit, force ? 1 : 0);
    CheckLibgit2.Check(res, "Unable to create branch '{0}'", branchName);
    return new GitReference(branch);
  }

  public IGitReference CreateBranch(string branchName, IGitAnnotatedCommit target, bool force)
  {
    var concreteTarget = GittelObjects.DowncastNonNull<GitAnnotatedCommit>(target);
    var res = libgit2.branch.GitBranchCreateFromAnnotated(
      out var branch, _nativeGitRepository, branchName, concreteTarget.NativeAnnotatedCommit, force ? 1 : 0);
    CheckLibgit2.Check(res, "Unable to create branch '{0}'", branchName);
    return new GitReference(branch);
  }

  public void Cherrypick(IGitCommit commit, CherrypickOptions? options = null)
  {
    var managedCommit = GittelObjects.DowncastNonNull<GitCommit>(commit);
    using var scope = new DisposableCollection();
    using var nativeOptions = options?.ToNative(scope);
    var res = libgit2.cherrypick.GitCherrypick(
       _nativeGitRepository, managedCommit.NativeGitCommit, nativeOptions);
    CheckLibgit2.Check(res, "Unable to cherrypick commit");
  }

  public IGitIndex CherrypickCommit(
    IGitCommit cherrypickCommit, IGitCommit ourCommit, uint mainline, MergeOptions? options = null)
  {
    var managedCherrypickCommit = GittelObjects.DowncastNonNull<GitCommit>(cherrypickCommit);
    var managedOurCommit = GittelObjects.DowncastNonNull<GitCommit>(ourCommit);

    using var scope = new DisposableCollection();
    using var nativeOptions = options?.ToNative(scope);
    var res = libgit2.cherrypick.GitCherrypickCommit(
      out var index, _nativeGitRepository, managedCherrypickCommit.NativeGitCommit,
      managedOurCommit.NativeGitCommit, mainline, nativeOptions);

    CheckLibgit2.Check(res, "Unable to cherrypick commit");
    return new GitIndex(index);
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
    string message, IGitTree tree, IReadOnlyCollection<IGitCommit>? parents)
  {
    var managedAuthor = GittelObjects.DowncastNonNull<GitSignature>(author);
    var managedCommitter = GittelObjects.DowncastNonNull<GitSignature>(committer);
    var managedTree = GittelObjects.DowncastNonNull<GitTree>(tree);
    var nativeParents = parents?
      .Select(GittelObjects.DowncastNonNull<GitCommit>)
      .Select(parents => parents.NativeGitCommit)
      .ToArray();
    var data = new libgit2.GitOid.__Internal();
    using var commitOid = libgit2.GitOid.__CreateInstance(data);
    var res = libgit2.commit.GitCommitCreate(
      commitOid, _nativeGitRepository, updateRef, managedAuthor.NativeGitSignature,
      managedCommitter.NativeGitSignature, null, message, managedTree.NativeGitTree,
      (UIntPtr)(nativeParents?.Length ?? 0), nativeParents);
    CheckLibgit2.Check(res, "Unable to create commit");
    return GitOidMapper.FromNative(commitOid);
  }

  public byte[] CreateCommitObject(IGitSignature author, IGitSignature committer,
    string message, IGitTree tree, IReadOnlyCollection<IGitCommit>? parents)
  {
    var managedAuthor = GittelObjects.DowncastNonNull<GitSignature>(author);
    var managedCommitter = GittelObjects.DowncastNonNull<GitSignature>(committer);
    var managedTree = GittelObjects.DowncastNonNull<GitTree>(tree);
    var nativeParents = parents?
      .Select(GittelObjects.DowncastNonNull<GitCommit>)
      .Select(parents => parents.NativeGitCommit)
      .ToArray();
    var res = libgit2.commit.GitCommitCreateBuffer(
      out var commitBuffer, _nativeGitRepository, managedAuthor.NativeGitSignature,
      managedCommitter.NativeGitSignature, null, message, managedTree.NativeGitTree,
      (UIntPtr)(parents?.Count ?? 0), nativeParents);
    using (commitBuffer.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to create commit object");
      return StringUtil.ToArray(commitBuffer);
    }
  }

  public GitOid CreateCommitWithSignature(string commitContent, string? signature, string? signatureField)
  {
    var res = libgit2.commit.GitCommitCreateWithSignature(
      out var commitOid, _nativeGitRepository, commitContent, signature, signatureField);
    using (commitOid)
    {
      CheckLibgit2.Check(res, "Unable to create commit with signature");
      return GitOidMapper.FromNative(commitOid);
    }
  }

  public (byte[] Signature, byte[] SignedData) ExtractCommitSignature(
    GitOid commitId, string? signatureField)
  {
    using var nativeCommitId = GitOidMapper.ToNative(commitId);
    var res = libgit2.commit.GitCommitExtractSignature(
      out var signature, out var signedData, _nativeGitRepository, nativeCommitId, signatureField);
    using (signature) using (signedData)
    {
      CheckLibgit2.Check(res, "Unable to extract commit signature");
      return (StringUtil.ToArray(signature), StringUtil.ToArray(signedData));
    }
  }

  public IGitCommit LookupCommit(GitOid oid)
  {
    using var nativeOid = GitOidMapper.ToNative(oid);
    var res = libgit2.commit.GitCommitLookup(out var commit, _nativeGitRepository, nativeOid);
    CheckLibgit2.Check(res, "Unable to lookup commit");
    return new GitCommit(commit, this);
  }

  public IGitCommit LookupCommitPrefix(byte[] shortId)
  {
    GitOid gitOid = GitOidMapper.FromShortId(shortId);
    using var nativeOid = GitOidMapper.ToNative(gitOid);
    var res = libgit2.commit.GitCommitLookupPrefix(out var commit,
      _nativeGitRepository, nativeOid, (UIntPtr)shortId.Length);
    CheckLibgit2.Check(res, "Unable to lookup commit");
    return new GitCommit(commit, this);
  }

  public IGitCommit LookupCommitPrefix(string shortSha)
  {
    if (shortSha.Length % 2 != 0)
    {
      shortSha += "0";
    }
    var shortId = Convert.FromHexString(shortSha);
    return LookupCommitPrefix(shortId);
  }

  public IGitIndex GetIndex()
  {
    var res = libgit2.repository.GitRepositoryIndex(out var nativeIndex, _nativeGitRepository);
    CheckLibgit2.Check(res, "Unable to get index");
    return new GitIndex(nativeIndex);
  }

  public IGitTree LookupTree(GitOid oid)
  {
    using var nativeOid = GitOidMapper.ToNative(oid);
    var res = libgit2.tree.GitTreeLookup(out var nativeTree, _nativeGitRepository, nativeOid);
    CheckLibgit2.Check(res, "Unable to lookup tree");
    return new GitTree(nativeTree);
  }

  public IGitMailmap GetMailmap()
  {
    var res = libgit2.mailmap.GitMailmapFromRepository(out var nativeMailmap, _nativeGitRepository);
    CheckLibgit2.Check(res, "Unable to get mailmap");
    return new GitMailmap(nativeMailmap);
  }

  public bool IsBare()
  {
    var res = libgit2.repository.GitRepositoryIsBare(_nativeGitRepository);
    return res != 0;
  }

  public IGitRemote CreateRemote(string name, string url)
  {
    var res = libgit2.remote.GitRemoteCreate(out var remote, _nativeGitRepository, name, url);
    CheckLibgit2.Check(res, "Unable to create remote");
    return new GitRemote(remote, true);
  }

  public IGitRemote LookupRemote(string name)
  {
    var res = libgit2.remote.GitRemoteLookup(out var remote, _nativeGitRepository, name);
    CheckLibgit2.Check(res, "Unable to lookup remote");
    return new GitRemote(remote, true);
  }

  public IGitAnnotatedCommit GetAnnotatedCommitFromFetchhead(string branchName, string remoteUrl, GitOid Id)
  {
    using var nativeId = GitOidMapper.ToNative(Id);
    var res = libgit2.annotated_commit.GitAnnotatedCommitFromFetchhead(
      out var nativeAnnotatedCommit, _nativeGitRepository, branchName, remoteUrl, nativeId);
    CheckLibgit2.Check(res, "Unable to get annotated commit from fetchhead");
    return new GitAnnotatedCommit(nativeAnnotatedCommit);
  }

  public IGitAnnotatedCommit GetAnnotatedCommitFromRef(IGitReference gitReferencee)
  {
    var managedReference = GittelObjects.DowncastNonNull<GitReference>(gitReferencee);
    var res = libgit2.annotated_commit.GitAnnotatedCommitFromRef(
      out var nativeAnnotatedCommit, _nativeGitRepository, managedReference.NativeGitReference);
    CheckLibgit2.Check(res, "Unable to get annotated commit from ref");
    return new GitAnnotatedCommit(nativeAnnotatedCommit);
  }

  public IGitAnnotatedCommit GetAnnotatedCommitFromRevspec(string refspec)
  {
    var res = libgit2.annotated_commit.GitAnnotatedCommitFromRevspec(
      out var nativeAnnotatedCommit, _nativeGitRepository, refspec);
    CheckLibgit2.Check(res, "Unable to get annotated commit from revspec");
    return new GitAnnotatedCommit(nativeAnnotatedCommit);
  }

  public IGitAnnotatedCommit AnnotatedCommitLookup(GitOid id)
  {
    using var nativeId = GitOidMapper.ToNative(id);
    var res = libgit2.annotated_commit.GitAnnotatedCommitLookup(
      out var nativeAnnotatedCommit, _nativeGitRepository, nativeId);
    CheckLibgit2.Check(res, "Unable to lookup annotated commit");
    return new GitAnnotatedCommit(nativeAnnotatedCommit);
  }

  public IGitDiff DiffTreeToWorkdir(IGitTree? tree, GitDiffOptions? options = null)
  {
    using var scope = new DisposableCollection();
    using var nativeOptions = options?.ToNative(scope);
    var managedTree = GittelObjects.Downcast<GitTree>(tree);
    var res = libgit2.diff.GitDiffTreeToWorkdir(
      out var nativeDiff, _nativeGitRepository, managedTree?.NativeGitTree, nativeOptions);
    CheckLibgit2.Check(res, "Unable to diff tree to workdir");
    return new GitDiff(nativeDiff);
  }

  public IGitBlob LookupBlob(GitOid oid)
  {
    using var nativeOid = GitOidMapper.ToNative(oid);
    var res = libgit2.blob.GitBlobLookup(out var nativeBlob, _nativeGitRepository, nativeOid);
    CheckLibgit2.Check(res, "Unable to lookup blob");
    return new GitBlob(nativeBlob);
  }

  public IGitBlob LookupBlobByPrefix(byte[] shortId)
  {
    var gitOid = GitOidMapper.FromShortId(shortId);
    using var nativeOid = GitOidMapper.ToNative(gitOid);
    var res = libgit2.blob.GitBlobLookupPrefix(
      out var nativeBlob, _nativeGitRepository, nativeOid, (UIntPtr)shortId.Length);
    CheckLibgit2.Check(res, "Unable to lookup blob");
    return new GitBlob(nativeBlob);
  }

  public IGitBlob LookupBlobByPrefix(string shortSha)
  {
    if (shortSha.Length % 2 != 0)
    {
      shortSha += "0";
    }
    var shortId = Convert.FromHexString(shortSha);
    return LookupBlobByPrefix(shortId);
  }

  public GitOid CreateBlob(byte[] data)
  {
    using PinnedBuffer pinnedBuffer = new(data);
    var res = libgit2.blob.GitBlobCreateFromBuffer(
      out var nativeId, _nativeGitRepository, pinnedBuffer.Pointer, (UIntPtr)pinnedBuffer.Length);
    CheckLibgit2.Check(res, "Unable to create blob");
    using (nativeId)
    {
      return GitOidMapper.FromNative(nativeId);
    }
  }

  public GitOid CreateBlobFromDisk(string path)
  {
    var res = libgit2.blob.GitBlobCreateFromDisk(out var nativeId, _nativeGitRepository, path);
    CheckLibgit2.Check(res, "Unable to create blob from disk");
    using (nativeId)
    {
      return GitOidMapper.FromNative(nativeId);
    }
  }

  public AbstractGitWriteStream CreateBlobFromStream(string? hintpath)
  {
    var res = libgit2.blob.GitBlobCreateFromStream(out var nativeStream, _nativeGitRepository, hintpath);
    CheckLibgit2.Check(res, "Unable to create blob from stream");
    return new GitWriteStream(nativeStream);
  }

  public GitOid CreateBlobFromWorkdir(string relativePath)
  {
    var res = libgit2.blob.GitBlobCreateFromWorkdir(out var nativeId, _nativeGitRepository, relativePath);
    CheckLibgit2.Check(res, "Unable to create blob from workdir");
    using (nativeId)
    {
      return GitOidMapper.FromNative(nativeId);
    }
  }

  public IGitBlame BlameFile(string path, GitBlameOptions? options = null)
  {
    using var scope = new DisposableCollection();
    using var nativeOptions = options?.ToNative(scope);
    var res = libgit2.blame.GitBlameFile(out var nativeBlame, _nativeGitRepository, path, nativeOptions);
    CheckLibgit2.Check(res, "Unable to blame file");
    return new GitBlame(nativeBlame);
  }

  public IGitObject LookupObject(GitOid oid, GitObjectType type)
  {
    using var nativeOid = GitOidMapper.ToNative(oid);
    var res = libgit2.@object.GitObjectLookup(
      out var nativeObject, _nativeGitRepository, nativeOid, type.ToNative());
    CheckLibgit2.Check(res, "Unable to lookup object");
    return new GitObject(nativeObject);
  }

  public IGitObject LookupObjectByPrefix(string shortId, GitObjectType type)
  {
    if (shortId.Length % 2 != 0)
    {
      shortId += "0";
    }
    using var nativeShortId = GitOidMapper.ToNative(GitOidMapper.FromShortId(Convert.FromHexString(shortId)));
    var res = libgit2.@object.GitObjectLookupPrefix(
      out var nativeObject, _nativeGitRepository, nativeShortId, (UIntPtr)shortId.Length / 2, type.ToNative());
    CheckLibgit2.Check(res, "Unable to lookup object");
    return new GitObject(nativeObject);
  }

  public IGitDescribeResult DescribeWorkdir(GitDescribeOptions? options = null)
  {
    using var nativeOptions = options?.ToNative();
    var res = libgit2.describe.GitDescribeWorkdir(
      out var nativeDescribeResult, _nativeGitRepository, nativeOptions);
    CheckLibgit2.Check(res, "Unable to describe workdir");
    return new GitDescribeResult(nativeDescribeResult);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (_ownsNativeInstance)
      {
        libgit2.repository.GitRepositoryFree(_nativeGitRepository);
      }
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
