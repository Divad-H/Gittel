namespace Libgit2Bindings;

public abstract class AbstractGitWriteStream : Stream
{
  public abstract GitOid Commit();
}
