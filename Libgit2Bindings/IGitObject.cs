namespace Libgit2Bindings;

public interface IGitObject : IDisposable
{
  public GitOid Id { get; }
}
