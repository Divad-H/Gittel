using System.IO.Compression;

namespace Libgit2Bindings.Util;

public static class GitDiffBinaryInflate
{
  /// <summary>
  /// Inflate the whole deflated data of a GitDiffBinaryFile.
  /// </summary>
  /// <remarks>
  /// The <see cref="GitDiffBinaryFile"/> only contains binary data if the diff was called
  /// with <see cref="GitDiffOptionFlags.ShowBinary"/>
  /// </remarks>
  /// <param name="binary">The binary file</param>
  /// <returns>A byte array with the inflated raw data.</returns>
  public static unsafe byte[] Inflate(this GitDiffBinaryFile binary)
  {
    if (binary.DeflatedData.Length <= 2)
    {
      return [];
    }
    byte[] inflatedData = new byte[binary.InflatedLength];
    fixed (byte* ptr = &binary.DeflatedData[2])
    {
      using var stream = new UnmanagedMemoryStream(ptr, binary.DeflatedData.Length - 2);
      using var deflateStream = new DeflateStream(stream, CompressionMode.Decompress);
      deflateStream.Read(new Span<byte>(inflatedData));
      return inflatedData;
    }
  }
}
