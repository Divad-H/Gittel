namespace Libgit2Bindings;

public static class Factory
{
  public static ILibgit2 CreateLibgit2()
  {
    return new Libgit2();
  }
}
