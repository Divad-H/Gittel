using libgit2;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Libgit2Bindings;

public interface IGitConfig : IDisposable
{
  /// <summary>
  /// Set the value of a boolean config variable in the config file with the highest level (usually the local one).
  /// </summary>
  /// <param name="name">the variable's name</param>
  /// <param name="value">the value to store</param>
  void SetBool(string name, bool value);

  /// <summary>
  /// Get the value of a boolean config variable.
  /// </summary>
  /// <remarks>
  /// All config files will be looked into, in the order of their defined level.
  /// A higher level means a higher priority. 
  /// The first occurrence of the variable will be returned here.
  /// </remarks>
  /// <param name="name">the variable's name</param>
  /// <returns>The bool value</returns>
  bool GetBool(string name);

  /// <summary>
  /// Set the value of an integer config variable in the config file with the highest level (usually the local one).
  /// </summary>
  /// <param name="name">the variable's name</param>
  /// <param name="value">integer value for the variable</param>
  void SetInt32(string name, int value);

  /// <summary>
  /// Get the value of an integer config variable.
  /// </summary>
  /// <remarks>
  /// All config files will be looked into, in the order of their defined level.
  /// A higher level means a higher priority. 
  /// The first occurrence of the variable will be returned here.</remarks>
  /// <param name="name">the variable's name</param>
  /// <returns>The variables value</returns>
  int GetInt32(string name);

  /// <summary>
  /// Set the value of a long integer config variable in the config file with the highest level (usually the local one).
  /// </summary>
  /// <param name="name">the variable's name</param>
  /// <param name="value">long integer value for the variable</param>
  void SetInt64(string name, Int64 value);

  /// <summary>
  /// Get the value of a long integer config variable.
  /// </summary>
  /// <remarks>
  /// All config files will be looked into, in the order of their defined level.
  /// A higher level means a higher priority. 
  /// The first occurrence of the variable will be returned here.</remarks>
  /// <param name="name">the variable's name</param>
  /// <returns>The variables value</returns>
  Int64 GetInt64(string name);

  /// <summary>
  /// Set a multivar in the local config file.
  /// </summary>
  /// <param name="name">the variable's name</param>
  /// <param name="regexp">a regular expression to indicate which values to replace</param>
  /// <param name="value">the new value.</param>
  void SetMultiVar(string name, string regexp, string value);

  /// <summary>
  /// Get each value of a multivar
  /// </summary>
  /// <param name="name">the variable's name</param>
  /// <param name="regexp">regular expression to filter which variables we're interested in.
  /// Use <see cref="null"/> to indicate all</param>
  /// <returns>The config enumeration</returns>
  IEnumerable<GitConfigEntry> GetMultiVarEntries(string name, string? regexp);

  /// <summary>
  /// Set the value of a string config variable in the config file with the highest level (usually the local one).
  /// </summary>
  /// <param name="name">the variable's name</param>
  /// <param name="value">string value for the variable</param>
  void SetString(string name, string value);

  /// <summary>
  /// Get the value of a string config variable.
  /// </summary>
  /// <remarks>
  /// All config files will be looked into, in the order of their defined level.
  /// A higher level means a higher priority. 
  /// The first occurrence of the variable will be returned here.</remarks>
  /// <param name="name">the variable's name</param>
  /// <returns>The variables value</returns>
  string GetString(string name);

  /// <summary>
  /// Get the value of a path config variable.
  /// </summary>
  /// <param name="name">the variable's name</param>
  /// <returns>The variables value</returns>
  string GetPath(string name);

  /// <summary>
  /// Get the <see cref="GitConfigEntry"/> of a config variable.
  /// </summary>
  /// <param name="name">the variable's name</param>
  /// <returns>The variable git config entry</returns>
  GitConfigEntry GetEntry(string name);

  /// <summary>
  /// Delete a config variable from the config file with the highest level (usually the local one).
  /// </summary>
  /// <param name="name">the variable to delete</param>
  void DeleteEntry(string name);

  /// <summary>
  /// Deletes one or several entries from a multivar in the local config file.
  /// </summary>
  /// <param name="name">the variable's name</param>
  /// <param name="regexp">a regular expression to indicate which values to delete</param>
  void DeleteMultiVar(string name, string regexp);

  /// <summary>
  /// Create a snapshot of the configuration
  /// </summary>
  /// <remarks>
  /// Create a snapshot of the current state of a configuration, 
  /// which allows you to look into a consistent view of the configuration for 
  /// looking up complex values (e.g. a remote, submodule).</remarks>
  /// <returns>the snapshot config object</returns>
  IGitConfig Snapshot();

