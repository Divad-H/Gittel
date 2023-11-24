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

  /// <summary>
  /// Locate the path to the system configuration file
  /// </summary>
  /// <returns>Path to the system git config file</returns>
  string FindSystemConfig();

  /// <summary>
  /// Locate the path to the configuration file in ProgramData
  /// </summary>
  /// <returns>Path to the git config file in ProgramData</returns>
  string FindProgramdataConfig();

  /// <summary>
  /// Locate the path to the global xdg compatible configuration file
  /// </summary>
  /// <returns>Path to the global xdg compatible git config file</returns>
  string FindXdgConfig();

  /// <summary>
  /// Allocate a new configuration object
  /// </summary>
  /// <remarks>
  /// This object is empty, so you have to add a file to it before you can do anything with it.
  /// </remarks>
  /// <returns>the new configuration</returns>
  IGitConfig NewConfig();

  /// <summary>
  /// Create a new config instance containing a single on-disk file
  /// </summary>
  /// <remarks>
  /// This method is a simple utility wrapper for the following sequence of calls: 
  /// - <see cref="NewConfig"/> - <see cref="IGitConfig.AddFileOndisk"/>
  /// </remarks>
  /// <returns>The created configuration instance</returns>
  IGitConfig OpenConfigOndisk(string path);

  /// <summary>
  /// Parse a string value as a bool.
  /// </summary>
  /// <remarks>
  /// Valid values for true are: 'true', 'yes', 'on', 1 or any number different from 0.
  /// Valid values for false are: 'false', 'no', 'off', 0
  /// </remarks>
  /// <param name="value">value to parse</param>
  /// <returns>the result of the parsing</returns>
  bool ParseConfigBool(string value);

  /// <summary>
  /// Parse a string value as an int32.
  /// </summary>
  /// <remarks>
  /// An optional value suffix of 'k', 'm', or 'g' will cause the value to be multiplied 
  /// by 1024, 1048576, or 1073741824 prior to output.
  /// </remarks>
  /// <param name="value">value to parse</param>
  /// <returns>the result of the parsing</returns>
  int ParseConfigInt32(string value);

  /// <summary>
  /// Parse a string value as an int64.
  /// </summary>
  /// <remarks>
  /// An optional value suffix of 'k', 'm', or 'g' will cause the value to be multiplied 
  /// by 1024, 1048576, or 1073741824 prior to output.
  /// </remarks>
  /// <param name="value">value to parse</param>
  /// <returns>the result of the parsing</returns>
  long ParseConfigInt64(string value);

  /// <summary>
  /// Parse a string value as a path.
  /// </summary>
  /// <remarks>
  /// <para>A leading '~' will be expanded to the global search path.</para>
  /// <para>If the value does not begin with a tilde, the input will be returned.</para>
  /// </remarks>
  /// <param name="value">value to parse</param>
  /// <returns>the result of the parsing</returns>
  string ParseConfigPath(string value);

  /// <summary>
  /// Clone a remote repository.
  /// </summary>
  /// <param name="url">the remote repository to clone</param>
  /// <param name="localPath">local directory to clone to</param>
  /// <param name="options">configuration options for the clone.</param>
  /// <returns>the resulting repository object</returns>
  IGitRepository Clone(string url, string localPath, CloneOptions? options = null);

  /// <summary>
  /// Determine if the given content is most certainly binary or not; this is the same 
  /// mechanism used by <see cref="IGitBlob.IsBinary()"/> but only looking at raw data.
  /// </summary>
  /// <param name="blobData"></param>
  /// <returns>true if the content of the blob is detected as binary; false otherwise.</returns>
  bool BlobDataIsBinary(byte[] blobData);

  /// <summary>
  /// Determine whether a branch name is valid, meaning that (when prefixed with refs/heads/)
  /// that it is a valid reference name, and that any additional branch name restrictions are 
  /// imposed (eg, it cannot start with a -).
  /// </summary>
  /// <param name="branchName">a branch name to test</param>
  /// <returns>validity of given branch name</returns>
  bool BranchNameIsValid(string branchName);
}
