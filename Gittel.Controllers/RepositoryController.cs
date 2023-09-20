using ApiGenerator.Attributes;
using Libgit2Bindings;

namespace Gittel.Controllers;

internal record RepositoryDto
{
  public required string Path { get; init; }
  public string? WorkDir { get; init; }
}

internal record DiscoverRepositoryDto
{
  public required string BasePath { get; init; }
}

[GenerateController]
internal class RepositoryController
{
  public ILibgit2 _libgit2 { get; init; }

  public RepositoryController(ILibgit2 libgit2)
  {
    _libgit2 = libgit2;
  }

  public Task<RepositoryDto> DiscoverRepository(DiscoverRepositoryDto data, CancellationToken ct)
  {
    var gitDir = _libgit2.DiscoverRepository(data.BasePath, false, Array.Empty<string>());
    using var repo = _libgit2.OpenRepository(gitDir);

    return Task.FromResult<RepositoryDto>(new() { 
      Path = gitDir,
      WorkDir = repo.GetWorkdir()
    });
  }
}
