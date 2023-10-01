using Libgit2Bindings.Test.Helpers;

namespace Libgit2Bindings.Test
{
  public class GitRepositoryTest
  {
    [Fact]
    public void CanCreateRepository()
    {
      using var libgit2 = new Libgit2();

      using var tempDirectory = new TemporaryDirectory();
      using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

      var directoryPath = Path.GetFullPath(Path.Combine(tempDirectory.DirectoryPath, ".git"));
      var repoPath = Path.GetFullPath(repo.GetPath()).TrimEnd('/', '\\');
      Assert.Equal(directoryPath, repoPath);
    }

    [Fact]
    public void CanCreateBareRepository()
    {
      using var libgit2 = new Libgit2();

      using var tempDirectory = new TemporaryDirectory();
      using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, true);

      var directoryPath = Path.GetFullPath(tempDirectory.DirectoryPath).TrimEnd('/', '\\');
      var repoPath = Path.GetFullPath(repo.GetPath()).TrimEnd('/', '\\');

      Assert.Equal(directoryPath, repoPath);
    }

    [Fact]
    public void CanCheckoutSimpleRepositoryHead()
    {
      using var libgit2 = new Libgit2();

      using var repo = libgit2.OpenRepository(@"G:\Projects\test-repo-worktree");

      CheckoutOptions checkoutOptions = new()
      {
        Strategy = CheckoutStrategy.Force
      };
      repo.CheckoutHead(checkoutOptions);

      int sieben = 9;
    }

    [Fact]
    public void CanGetRepositoryConfig()
    {
      using var libgit2 = new Libgit2();

      using var tempDirectory = new TemporaryDirectory();
      using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

      using var config = repo.GetConfig();

      Assert.NotNull(config);
    }
  }
}