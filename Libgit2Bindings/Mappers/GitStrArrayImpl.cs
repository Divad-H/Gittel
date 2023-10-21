using System.Runtime.InteropServices;
using System.Text;

namespace Libgit2Bindings.Mappers;

internal class GitStrArrayImpl : IDisposable
{
  private readonly List<GCHandle> _gcHandles = new();

  public libgit2.GitStrarray NativeStrArray { get; }

  public GitStrArrayImpl(IReadOnlyCollection<string> managedStrings)
  {
    var nativeStrings = new IntPtr[managedStrings.Count];
    var nativeStringsHandle = GCHandle.Alloc(nativeStrings, GCHandleType.Pinned);
    _gcHandles.Add(nativeStringsHandle);

    foreach(var (managedString, index) in managedStrings.Select((s, i) => (s, i)))
    {
      var nativeString = Encoding.UTF8.GetBytes(managedString + '\0');

      var handle = GCHandle.Alloc(nativeString, GCHandleType.Pinned);
      _gcHandles.Add(handle);
      nativeStrings[index] = handle.AddrOfPinnedObject();
    }

    NativeStrArray = new();
    unsafe
    {
      ((libgit2.GitStrarray.__Internal*)NativeStrArray.__Instance)->strings 
        = nativeStringsHandle.AddrOfPinnedObject();
    }
    NativeStrArray.Count = (UIntPtr)managedStrings.Count;
  }

  public void Dispose()
  {
    NativeStrArray.Dispose();
    foreach(var handle in _gcHandles)
    {
      handle.Free();
    }
  }
}
