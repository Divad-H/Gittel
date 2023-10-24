using Libgit2Bindings.Mappers;
using System.Runtime.InteropServices;

namespace Libgit2Bindings.Callbacks;

internal class GitConfigForeachCallback : IDisposable
{
  private readonly GCHandle _gcHandle;

  private readonly List<GitConfigEntry> _entries = new();

  public GitConfigForeachCallback()
  {
    _gcHandle = GCHandle.Alloc(this);
  }

  public IntPtr Payload => GCHandle.ToIntPtr(_gcHandle);
  public IReadOnlyCollection<GitConfigEntry> GetEntries() => _entries;

  public static int GitConfigForeachCb(IntPtr entry, IntPtr payload)
  {
    GCHandle gcHandle = GCHandle.FromIntPtr(payload);
    using var entryStruct = libgit2.GitConfigEntry.__CreateInstance(entry);
    var callbacks = (GitConfigForeachCallback)gcHandle.Target!;
    callbacks._entries.Add(GitConfigEntryMapper.FromNative(entryStruct));
    return (int)libgit2.GitErrorCode.GIT_OK;
  }

  public void Dispose()
  {
    _gcHandle.Free();
  }
}
