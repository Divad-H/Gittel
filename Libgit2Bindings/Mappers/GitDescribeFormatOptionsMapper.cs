namespace Libgit2Bindings.Mappers;

internal static class GitDescribeFormatOptionsMapper
{
  public static libgit2.GitDescribeFormatOptions ToNative(this GitDescribeFormatOptions managedOptions)
  {
    return new()
    {
      Version = (UInt32)libgit2.GitDescribeFormatOptionsVersion.GIT_DESCRIBE_FORMAT_OPTIONS_VERSION,
      AbbreviatedSize = managedOptions.AbbreviatedSize,
      AlwaysUseLongFormat = managedOptions.AlwaysUseLongFormat ? 1 : 0,
      DirtySuffix = managedOptions.DirtySuffix,
    };
  }
}
