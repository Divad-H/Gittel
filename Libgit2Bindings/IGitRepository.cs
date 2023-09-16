namespace Libgit2Bindings;

public interface IGitRepository : IDisposable
{
  IGitReference GetHead();

  string GetPath();
}
