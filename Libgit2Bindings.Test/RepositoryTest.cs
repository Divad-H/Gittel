using Libgit2Bindings.Test.Helpers;

namespace Libgit2Bindings.Test
{
  public class RepositoryTest
  {
    [Fact]
    public void CanCreateRepository()
    {
      var libgit2 = new Libgit2();

      using var tempDirectory = new TemporaryDirectory();
      var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

      var directoryPath = Path.GetFullPath(Path.Combine(tempDirectory.DirectoryPath, ".git"));
      var repoPath = Path.GetFullPath(repo.GetPath()).TrimEnd('/', '\\');
      Assert.Equal(directoryPath, repoPath);
    }

    [Fact]
    public void CanCreateBareRepository()
    {
      var libgit2 = new Libgit2();

      using var tempDirectory = new TemporaryDirectory();
      var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, true);

      var directoryPath = Path.GetFullPath(tempDirectory.DirectoryPath).TrimEnd('/', '\\');
      var repoPath = Path.GetFullPath(repo.GetPath()).TrimEnd('/', '\\');

      Assert.Equal(directoryPath, repoPath);
    }
  }
}
