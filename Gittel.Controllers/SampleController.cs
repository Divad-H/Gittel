using ApiGenerator;
using TypeGen.Core.TypeAnnotations;

namespace Gittel.Controllers;

[ExportTsClass]
public record SampleResultDto
{
  public required string Text { get; init; }
}

[ExportTsClass]
public record SampleRequestDto
{
  public required string Text { get; init; }
}

public class SampleController : IController
{
  public Task<SampleResultDto> SampleFunction(SampleRequestDto data, CancellationToken ct)
  {
    return Task.FromResult<SampleResultDto>(new() { Text = data.Text });
  }
}
