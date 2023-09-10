using TypeGen.Core.TypeAnnotations;
using System.Collections.Immutable;

namespace ApiGenerator;

public enum RequestType
{
  FunctionCall,
  Subscription,
  Unsubscription,
}

public enum ResponseType
{
  Success,
  Error,
  Completed,
}

public class RequestDto
{
  public required string Controller { get; init; }
  public required string Function { get; init; }
  public required string RequestId { get; init; }
  public required RequestType RequestType { get; init; }
  public ImmutableArray<string> Data { get; init; }
}

public record ResponseDto
{
  public required string RequestId { get; init; }
  public required ResponseType ResponseType { get; init; }
  public string? Data { get; init; }
}
