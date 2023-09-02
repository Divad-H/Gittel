using ApiGenerator;
using ApiGenerator.Attributes;
using TypeGen.Core.TypeAnnotations;

namespace Gittel.Controllers;

[ExportTsInterface]
public record SampleResultDto
{
  public required string Text { get; init; }
}

[ExportTsInterface]
public record SampleRequestDto
{
  public required string Text { get; init; }
}

[GenerateController]
public class SampleController : IController
{
  public Task<SampleResultDto> SampleFunction(SampleRequestDto data, CancellationToken ct)
  {
    return Task.FromResult<SampleResultDto>(new() { Text = data.Text });
  }
}
