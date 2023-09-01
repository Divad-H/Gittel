using TypeGen.Core.TypeAnnotations;

namespace Gittel.Controllers;

public enum RequestType
{
  FunctionCall,
  Subscription,
  Unsubscription,
}


[ExportTsInterface]
public class RequestDto
{
  public required string Controller { get; init; }
  public required string Function { get; init; }
  public required string RequestId { get; init; }
  public required RequestType RequestType { get; init; }
  public string? Data { get; init; }
}

[ExportTsInterface]
public record ResponseDto
{
  public required string RequestId { get; init; }
  public required bool Success { get; init; }
  public string? Data { get; init; }
}
