namespace Libgit2Bindings;

public interface ILibgit2
{
  IGitRepository GitRepositoryOpen(string path);
}
