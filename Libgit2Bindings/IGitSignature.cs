namespace Libgit2Bindings;

public interface IGitSignature : IDisposable
{
  public string Name { get; }
  public string Email { get; }
  public DateTimeOffset When { get; }
}
