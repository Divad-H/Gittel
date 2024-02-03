using Libgit2Bindings.Util;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Libgit2Bindings.Callbacks;

internal sealed class GitRebaseCallbackImpl : IDisposable
{
  private readonly GCHandle _gcHandle;

  private readonly GitRebaseCommitCreateCallback? _commitCreate;

  public GitRebaseCallbackImpl(
    GitRebaseCommitCreateCallback? commitCreate)
  {
    _commitCreate = commitCreate;

    _gcHandle = GCHandle.Alloc(this);
  }

  public IntPtr Payload => GCHandle.ToIntPtr(_gcHandle);

  [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
  public static int GitRebaseCommitCreateCb(
    IntPtr outCommitId, IntPtr author, IntPtr committer, IntPtr messageEncoding, IntPtr message,
    IntPtr tree, nuint parentCount, IntPtr parents, IntPtr payload)
  {
    Func<GitOperationContinuationWithPassthrough> func = () =>
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(payload);
      var callbacks = (GitRebaseCallbackImpl)gcHandle.Target!;

      using var nAuthor = libgit2.GitSignature.__CreateInstance(author);
      using var mAuthor = new GitSignature(nAuthor, false);
      using var nCommitter = libgit2.GitSignature.__CreateInstance(committer);
      using var mCommitter = new GitSignature(nCommitter, false);

      var encoding = Encoding.UTF8;
      string? encodingStr = null;
      if (messageEncoding != IntPtr.Zero)
      {
        try
        {
          encodingStr = CppSharp.Runtime.MarshalUtil.GetString(encoding, messageEncoding);
          encoding = Encoding.GetEncoding(encodingStr);
        }
        catch (Exception) { }
      }

      var strMessage = string.Empty;
      if (message != IntPtr.Zero)
      {
        strMessage = CppSharp.Runtime.MarshalUtil.GetString(encoding, message);
      }

      var nTree = libgit2.GitTree.__CreateInstance(tree);
      using var mTree = new GitTree(nTree, false);

      ReadOnlySpan<IntPtr> parentPtrs;
      unsafe
      {
        parentPtrs = new ReadOnlySpan<IntPtr>(parents.ToPointer(), (int)parentCount);
      }
      List<IGitCommit> commits = new((int)parentCount);
      foreach (var parent in parentPtrs)
      {
        var nParent = libgit2.GitCommit.__CreateInstance(parent);
        commits.Add(new GitCommit(nParent, false));
      }

      var res = callbacks._commitCreate!(out var commitOid, mAuthor, mCommitter, 
        strMessage, mTree, commits);

      if (res != GitOperationContinuationWithPassthrough.Continue)
      {
        return res;
      }

      if (commitOid is null)
      {
        throw new InvalidOperationException("Commit oid must not be null.");
      }
      
      using var nativeOid = libgit2.GitOid.__CreateInstance(outCommitId);
      nativeOid.Id = commitOid.Id;

      return res;
    };
    
    return func.ExecuteInTryCatch(nameof(GitRebaseCommitCreateCb));
  }

  public void Dispose()
  {
    _gcHandle.Free();
  }
}
