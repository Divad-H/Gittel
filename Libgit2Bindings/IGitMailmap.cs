namespace Libgit2Bindings;

public record NameAndEmail(string Name, string Email);

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

  /// <summary>
  /// Resolve a name and email to the corresponding real name and email.
  /// </summary>
  /// <param name="name">the name to look up</param>
  /// <param name="email">the email to look up</param>
  /// <returns>The looked up name and email</returns>
  NameAndEmail Resolve(string name, string email);

  /// <summary>
  /// Resolve a signature to use real names and emails with a mailmap.
  /// </summary>
  /// <param name="signature">signature to resolve</param>
  /// <returns>The resolved signature</returns>
  IGitSignature ResolveSignature(IGitSignature signature);
}
