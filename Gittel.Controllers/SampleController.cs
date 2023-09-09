using ApiGenerator.Attributes;
using TypeGen.Core.TypeAnnotations;

namespace Gittel.Controllers;

public record SampleResultDto
{
  public required string Text { get; init; }
}

[ExportTsInterface]
public record SampleRequestDto
{
  public required string Text { get; init; }
}

[ExportTsInterface]
public record SampleRequestDto2
{
  public required int Number { get; init; }
}

[GenerateController]
public class SampleController
{
  public Task<SampleResultDto> SampleFunction(SampleRequestDto data, CancellationToken ct)
  {
    return Task.FromResult<SampleResultDto>(new() { Text = data.Text });
  }

  public Task ReturnVoid(SampleRequestDto data1, SampleRequestDto2 data2, CancellationToken ct)
  {
    return Task.CompletedTask;
  }
}
