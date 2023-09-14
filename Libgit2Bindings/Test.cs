using libgit2;

namespace Libgit2Bindings
{
  public class Test
  {
    public unsafe static void Foo()
    {
      global.GitLibgit2Init();

      IntPtr repoPtr;

      var res = repository.__Internal.GitRepositoryOpen((IntPtr)(&repoPtr), @"G:\Projects\test-repo");
      GitRepository gitRepository = GitRepository.__CreateInstance(repoPtr);

      if (res == 0)
      {
        IntPtr gitStatusListPtr;
        GitStatusOptions.__Internal statusOptions = new();
        statusOptions.version = (uint)StructVersion.GIT_STATUS_OPTIONS_VERSION;
        statusOptions.flags = (uint)GitStatusOptT.GIT_STATUS_OPT_INCLUDE_UNTRACKED;

        res = status.__Internal.GitStatusListNew((IntPtr)(&gitStatusListPtr), repoPtr, (IntPtr)(&statusOptions));

        if (res == 0)
        {
          var entryCount = status.__Internal.GitStatusListEntrycount(gitStatusListPtr);

          GitStatusEntry.__Internal* entry;
          for (uint i = 0; i < entryCount; i++)
          {
            entry = (GitStatusEntry.__Internal*)status.__Internal.GitStatusByindex(gitStatusListPtr, i);

            var statusFlags = entry->status;

            var gitDiffFile = GitDiffFile.__CreateInstance((IntPtr)(&((GitDiffDelta.__Internal*)entry->head_to_index)->new_file));
            var path = gitDiffFile?.Path;
          }

          status.__Internal.GitStatusListFree(gitStatusListPtr);
        }

        repository.__Internal.GitRepositoryFree(repoPtr);
      }

      global.GitLibgit2Shutdown();
    }

    public static void Bar()
    {
      global.GitLibgit2Init();

      var res = repository.GitRepositoryOpen(out var repo, @"G:\Projects\test-repo");
      if (res == 0)
      {
        using var statusOptions = new GitStatusOptions();
        statusOptions.Version = (uint)StructVersion.GIT_STATUS_OPTIONS_VERSION;
        statusOptions.Flags = (uint)GitStatusOptT.GIT_STATUS_OPT_INCLUDE_UNTRACKED;

        res = status.GitStatusListNew(out var gitStatusList, repo, statusOptions);
        if (res == 0)
        {
          var entryCount = status.GitStatusListEntrycount(gitStatusList);

          for (uint i = 0; i < entryCount; i++)
          {
            var entry = status.GitStatusByindex(gitStatusList, i);

            var path = entry.HeadToIndex.OldFile?.Path;

          }

          status.GitStatusListFree(gitStatusList);
        }

        repository.GitRepositoryFree(repo);
      }

      global.GitLibgit2Shutdown();
    }
  }
}
