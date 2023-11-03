namespace Libgit2Bindings;

/// <summary>
/// Structure that represents a blame hunk.
/// </summary>
public record GitBlameHunk : IDisposable
{
  /// <summary>
  /// The number of lines in the hunk.
  /// </summary>
  public UInt64 LinesInHunk { get; init; }

  /// <summary>
  /// The OID of the commit where this line was last changed.
  /// </summary>
  public required GitOid FinalCommitId { get; init; }

  /// <summary>
  /// The 1-based line number where this hunk begins, in the final version of the file.
  /// </summary>
  public UInt64 FinalStartLineNumber { get; init; }

  /// <summary>
  /// The author of <see cref="FinalCommitId"/>. 
  /// If <see cref="GitBlameFlags.UseMailmap"/> has been specified, 
  /// it will contain the canonical real name and email address.
  /// </summary>
  public IGitSignature? FinalSignature { get; init; }

  /// <summary>
  /// The OID of the commit where this hunk was found. 
  /// This will usually be the same as <see cref="FinalCommitId"/>, except when 
  /// <see cref="GitBlameFlags.TrackCopiesAnyCommitCopies"/> has been specified.
  /// </summary>
  public required GitOid OriginalCommitId { get; init; }

  /// <summary>
  /// The path to the file where this hunk originated, as of the commit specified 
  /// by <see cref="OriginalCommitId"/>.
  /// </summary>
  public string? OriginalPath { get; init; }

  /// <summary>
  /// The 1-based line number where this hunk begins in the file named by <see cref="OriginalPath"/> 
  /// in the commit specified by <see cref="OriginalCommitId"/>.
  /// </summary>
  public UInt64 OriginalStartLineNumber { get; init; }

  /// <summary>
  /// The author of <see cref="OriginalCommitId"/>. If <see cref="GitBlameFlags.UseMailmap"/> 
  /// has been specified, it will contain the canonical real name and email address.
  /// </summary>
  public IGitSignature? OriginalSignature { get; init; }

  /// <summary>
  /// iff the hunk has been tracked to a boundary commit (the root, or the commit
  /// specified in <see cref="GitBlameOptions.OldestCommit"/>)
  /// </summary>
  public bool Boundary { get; init; }

  #region IDisposable Support
  private bool disposedValue = false;
  protected virtual void Dispose(bool disposing)
  {
    if (!disposedValue)
    {
      FinalSignature?.Dispose();
      OriginalSignature?.Dispose();
      disposedValue = true;
    }
  }

  ~GitBlameHunk()
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
