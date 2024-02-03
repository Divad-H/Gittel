namespace Libgit2Bindings.Util;

internal static class HandleCallbackException
{
  public static int ExecuteInTryCatch(this Func<int> action, string callbackName)
  {
    int res = (int)libgit2.GitErrorCode.GIT_EUSER;
    
    try
    {
      res = action();

      if (res < 0)
      {
        libgit2.errors.GitErrorSetStr(res, $"{callbackName} failed.");
      }

      return res;
    }
    catch (Exception ex)
    {
      if (ex.Message is not null)
      {
        libgit2.errors.GitErrorSetStr(res, $"{callbackName} failed by exception.\n{ex.Message}");
      }
      else
      {
        libgit2.errors.GitErrorSetStr(res, $"{callbackName} failed by exception.");
      }

      return res;
    }
  }

  public static int ExecuteInTryCatch(this Func<GitOperationContinuation> action, string callbackName)
  {
    Func<int> func = () =>
    {
      var res = action();
      return (int)(res == GitOperationContinuation.Continue ? libgit2.GitErrorCode.GIT_OK : libgit2.GitErrorCode.GIT_EUSER);
    };

    return func.ExecuteInTryCatch(callbackName);
  }

  public static int ExecuteInTryCatch(this Func<GitOperationContinuationWithPassthrough> action, string callbackName)
  {
    Func<int> func = () =>
    {
      var res = action();
      if (res == GitOperationContinuationWithPassthrough.Continue)
        return (int)libgit2.GitErrorCode.GIT_OK;
      if (res == GitOperationContinuationWithPassthrough.Passthrough)
        return (int)libgit2.GitErrorCode.GIT_PASSTHROUGH;
      return (int)libgit2.GitErrorCode.GIT_EUSER;
    };

    return func.ExecuteInTryCatch(callbackName);
  }
}
