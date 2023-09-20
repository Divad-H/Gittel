using libgit2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Libgit2Bindings;

[Flags]
public enum DiffFlags
{
  /// <summary>
  /// file(s) treated as binary data
  /// </summary>
  Binary = 1 << 0,
  /// <summary>
  /// file(s) treated as text data
  /// </summary>
  NotBinary = 1 << 1,
  /// <summary>
  /// `id` value is known correct
  /// </summary>
  ValidId = 1 << 2,
  /// <summary>
  /// file exists at this side of the delta
  /// </summary>
  Exists = 1 << 3,
  /// <summary>
  /// file size value is known correct
  /// </summary>
  ValidSize = 1 << 4
}

/// <summary>
/// Description of one side of a delta.
/// <para>
/// Although this is called a "file", it could represent a file, a symbolic
/// link, a submodule commit id, or even a tree (although that only if you
/// are tracking type changes or ignored/untracked directories).
/// </para>
/// </summary>
public record class DiffFile
{
  /// <summary>
  /// The `git_oid` of the item.  If the entry represents an
	/// absent side of a diff(e.g.the `OldFile` of a `DeltaAdded` delta),
	/// then the oid will be zeroes.
  /// </summary>
  public required GitOid Oid { get; init; }
  /// <summary>
  /// The path to the entry relative to the working directory of the repository.
  /// </summary>
  public required string Path { get; init; }
  /// <summary>
  /// The size of the entry in bytes.
  /// </summary>
  public required UInt64 Size { get; init; }
  /// <summary>
  /// A combination of the <see cref="DiffFlags"/> types
  /// </summary>
  public required DiffFlags Flags { get; init; }
}
