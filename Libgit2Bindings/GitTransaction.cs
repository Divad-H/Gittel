namespace Libgit2Bindings;

internal class GitTransaction : IGitTransaction
{
  private readonly libgit2.GitTransaction _nativeGitTransaction;

  public GitTransaction(libgit2.GitTransaction nativeGitTransaction)
  {
    _nativeGitTransaction = nativeGitTransaction;
  }

  public void Commit()
  {
    var res = libgit2.transaction.GitTransactionCommit(_nativeGitTransaction);
    CheckLibgit2.Check(res, "Unable to commit transaction");
  }

  #region IDisposable Support
  private bool _disposedValue;
  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      libgit2.transaction.GitTransactionFree(_nativeGitTransaction);
      _disposedValue = true;
    }
  }

  ~GitTransaction()
  {
    Dispose(disposing: false);
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
  #endregion
}
