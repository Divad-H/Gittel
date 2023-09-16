namespace Libgit2Bindings;

internal interface ILibgit2Internal : ILibgit2
{
  void GitRepositoryFree(libgit2.GitRepository repo);
}
