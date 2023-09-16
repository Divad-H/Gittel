namespace ApiGenerator;

public enum ResponseType
{
  Success,
  Error,
  Completed,
}

public record ResponseDto
{
  public required string RequestId { get; init; }
  public required ResponseType ResponseType { get; init; }
  public string? Data { get; init; }
}