  /// <summary>
  /// Lock the backend with the highest priority
  /// </summary>
  /// <remarks>
  /// <para>
  /// Locking disallows anybody else from writing to that backend.
  /// Any updates made after locking will not be visible to a reader until the file is unlocked.
  /// </para>
  /// <para>
  /// You can apply the changes by calling <see cref="IGitTransaction.Commit"/> before disposing the transaction. 
  /// Either of these actions will unlock the config.</para>
  /// </remarks>
  /// <returns>the resulting transaction, use this to commit or undo the changes</returns>
  IGitTransaction Lock();

  /// <summary>
  /// Add an on-disk config file instance to an existing config object.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The on-disk file pointed at by path will be opened and parsed; 
  /// it's expected to be a native Git config file following the default Git config syntax (see man git-config).
  /// </para>
  /// <para>
  /// If the file does not exist, the file will still be added and it will be created the first time 
  /// we write to it.
  /// </para>
  /// <para>Note that the configuration object will free the file automatically.</para>
  /// <para>
  /// Further queries on this config object will access each of the config file instances in order 
  /// (instances with a higher priority level will be accessed first).
  /// </para>
  /// </remarks>
  /// <param name="path">path to the configuration file to add</param>
  /// <param name="level">the priority level of the backend</param>
  /// <param name="repository">optional repository to allow parsing of conditional includes</param>
  /// <param name="force">replace config file at the given priority level</param>
  void AddFileOndisk(string path, GitConfigLevel level, IGitRepository? repository, bool force);

  /// <summary>
  /// Build a single-level focused config object from a multi-level one.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The returned config object can be used to perform get/set/delete operations on a single specific level.
  /// </para>
  /// <para>
  /// Getting several times the same level from the same parent multi-level config will return different 
  /// config instances, but containing the same config_file instance.
  /// </para>
  /// </remarks>
  /// <param name="level">Configuration level to search for</param>
  /// <returns>The created <see cref="IGitConfig"/> instance</returns>
  IGitConfig OpenLevel(GitConfigLevel level);

  /// <summary>
  /// Get all config entries
  /// </summary>
  /// <remarks>
  /// The optional regular expression is applied case-sensitively on the normalized form of the 
  /// variable name: the section and variable parts are lower-cased. The subsection is left unchanged.
  /// </remarks>
  /// <param name="regexp">optional regular expression to match against config names</param>
  /// <returns>the configuration entries</returns>
  IEnumerable<GitConfigEntry> GetEntries(string? regexp = null);
}

public enum GitConfigLevel
{
  /// <summary>System-wide on Windows, for compatibility with portable git</summary>
  ProgramData,
  /// <summary>System-wide configuration file; /etc/gitconfig on Linux systems</summary>
  System,
  /// <summary>XDG compatible configuration file; typically ~/.config/git/config</summary>
  Xdg,
  /// <summary>
  /// <para>User-specific configuration file (also called Global configuration</para>
  /// <para>file); typically ~/.gitconfig</para>
  /// </summary>
  Global,
  /// <summary>
  /// <para>Repository specific configuration file; $WORK_DIR/.git/config on</para>
  /// <para>non-bare repos</para>
  /// </summary>
  Local,
  /// <summary>Application specific configuration file; freely defined by applications</summary>
  App,
  /// <summary>
  /// <para>Represents the highest level available config file (i.e. the most</para>
  /// <para>specific config file available that actually is loaded)</para>
  /// </summary>
  Highest
}

public record GitConfigEntry
{
  /// <summary>
  /// Name of the configuration entry (normalized)
  /// </summary>
  public required string Name { get; init; }
  /// <summary>
  /// Literal (string) value of the entry
  /// </summary>
  public required string Value { get; init; }
  /// <summary>
  /// The type of backend that this entry exists in (eg, "file")
  /// </summary>
  public required string BackendType { get; init; }
  /// <summary>
  /// The path to the origin of this entry. For config files, this is the path to the file.
  /// </summary>
  public required string OriginPath { get; init; }
  /// <summary>
  /// Depth of includes where this variable was found 
  /// </summary>
  public uint IncludeDepth { get; init; }
  /// <summary>
  /// Configuration level for the file this was found in 
  /// </summary>
  public GitConfigLevel Level { get; init; }
}
