using Libgit2Bindings.Util;

namespace Libgit2Bindings;

internal static class CheckLibgit2
{
  public static void Check(int result, string messageFormat, params object[] args)
  {
    if (messageFormat is null)
    {
      throw new ArgumentNullException(nameof(messageFormat));
    }

    if (result == 0)
    { 
      return; 
    }

    using var error = libgit2.errors.GitErrorLast();

    var message = args.Any() ? string.Format(messageFormat, args) : messageFormat;

    if (error is not null)
    {
      message = string.Format("{0} [{1}] - {2}", message, result, StringUtil.ToString(error));
    }
    else
    {
      message = string.Format("{0} [{1}] - Unknown Error", message, result);
    }

    throw new Libgit2Exception(message);
  } 
}
