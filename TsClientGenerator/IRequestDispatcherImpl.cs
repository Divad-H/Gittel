using System.Text.Json;

namespace ApiGenerator;

public interface IRequestDispatcherImpl
{
  Task<string?> DispatchRequest(RequestDto request, JsonSerializerOptions jsonSerializerOptions, CancellationToken ct);
}
