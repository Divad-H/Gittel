namespace Libgit2Bindings;

public interface IOdbWritePack
{
  void Append(byte[] data, GitIndexerProgress stats);

  void Commit(GitIndexerProgress stats);
}
