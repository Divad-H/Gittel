namespace Libgit2Bindings;

public interface IGitRepository : IDisposable
{
  IGitReference GetHead();
}
