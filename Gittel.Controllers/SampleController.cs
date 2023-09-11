using ApiGenerator.Attributes;
using System.Reactive.Linq;
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

[ExportTsInterface]
public record SampleResponseForObservable
{
  public required string Text { get; init; }
  public required int Number { get; init; }
}

[ExportTsInterface]
public record SampleRequestDtoForObservable1
{
  public required int Number { get; init; } 
}

[ExportTsInterface]
public record SampleRequestDtoForObservable2
{
  public required string Text { get; init; }
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

  public IObservable<SampleResultDto> SampleEvent(SampleRequestDto data)
  {
    return Observable
      .Interval(TimeSpan.FromSeconds(1))
      .Take(10)
      .Select(i => new SampleResultDto() { Text = data.Text + i });
  }

  public IObservable<SampleResponseForObservable> SampleEventWithMultipleParameters(SampleRequestDtoForObservable1 data1, SampleRequestDtoForObservable2 data2)
  {
    return Observable
      .Interval(TimeSpan.FromSeconds(1))
      .Take(10)
      .Select(i => new SampleResponseForObservable() { Text = data2.Text + i, Number = data1.Number + (int)i });
  }
}
