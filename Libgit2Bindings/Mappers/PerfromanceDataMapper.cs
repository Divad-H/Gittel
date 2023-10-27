namespace Libgit2Bindings.Mappers;

internal static class PerfromanceDataMapper
{
  public static PerformanceData FromNative(libgit2.GitCheckoutPerfdata perfdata)
  {
    return new PerformanceData(perfdata.MkdirCalls, perfdata.StatCalls, perfdata.ChmodCalls);
  }

  public static PerformanceData FromNativePtr(IntPtr ptr)
  {
    using var native = libgit2.GitCheckoutPerfdata.__CreateInstance(ptr);
    return FromNative(native);
  }
}
