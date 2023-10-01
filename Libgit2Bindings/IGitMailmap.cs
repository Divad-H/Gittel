namespace Libgit2Bindings;

public interface IGitMailmap : IDisposable
{
  /// <summary>
  /// Add a single entry to the given mailmap object. 
  /// If the entry already exists, it will be replaced with the new entry.
  /// </summary>
  /// <param name="realName">the real name to use, or null</param>
  /// <param name="realEmail">the real email to use, or null</param>
  /// <param name="replaceName">the name to replace, or null</param>
  /// <param name="replaceEmail">the email to replace</param>
  void AddEntry(string? realName, string? realEmail, string? replaceName, string replaceEmail);
}
