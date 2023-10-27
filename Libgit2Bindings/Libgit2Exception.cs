namespace Libgit2Bindings;

public class Libgit2Exception : Exception
{
  public int ErrorCode { get; }

  public Libgit2Exception(string message, int errorCode) : base(message)
  {
    ErrorCode = errorCode;
  }
}
