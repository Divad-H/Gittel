using libgit2;

namespace Libgit2Bindings;

public interface ILibgit2
{
  /// <summary>
  /// Open a git repository
  /// </summary>
  /// <param name="path">The path to the repository</param>
  /// <returns>The opened repository</returns>
  IGitRepository OpenRepository(string path);

  /// <summary>
  /// Look for a gir repository and return its path.
  /// The lookup start from the given path and walk upward parent directories if nothing has been found.
  /// The lookup ends when a repository has been found or when reaching a directory referenced in <paramref name="ceilingDirectories"/>.
  /// </summary>
  /// <param name="startPath">The base path where the lookup starts</param>
  /// <param name="acrossFilesystem">If true, then the lookup will not stop when a filesystem device change is detected while exploring parent directories.</param>
  /// <param name="ceilingDirectories">A list of absolute symbolic link free paths. The lookup will stop when any of this paths is reached. 
  /// Note that the lookup always performs on start_path no matter start_path appears in ceiling_dirs</param>
  /// <returns>The path of the found repository</returns>
  string DiscoverRepository(string startPath, bool acrossFilesystem, string[] ceilingDirectories);

  /// <summary>
  /// Create a new git repository in the given folder
  /// </summary>
  /// <param name="path">The path to the repository</param>
  /// <param name="isBare">if true, a Git repository without a working directory is created at the pointed path.
  /// If false, provided path will be considered as the working directory into which the .git directory will be created.</param>
  /// <returns>The repository which will be created or reinitialized</returns>
  IGitRepository InitRepository(string path, bool isBare);

  /// <summary>
  /// Create a new action signature.
  /// </summary>
  /// <remarks>
  /// Note: angle brackets ('&lt;' and '&gt;') characters are not allowed to be used in either the name or the email parameter.</remarks>
  /// <param name="name">name of the person</param>
  /// <param name="email">email of the person</param>
  /// <param name="when">time when the action happened</param>
  /// <returns>new signature</returns>
  IGitSignature CreateGitSignature(string name, string email, DateTimeOffset when);

  /// <summary>
  /// Create a new signature by parsing the given buffer, which is
  /// expected to be in the format "Real Name &lt;email&gt; timestamp tzoffset",
  /// where `timestamp` is the number of seconds since the Unix epoch and
  /// `tzoffset` is the timezone offset in `hhmm` format(note the lack
  /// of a colon separator).
  /// </summary>
  /// <param name="signature">signature string</param>
  /// <returns>new signature</returns>
  IGitSignature CreateGitSignature(string signature);

  /// <summary>
  /// Locate the path to the global configuration file
  /// </summary>
  /// <returns>Path to the global git config file</returns>
  string FindGlobalConfig();
}
