using Libgit2Bindings.Callbacks;
using Libgit2Bindings.Mappers;
using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal class Libgit2 : ILibgit2, IDisposable
{
  public Libgit2()
  {
    libgit2.global.GitLibgit2Init();
  }

  public IGitRepository OpenRepository(string path)
  {
    var res = libgit2.repository.GitRepositoryOpen(out var repo, path);
    CheckLibgit2.Check(res, "Unable to open repository '{0}'", path);
    return new GitRepository(repo, true);
  }

  public IGitRepository InitRepository(string path, bool isBare)
  {
    var res = libgit2.repository.GitRepositoryInit(out var repo, path, isBare ? 1u : 0);
    CheckLibgit2.Check(res, "Unable to initialize repository '{0}'", path);
    return new GitRepository(repo, true);
  }

  public IGitRepository Clone(string url, string localPath, CloneOptions? options = null)
  {
    using DisposableCollection disposable = new();
    using var nativeOptions = options?.ToNative(disposable);

    var res = libgit2.clone.GitClone(out var repo, url, localPath, nativeOptions);
    CheckLibgit2.Check(res, "Unable to clone repository '{0}'", url);
    return new GitRepository(repo, true);
  }

  public string DiscoverRepository(string startPath, bool acrossFilesystem, string[] ceilingDirectories)
  {
    var res = libgit2.repository.GitRepositoryDiscover(
      out var repoPath, 
      startPath, 
      acrossFilesystem ? 1 : 0, 
      ceilingDirectories.Length != 0 ? string.Join((char)libgit2.PathListSeparator.GIT_PATH_LIST_SEPARATOR, ceilingDirectories) : null);

    using (repoPath.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to discover repository '{0}'", startPath);
      return StringUtil.ToString(repoPath);
    }
  }

  public IGitSignature CreateGitSignature(string signature)
  {
    var res = libgit2.signature.GitSignatureFromBuffer(out var nativeSignature, signature);
    CheckLibgit2.Check(res, "Unable to create signature");
    return new GitSignature(nativeSignature);
  }

  public IGitSignature CreateGitSignature(string name, string email, DateTimeOffset when)
  {
    var res = libgit2.signature.GitSignatureNew(
      out var signature, name, email, when.ToUnixTimeSeconds(), (int)when.Offset.TotalMinutes);
    CheckLibgit2.Check(res, "Unable to create signature");
    return new GitSignature(signature);
  }

  public string FindGlobalConfig()
  {
    var res = libgit2.config.GitConfigFindGlobal(out var path);
    using (path.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to find global config");
      return StringUtil.ToString(path);
    }
  }

  public string FindSystemConfig()
  {
    var res = libgit2.config.GitConfigFindSystem(out var path);
    using (path.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to find system config");
      return StringUtil.ToString(path);
    }
  }

  public string FindProgramdataConfig()
  {
    var res = libgit2.config.GitConfigFindProgramdata(out var path);
    using (path.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to find programdata config");
      return StringUtil.ToString(path);
    }
  }

  public string FindXdgConfig()
  {
    var res = libgit2.config.GitConfigFindXdg(out var path);
    using (path.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to find xdg config");
      return StringUtil.ToString(path);
    }
  }

  public IGitConfig NewConfig()
  {
    var res = libgit2.config.GitConfigNew(out var config);
    CheckLibgit2.Check(res, "Unable to create config");
    return new GitConfig(config);
  }

  public IGitConfig OpenConfigOndisk(string path)
  {
    var res = libgit2.config.GitConfigOpenOndisk(out var config, path);
    CheckLibgit2.Check(res, "Unable to open config");
    return new GitConfig(config);
  }

  public bool ParseConfigBool(string value)
  {
    var res = libgit2.config.GitConfigParseBool(out var result, value);
    CheckLibgit2.Check(res, "Unable to parse config value");
    return result != 0;
  }

  public int ParseConfigInt32(string value)
  {
    var res = libgit2.config.GitConfigParseInt32(out var result, value);
    CheckLibgit2.Check(res, "Unable to parse config value");
    return result;
  }

  public long ParseConfigInt64(string value)
  {
    var res = libgit2.config.GitConfigParseInt64(out var result, value);
    CheckLibgit2.Check(res, "Unable to parse config value");
    return result;
  }

  public string ParseConfigPath(string value)
  {
    var res = libgit2.config.GitConfigParsePath(out var result, value);
    using (result.GetDisposer())
    {
      CheckLibgit2.Check(res, "Unable to parse config value");
      return StringUtil.ToString(result);
    }
  }

  public bool BlobDataIsBinary(byte[] blobData)
  {
    using var pinnedBuffer = new PinnedBuffer(blobData);
    var res = libgit2.blob.GitBlobDataIsBinary(pinnedBuffer.Pointer, (UIntPtr)pinnedBuffer.Length);
    CheckLibgit2.Check(res, "Unable to check if blob data is binary");
    return res != 0;
  }

  public bool BranchNameIsValid(string branchName)
  {
    int valid = 0;
    var res = libgit2.branch.GitBranchNameIsValid(ref valid, branchName);
    CheckLibgit2.Check(res, "Unable to check if branch name is valid");
    return valid != 0;
  }

  public bool ReferenceNameIsValid(string referenceName)
  {
    int valid = 0;
    var res = libgit2.refs.GitReferenceNameIsValid(ref valid, referenceName);
    CheckLibgit2.Check(res, "Unable to check if reference name is valid");
    return valid != 0;
  }

  public string NormalizeReferenceName(string referenceName, GitReferenceFormat format)
  {
    nuint bufSize = 1024;
    int res;
    do
    {
      byte[] buffer = new byte[bufSize];
      using var result = new PinnedBuffer(buffer);
      unsafe
      {
        res = libgit2.refs.GitReferenceNormalizeName(
          (sbyte*)result.Pointer, bufSize, referenceName, (UInt32)format);
      }
      if (res == (int)libgit2.GitErrorCode.GIT_EBUFS)
      {
        bufSize *= 2;
        continue;
      }
      CheckLibgit2.Check(res, "Unable to normalize reference name");
      unsafe
      {
        return StringUtil.ToString((sbyte*)result.Pointer);
      }
    } while(res == (int)libgit2.GitErrorCode.GIT_EBUFS);
    throw new Libgit2Exception("Unable to normalize reference name", -1);
  }

  public bool GitObjectTypeIsLoose(GitObjectType type)
  {
    return libgit2.@object.GitObjectTypeisloose(type.ToNative()) != 0;
  }

  public bool GitObjectRawContentIsValid(byte[] rawContent, GitObjectType type)
  {
    using var pinnedBuffer = new PinnedBuffer(rawContent);
    int valid = 0;
    var res = libgit2.@object.GitObjectRawcontentIsValid(
      ref valid, pinnedBuffer.Pointer, (UIntPtr)pinnedBuffer.Length, type.ToNative());
    CheckLibgit2.Check(res, "Unable to check if raw content is valid");
    return valid != 0;
  }

  public void DiffBlobToBuffer(
    IGitBlob? oldBlob, string? oldAsPath, byte[]? newBuffer, string? newBufferAsPath, 
    GitDiffOptions? options = null, 
    IGitDiff.FileCallback? fileCallback = null,
    IGitDiff.BinaryCallback? binaryCallback = null,
    IGitDiff.HunkCallback? hunkCallback = null,
    IGitDiff.LineCallback? lineCallback = null)
  {
    using DisposableCollection disposable = new();
    using var nativeOptions = options?.ToNative(disposable);

    using var managedBlob = GittelObjects.Downcast<GitBlob>(oldBlob);

    using var callbacks = new GitDiffCallbacks(fileCallback, binaryCallback, hunkCallback, lineCallback);

    using var newNativeBuffer = newBuffer is not null ? new PinnedBuffer(newBuffer) : null;

    var res = libgit2.diff.GitDiffBlobToBuffer(
      managedBlob?.NativeGitBlob, oldAsPath, newNativeBuffer?.Pointer ?? IntPtr.Zero, 
      (UIntPtr)(newNativeBuffer?.Length ?? 0), newBufferAsPath, nativeOptions,
      fileCallback is null ? null : GitDiffCallbacks.GitDiffFileCb,
      binaryCallback is null ? null : GitDiffCallbacks.GitDiffBinaryCb,
      hunkCallback is null ? null : GitDiffCallbacks.GitDiffHunkCb,
      lineCallback is null ? null : GitDiffCallbacks.GitDiffLineCb,
      callbacks.Payload);
    CheckLibgit2.Check(res, "Unable to diff blob to buffer");
  }

  public IGitPatch PatchFromBlobAndBuffer(
    IGitBlob? oldBlob, string? oldAsPath, 
    byte[]? newBuffer, string? newBufferAsPath = null, 
    GitDiffOptions? options = null)
  {
    using DisposableCollection disposable = new();
    using var nativeOptions = options?.ToNative(disposable);

    using var managedBlob = GittelObjects.Downcast<GitBlob>(oldBlob);

    using var newNativeBuffer = newBuffer is not null ? new PinnedBuffer(newBuffer) : null;

    var res = libgit2.patch.GitPatchFromBlobAndBuffer(
      out var patch, managedBlob?.NativeGitBlob, oldAsPath, 
      newNativeBuffer?.Pointer ?? IntPtr.Zero, (UIntPtr)(newNativeBuffer?.Length ?? 0), newBufferAsPath, 
      nativeOptions);
    CheckLibgit2.Check(res, "Unable to create patch from blob and buffer");
    return new GitPatch(patch);
  }

  public void DiffBlobs(IGitBlob? oldBlob, string? oldAsPath, 
    IGitBlob? newBlob, string? newBlobAsPath = null, 
    GitDiffOptions? options = null,
    IGitDiff.FileCallback? fileCallback = null, 
    IGitDiff.BinaryCallback? binaryCallback = null, 
    IGitDiff.HunkCallback? hunkCallback = null, 
    IGitDiff.LineCallback? lineCallback = null)
  {
    using DisposableCollection disposable = new();
    using var nativeOptions = options?.ToNative(disposable);

    using var managedOldBlob = GittelObjects.Downcast<GitBlob>(oldBlob);
    using var managedNewBlob = GittelObjects.Downcast<GitBlob>(newBlob);

    using var callbacks = new GitDiffCallbacks(fileCallback, binaryCallback, hunkCallback, lineCallback);

    var res = libgit2.diff.GitDiffBlobs(
      managedOldBlob?.NativeGitBlob, oldAsPath, 
      managedNewBlob?.NativeGitBlob, newBlobAsPath, 
      nativeOptions,
      fileCallback is null ? null : GitDiffCallbacks.GitDiffFileCb,
      binaryCallback is null ? null : GitDiffCallbacks.GitDiffBinaryCb,
      hunkCallback is null ? null : GitDiffCallbacks.GitDiffHunkCb,
      lineCallback is null ? null : GitDiffCallbacks.GitDiffLineCb,
      callbacks.Payload);
    CheckLibgit2.Check(res, "Unable to diff blobs");
  }

  public IGitPatch PatchFromBlobs(
    IGitBlob? oldBlob, string? oldAsPath, 
    IGitBlob? newBlob, string? newBlobAsPath = null, 
    GitDiffOptions? options = null)
  {
    using DisposableCollection disposable = new();
    using var nativeOptions = options?.ToNative(disposable);

    using var managedOldBlob = GittelObjects.Downcast<GitBlob>(oldBlob);
    using var managedNewBlob = GittelObjects.Downcast<GitBlob>(newBlob);

    var res = libgit2.patch.GitPatchFromBlobs(
      out var patch, managedOldBlob?.NativeGitBlob, oldAsPath, 
      managedNewBlob?.NativeGitBlob, newBlobAsPath, nativeOptions);
    CheckLibgit2.Check(res, "Unable to create patch from blobs");
    return new GitPatch(patch);
  }

  public void DiffBuffers(
    byte[]? oldBuffer, string? oldAsPath, byte[]? newBuffer, string? newAsPath, 
    GitDiffOptions? options = null, 
    IGitDiff.FileCallback? fileCallback = null, 
    IGitDiff.BinaryCallback? binaryCallback = null, 
    IGitDiff.HunkCallback? hunkCallback = null, 
    IGitDiff.LineCallback? lineCallback = null)
  {
    using DisposableCollection disposable = new();
    using var nativeOptions = options?.ToNative(disposable);

    using var callbacks = new GitDiffCallbacks(fileCallback, binaryCallback, hunkCallback, lineCallback);

    using var oldNativeBuffer = oldBuffer is not null ? new PinnedBuffer(oldBuffer) : null;
    using var newNativeBuffer = newBuffer is not null ? new PinnedBuffer(newBuffer) : null;

    var res = libgit2.diff.GitDiffBuffers(
      oldNativeBuffer?.Pointer ?? IntPtr.Zero, (UIntPtr)(oldNativeBuffer?.Length ?? 0), oldAsPath,
      newNativeBuffer?.Pointer ?? IntPtr.Zero, (UIntPtr)(newNativeBuffer?.Length ?? 0), newAsPath,
      nativeOptions,
      fileCallback is null ? null : GitDiffCallbacks.GitDiffFileCb,
      binaryCallback is null ? null : GitDiffCallbacks.GitDiffBinaryCb,
      hunkCallback is null ? null : GitDiffCallbacks.GitDiffHunkCb,
      lineCallback is null ? null : GitDiffCallbacks.GitDiffLineCb,
      callbacks.Payload);
  }

  public IGitPatch PatchFromBuffers(
    byte[]? oldBuffer, string? oldAsPath, 
    byte[]? newBuffer, string? newAsPath, 
    GitDiffOptions? options = null)
  {
    using DisposableCollection disposable = new();
    using var nativeOptions = options?.ToNative(disposable);

    using var oldNativeBuffer = oldBuffer is not null ? new PinnedBuffer(oldBuffer) : null;
    using var newNativeBuffer = newBuffer is not null ? new PinnedBuffer(newBuffer) : null;

    var res = libgit2.patch.GitPatchFromBuffers(
      out var patch, 
      oldNativeBuffer?.Pointer ?? IntPtr.Zero, (UIntPtr)(oldNativeBuffer?.Length ?? 0), oldAsPath,
      newNativeBuffer?.Pointer ?? IntPtr.Zero, (UIntPtr)(newNativeBuffer?.Length ?? 0), newAsPath,
      nativeOptions);
    CheckLibgit2.Check(res, "Unable to create patch from buffers");
    return new GitPatch(patch);
  }

  public IGitDiff DiffFromPatch(byte[] patch)
  {
    using var pinnedBuffer = new PinnedBuffer(patch);
    var res = libgit2.diff.GitDiffFromBuffer(
      out var diff, pinnedBuffer.Pointer, (UIntPtr)pinnedBuffer.Length);
    CheckLibgit2.Check(res, "Unable to create diff from patch");
    return new GitDiff(diff, true);
  }

  public sbyte GetDiffStatusCharByte(GitDeltaType gitDelta)
  {
    return libgit2.diff.GitDiffStatusChar(gitDelta.ToNative());
  }

  public char GetDiffStatusChar(GitDeltaType gitDelta)
  {
    return (char)GetDiffStatusCharByte(gitDelta);
  }

  public IGitIndexer NewGitIndexer(string path, uint mode, IGitOdb? odb, GitIndexerOptions? options = null)
  {
    DisposableCollection? disposables = new();
    try
    {
      using var nativeOptions = options?.ToNative(disposables);
      using var managedOdb = GittelObjects.Downcast<GitOdb>(odb);

      var res = libgit2.indexer.GitIndexerNew(
        out var indexer, path, mode, managedOdb?.NativeGitOdb, nativeOptions);
      CheckLibgit2.Check(res, "Unable to create indexer");
      var gitIndexer = new GitIndexer(indexer, disposables);
      disposables = null;
      return gitIndexer;
    }
    finally
    {
      disposables?.Dispose();
    }
  }

  public IGitMailmap NewGitMailmap()
  {
    var res = libgit2.mailmap.GitMailmapNew(out var mailmap);
    CheckLibgit2.Check(res, "Unable to create mailmap");
    return new GitMailmap(mailmap);
  }

  public IGitMailmap NewGitMailmapFromBuffer(byte[] buffer)
  {
    using var pinnedBuffer = new PinnedBuffer(buffer);
    var res = libgit2.mailmap.GitMailmapFromBuffer(
      out var mailmap, pinnedBuffer.Pointer, (UIntPtr)pinnedBuffer.Length);
    CheckLibgit2.Check(res, "Unable to create mailmap from buffer");
    return new GitMailmap(mailmap);
  }

  public IReadOnlyList<GitMessageTrailer> ParseGitMessageTrailers(byte[] message)
  {
    using libgit2.GitMessageTrailerArray gitMessageTrailerArray = new();
    if (!message.Contains((byte)0))
    {
      message = [.. message, (byte)0];
    }
    using var pinnedBuffer = new PinnedBuffer(message);
    var res = libgit2.message.__Internal.GitMessageTrailers(
      gitMessageTrailerArray.__Instance, pinnedBuffer.Pointer);
      CheckLibgit2.Check(res, "Unable to parse git message trailers");
    try
    {
      unsafe 
      {
        var pTrailerArray = ((libgit2.GitMessageTrailerArray.__Internal*)gitMessageTrailerArray.__Instance);
        var pTrailers = pTrailerArray->trailers;
        var count = pTrailerArray->count;
        List<GitMessageTrailer> result = new((int)count);
        for (UInt64 i = 0; i < count; ++i)
        {
          using var trailer= libgit2.GitMessageTrailer.__CreateInstance(pTrailers);
          result.Add(new GitMessageTrailer
          {
            Key = StringUtil.ToArrayFromNullTerminated(
              ((libgit2.GitMessageTrailer.__Internal*)trailer.__Instance)->key),
            Value = StringUtil.ToArrayFromNullTerminated(
              ((libgit2.GitMessageTrailer.__Internal*)trailer.__Instance)->value),
          });
          pTrailers += sizeof(libgit2.GitMessageTrailer.__Internal);
        }
        return result;
      }
    }
    finally
    {
      libgit2.message.__Internal.GitMessageTrailerArrayFree(gitMessageTrailerArray.__Instance);
    }
  }

  public byte[] PrettifyGitMessage(byte[] message, bool stripComments, byte commentChar)
  {
    if (!message.Contains((byte)0))
    {
      message = [.. message, (byte)0];
    }
    using var pinnedBuffer = new PinnedBuffer(message);
    var res = libgit2.message.GitMessagePrettify(
      out var result, pinnedBuffer.Pointer, stripComments ? 1 : 0, (sbyte)commentChar);
    CheckLibgit2.Check(res, "Unable to prettify message");
    using var gitBufDisposer = result.GetDisposer();
    return StringUtil.ToArray(result);
  }

  public GitMergeFileResult MergeFiles(GitMergeFileInput ancestor, 
    GitMergeFileInput ours, GitMergeFileInput theirs, GitMergeFileOptions? options = null)
  {
    using DisposableCollection disposable = new();
    using var nativeOptions = options?.ToNative();

    using var nativeAncestor = ancestor.ToNative(disposable);
    using var nativeOurs = ours.ToNative(disposable);
    using var nativeTheirs = theirs.ToNative(disposable);

    var res = libgit2.merge.GitMergeFile(
      out var result, nativeAncestor, nativeOurs, nativeTheirs, nativeOptions);
    CheckLibgit2.Check(res, "Unable to merge files");
    using (result)
    {
      return result.ToManaged(disposable);
    }
  }

  public IGitPathspec NewGitPathspec(IReadOnlyCollection<string> pathspecs)
  {
    using var strArray = new GitStrArrayImpl(pathspecs);
    var res = libgit2.pathspec.GitPathspecNew(out var pathspec, strArray.NativeStrArray);
    CheckLibgit2.Check(res, "Unable to create pathspec");
    return new GitPathspec(pathspec);
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.global.GitLibgit2Shutdown();
      _disposedValue = true;
    }
  }

  ~Libgit2()
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
