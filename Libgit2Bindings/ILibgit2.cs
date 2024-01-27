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

  /// <summary>
  /// Determine if the given <see cref="GitObjectType"/> is a valid loose object type.
  /// </summary>
  /// <param name="type">object type to test.</param>
  /// <returns>true if the type represents a valid loose object type, false otherwise.</returns>
  bool GitObjectTypeIsLoose(GitObjectType type);

  /// <summary>
  /// Analyzes a buffer of raw object content and determines its validity. 
  /// Tree, commit, and tag objects will be parsed and ensured that they are valid,
  /// parseable content. (Blobs are always valid by definition.) 
  /// An error message will be set with an informative message if the object is not valid.
  /// </summary>
  /// <param name="rawContent">The contents to validate</param>
  /// <param name="type">The type of the object in the buffer</param>
  /// <returns>true, if the content is valid, false othewise</returns>
  bool GitObjectRawContentIsValid(byte[] rawContent, GitObjectType type);

  /// <summary>
  /// Directly run a diff between a blob and a buffer.
  /// </summary>
  /// <param name="oldBlob">Blob for old side of diff, or null for empty blob</param>
  /// <param name="oldAsPath">Treat old blob as if it had this filename; can be null</param>
  /// <param name="newBuffer">Raw data for new side of diff, or null for empty</param>
  /// <param name="newBufferAsPath">Treat buffer as if it had this filename; can be null</param>
  /// <param name="options">Options for diff, or null for default options</param>
  /// <param name="fileCallback">Callback for "file"; made once if there is a diff; can be null</param>
  /// <param name="binaryCallback">Callback for binary files; can be null</param>
  /// <param name="hunkCallback">Callback for each hunk in diff; can be null</param>
  /// <param name="lineCallback">Callback for each line in diff; can be null</param>
  void DiffBlobToBuffer(IGitBlob? oldBlob, string? oldAsPath, byte[]? newBuffer, 
    string? newBufferAsPath = null, GitDiffOptions? options = null,
    IGitDiff.FileCallback? fileCallback = null,
    IGitDiff.BinaryCallback? binaryCallback = null,
    IGitDiff.HunkCallback? hunkCallback = null,
    IGitDiff.LineCallback? lineCallback = null);

  /// <summary>
  /// Directly generate a patch from the difference between a blob and a buffer.
  /// </summary>
  /// <remarks>
  /// This is just like <see cref="DiffBlobToBuffer"/> except it generates a patch object for 
  /// the difference instead of directly making callbacks.
  /// </remarks>
  /// <param name="oldBlob">Blob for old side of diff, or null for empty blob</param>
  /// <param name="oldAsPath">Treat old blob as if it had this filename; can be null</param>
  /// <param name="newBuffer">Raw data for new side of diff, or null for empty</param>
  /// <param name="newBufferAsPath">Treat buffer as if it had this filename; can be null</param>
  /// <param name="options">Options for diff, or null for default options</param>
  /// <returns>The generated patch</returns>
  IGitPatch PatchFromBlobAndBuffer(IGitBlob? oldBlob, string? oldAsPath, byte[]? newBuffer, 
    string? newBufferAsPath = null, GitDiffOptions? options = null);

  /// <summary>
  /// Directly run a diff between two blobs.
  /// </summary>
  /// <remarks>
  /// Compared to a file, a blob lacks some contextual information. 
  /// As such, the <see cref="GitDiffFile"/> given to the callback will have some fake data; i.e. 
  /// <see cref="GitDiffFile.Mode"/> will be 0 and <see cref="GitDiffFile.Path"/> will be null.
  /// <para/>
  /// null is allowed for either <paramref name="oldBlob"/> or <paramref name="newBlob"/> and will be 
  /// treated as an empty blob, with the oid set to null in the <see cref="GitDiffFile"/> data. 
  /// Passing null for both blobs is a noop; no callbacks will be made at all.
  /// <para/>
  /// We do run a binary content check on the blob content and if either blob looks like binary data, 
  /// the <see cref="GitDiffDelta"/> binary attribute will be set to 1 and no call to the 
  /// <paramref name="hunkCallback"/> nor <paramref name="lineCallback"/> will be made 
  /// (unless you pass <see cref="GitDiffOptionFlags.ForceText"/> of course).
  /// </remarks>
  /// <param name="oldBlob">Blob for old side of diff, or null for empty blob</param>
  /// <param name="oldAsPath">Treat old blob as if it had this filename; can be null</param>
  /// <param name="newBlob">Blob for new side of diff, or null for empty blob</param>
  /// <param name="newBlobAsPath">Treat new blob as if it had this filename; can be null</param>
  /// <param name="options">Options for diff, or null for default options</param>
  /// <param name="fileCallback">Callback for "file"; made once if there is a diff; can be null</param>
  /// <param name="binaryCallback">Callback for binary files; can be null</param>
  /// <param name="hunkCallback">Callback for each hunk in diff; can be null</param>
  /// <param name="lineCallback">Callback for each line in diff; can be null</param>
  void DiffBlobs(IGitBlob? oldBlob, string? oldAsPath, 
    IGitBlob? newBlob, string? newBlobAsPath = null, 
    GitDiffOptions? options = null,
    IGitDiff.FileCallback? fileCallback = null,
    IGitDiff.BinaryCallback? binaryCallback = null,
    IGitDiff.HunkCallback? hunkCallback = null,
    IGitDiff.LineCallback? lineCallback = null);

  /// <summary>
  /// Directly generate a patch from the difference between two blobs.
  /// </summary>
  /// <remarks>
  /// This is just like <see cref="DiffBlobs"/> except it generates a patch object for the difference 
  /// instead of directly making callbacks.
  /// </remarks>
  /// <param name="oldBlob">Blob for old side of diff, or null for empty blob</param>
  /// <param name="oldAsPath">Treat old blob as if it had this filename; can be null</param>
  /// <param name="newBlob">Blob for new side of diff, or null for empty blob</param>
  /// <param name="newBlobAsPath">Treat new blob as if it had this filename; can be null</param>
  /// <param name="options">Options for diff, or null for default options</param>
  /// <returns>The generated patch</returns>
  IGitPatch PatchFromBlobs(IGitBlob? oldBlob, string? oldAsPath, 
    IGitBlob? newBlob, string? newBlobAsPath = null, 
    GitDiffOptions? options = null);

  /// <summary>
  /// Directly run a diff between two buffers.
  /// </summary>
  /// <remarks>
  /// Even more than with <see cref="DiffBlobs"/>, comparing two buffer lacks context, 
  /// so the <see cref="GitDiffFile"/> parameters to the callbacks will be faked a la 
  /// the rules for <see cref="DiffBlobs"/>
  /// </remarks>
  /// <param name="oldBuffer">Raw data for old side of diff, or null for empty</param>
  /// <param name="oldAsPath">Treat old as if it had this filename; can be null</param>
  /// <param name="newBuffer">Raw data for new side of diff, or null for empty</param>
  /// <param name="newAsPath">Treat new as if it had this filename; can be null</param>
  /// <param name="options">Options for diff, or null for default options</param>
  /// <param name="fileCallback">Callback for "file"; made once if there is a diff; can be null</param>
  /// <param name="binaryCallback">Callback for binary files; can be null</param>
  /// <param name="hunkCallback">Callback for each hunk in diff; can be null</param>
  /// <param name="lineCallback">Callback for each line in diff; can be null</param>
  void DiffBuffers(byte[]? oldBuffer, string? oldAsPath, byte[]? newBuffer, string? newAsPath,
    GitDiffOptions? options = null,
    IGitDiff.FileCallback? fileCallback = null,
    IGitDiff.BinaryCallback? binaryCallback = null,
    IGitDiff.HunkCallback? hunkCallback = null,
    IGitDiff.LineCallback? lineCallback = null);

  /// <summary>
  /// Directly generate a patch from the difference between two buffers.
  /// </summary>
  /// <remarks>
  /// This is just like <see cref="DiffBuffers"/> except it generates a patch object for the 
  /// difference instead of directly making callbacks.
  /// </remarks>
  /// <param name="oldBuffer">Raw data for old side of diff, or null for empty</param>
  /// <param name="oldAsPath">Treat old as if it had this filename; can be null</param>
  /// <param name="newBuffer">Raw data for new side of diff, or null for empty</param>
  /// <param name="newAsPath">Treat new as if it had this filename; can be null</param>
  /// <param name="options">Options for diff, or null for default options</param>
  /// <returns>The generated patch</returns>
  IGitPatch PatchFromBuffers(byte[]? oldBuffer, string? oldAsPath, byte[]? newBuffer, string? newAsPath,
    GitDiffOptions? options = null);

  /// <summary>
  /// Read the contents of a git patch file into a <see cref="IGitDiff"/> object.
  /// </summary>
  /// <param name="patch">The contents of a patch file</param>
  /// <returns>The diff object</returns>
  IGitDiff DiffFromPatch(byte[] patch);

  /// <summary>
  /// Look up the single character abbreviation for a delta status code.
  /// </summary>
  /// <remarks>
  /// When you run git diff --name-status it uses single letter codes in the output 
  /// such as 'A' for added, 'D' for deleted, 'M' for modified, etc. This function 
  /// converts a git_delta_t value into these letters for your own purposes. 
  /// <see cref="GitDeltaType.Untracked"/>  will return a space (i.e. ' ').
  /// </remarks>
  /// <param name="gitDelta">The <see cref="GitDeltaType"/> value to look up</param>
  /// <returns>The single character label for that code</returns>
  sbyte GetDiffStatusCharByte(GitDeltaType gitDelta);

  /// <summary>
  /// Look up the single character abbreviation for a delta status code.
  /// </summary>
  /// <remarks>
  /// When you run git diff --name-status it uses single letter codes in the output 
  /// such as 'A' for added, 'D' for deleted, 'M' for modified, etc. This function 
  /// converts a git_delta_t value into these letters for your own purposes. 
  /// <see cref="GitDeltaType.Untracked"/>  will return a space (i.e. ' ').
  /// </remarks>
  /// <param name="gitDelta">The <see cref="GitDeltaType"/> value to look up</param>
  /// <returns>The single character label for that code</returns>
  char GetDiffStatusChar(GitDeltaType gitDelta);

  /// <summary>
  /// Create a new indexer instance
  /// </summary>
  /// <param name="path">Path to the directory where the packfile should be stored</param>
  /// <param name="mode">permissions to use creating packfile or 0 for defaults</param>
  /// <param name="odb">
  /// object database from which to read base objects when fixing thin packs. Pass null 
  /// if no thin pack is expected (an error will be returned if there are bases missing)
  /// </param>
  /// <param name="options">
  /// Optional structure containing additional options. See <see cref="GitIndexerOptions"/>.
  /// </param>
  /// <returns>the indexer instance</returns>
  IGitIndexer NewGitIndexer(string path, UInt32 mode, IGitOdb? odb, GitIndexerOptions? options = null);

  /// <summary>
  /// Create a new mailmap instance containing a single mailmap file
  /// </summary>
  /// <param name="buffer">buffer to parse the mailmap from</param>
  /// <returns>the new mailmap</returns>
  IGitMailmap NewGitMailmapFromBuffer(byte[] buffer);

  /// <summary>
  /// Allocate a new mailmap object.
  /// </summary>
  /// <remarks>
  /// This object is empty, so you'll have to add a mailmap file before you can do anything with it.
  /// </remarks>
  /// <returns>the new mailmap</returns>
  IGitMailmap NewGitMailmap();

  /// <summary>
  /// Parse trailers out of a message
  /// </summary>
  /// <remarks>
  /// Trailers are key/value pairs in the last paragraph of a message, not including any patches 
  /// or conflicts that may be present.
  /// </remarks>
  /// <param name="message">The message to be parsed</param>
  /// <returns>any trailers found during parsing</returns>
  IReadOnlyList<GitMessageTrailer> ParseGitMessageTrailers(byte[] message);

  /// <summary>
  /// Clean up excess whitespace and make sure there is a trailing newline in the message.
  /// </summary>
  /// <remarks>
  /// Optionally, it can remove lines which start with the comment character.
  /// </remarks>
  /// <param name="message">The message to be prettified.</param>
  /// <param name="stripComments">true to remove comment lines, false to leave them in.</param>
  /// <param name="commentChar">
  /// Comment character. Lines starting with this character are considered to be comments and 
  /// removed if stripComments is true.
  /// </param>
  /// <returns>the cleaned up message.</returns>
  byte[] PrettifyGitMessage(byte[] message, bool stripComments, byte commentChar);

  /// <summary>
  /// Merge two files as they exist in the in-memory data structures, using the given common ancestor as 
  /// the baseline, producing a <see cref="GitMergeFileResult"/> that reflects the merge result.
  /// </summary>
  /// <remarks>
  /// Note that this function does not reference a repository and any configuration must be passed 
  /// as <see cref="MergeOptions"/>.
  /// </remarks>
  /// <param name="ancestor">The contents of the ancestor file</param>
  /// <param name="ours">The contents of the file in "our" side</param>
  /// <param name="theirs">The contents of the file in "their" side</param>
  /// <param name="options">The merge file options or null for defaults</param>
  /// <returns>The file merge result</returns>
  GitMergeFileResult MergeFiles(GitMergeFileInput ancestor,
    GitMergeFileInput ours, GitMergeFileInput theirs, GitMergeFileOptions? options = null);

  /// <summary>
  /// Compile a pathspec
  /// </summary>
  /// <param name="pathspecs">The paths to match</param>
  /// <returns>The compiled pathspec</returns>
  IGitPathspec NewGitPathspec(IReadOnlyCollection<string> pathspecs);
}
