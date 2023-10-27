using Libgit2Bindings.Test.Helpers;
using Libgit2Bindings.Test.TestData;

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

    [Fact]
    public void CanCreateBranch()
    {
      const string branchName = "test-branch";

      using var repo = new RepoWithOneCommit();
      using var commit = repo.Repo.LookupCommit(repo.CommitOid);

      using var branch = repo.Repo.CreateBranch(branchName, commit, false);

      Assert.NotNull(branch);
      Assert.Equal(branchName, branch.BranchName());
    }

    [Fact]
    public void CanLookupBranch()
    {
      const string branchName = "test-branch";

      using var repo = new RepoWithOneCommit();
      using var commit = repo.Repo.LookupCommit(repo.CommitOid);
      using var branch = repo.Repo.CreateBranch(branchName, commit, false);

      using var lookupBranch = repo.Repo.LookupBranch(branchName, BranchType.LocalBranch);

      Assert.NotNull(lookupBranch);
      Assert.Equal(branchName, lookupBranch.BranchName());
    }

    [Fact]
    public void CanCloneLocalRepository()
    {
      using var sourceRepo = new RepoWithOneCommit();
      using var tempDirectory = new TemporaryDirectory();

      using var clonedRepo = sourceRepo.Libgit2.Clone(
        sourceRepo.TempDirectory.DirectoryPath, tempDirectory.DirectoryPath);

      using var clonedCommit = clonedRepo.LookupCommit(sourceRepo.CommitOid);

      Assert.NotNull(clonedRepo);
      Assert.Equal(sourceRepo.CommitOid, clonedCommit.GetId());
    }

    [Fact]
    public void CanCloneRepositoryBare()
    {
      using var sourceRepo = new RepoWithOneCommit();
      using var tempDirectory = new TemporaryDirectory();

      using var clonedRepo = sourceRepo.Libgit2.Clone(
        sourceRepo.TempDirectory.DirectoryPath, tempDirectory.DirectoryPath, new CloneOptions
        {
          Bare = true,
        });

      Assert.NotNull(clonedRepo);
      Assert.True(clonedRepo.IsBare());
    }

    [Fact]
    public void CanCreateRepositoryCustomWhileCloning()
    {
      using var sourceRepo = new RepoWithOneCommit();
      using var tempDirectory = new TemporaryDirectory();

      using var clonedRepo = sourceRepo.Libgit2.Clone(
        sourceRepo.TempDirectory.DirectoryPath, tempDirectory.DirectoryPath, new CloneOptions
        {
          RepositoryCreateCallback = (out IGitRepository? repository, string path, bool bare) =>
          {
            repository = sourceRepo.Libgit2.InitRepository(path, true);
            return 0;
          }
        });

      Assert.NotNull(clonedRepo);
      Assert.True(clonedRepo.IsBare());
    }

    [Fact]
    public void CanCreateRemoteCustomWhileCloning()
    {
      const string remoteName = "my-origin";
      using var sourceRepo = new RepoWithOneCommit();
      using var tempDirectory = new TemporaryDirectory();

      using var clonedRepo = sourceRepo.Libgit2.Clone(
        sourceRepo.TempDirectory.DirectoryPath, tempDirectory.DirectoryPath, new CloneOptions
        {
          RemoteCreateCallback = (out IGitRemote? remote, IGitRepository repository, string name, string url) =>
          {
            remote = repository.CreateRemote(remoteName, url);
            return 0;
          }
        });

      Assert.NotNull(clonedRepo);
      using var remote = clonedRepo.LookupRemote(remoteName);
      Assert.NotNull(remote);
    }

    [Fact]
    public void CanCloneRepositoryWithCustonCheckoutBranch()
    {
      const string branchName = "test-branch";

      using var sourceRepo = new RepoWithOneCommit();
      using var commit = sourceRepo.Repo.LookupCommit(sourceRepo.CommitOid);
      using var branch = sourceRepo.Repo.CreateBranch(branchName, commit, false);
      using var tempDirectory = new TemporaryDirectory();

      using var clonedRepo = sourceRepo.Libgit2.Clone(
        sourceRepo.TempDirectory.DirectoryPath, tempDirectory.DirectoryPath, new CloneOptions
        {
          CheckoutBranch = branchName,
        });

      using var clonedBranch = clonedRepo.LookupBranch(branchName, BranchType.LocalBranch);

      Assert.NotNull(clonedRepo);
      Assert.NotNull(clonedBranch);
    }

    [Fact]
    public void CanCloneWithCustomCheckoutStrategy()
    {
      using var sourceRepo = new RepoWithOneCommit();
      using var tempDirectory = new TemporaryDirectory();

      using var clonedRepo = sourceRepo.Libgit2.Clone(
        sourceRepo.TempDirectory.DirectoryPath, tempDirectory.DirectoryPath, new CloneOptions
        {
          CheckoutOptions = new CheckoutOptions
          {
            Strategy = CheckoutStrategy.None,
          }
        });

      Assert.NotNull(clonedRepo);
      Assert.False(File.Exists(Path.Combine(tempDirectory.DirectoryPath, RepoWithOneCommit.Filename)));
    }

    [Fact]
    public void CanAbortCloneInNotifyCallback()
    {
      using var sourceRepo = new RepoWithOneCommit();
      using var tempDirectory = new TemporaryDirectory();

      bool caughtException = false;
      try
      {
        using var clonedRepo = sourceRepo.Libgit2.Clone(
          sourceRepo.TempDirectory.DirectoryPath, tempDirectory.DirectoryPath, new CloneOptions
          {
            CheckoutOptions = new CheckoutOptions
            {
              NotifyFlags = CheckoutNotifyFlags.Updated,
              NotifyCallback = (CheckoutNotifyFlags why, string path, DiffFile? baseline, DiffFile? target, DiffFile? workdir) =>
              {
                if (why == CheckoutNotifyFlags.Updated && path.EndsWith(RepoWithOneCommit.Filename))
                {
                  return GitOperationContinuation.Cancel;
                }
                return GitOperationContinuation.Continue;
              }
            }
          });
      }
      catch (Libgit2Exception ex)
      {
        Assert.Equal(-7, ex.ErrorCode);
        caughtException = true;
        return;
      }
      Assert.True(caughtException);
    }
  }
}
