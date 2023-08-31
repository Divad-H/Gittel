using ApiGenerator;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;

namespace Gittel.Controllers
{
  internal record Request
  {
    public required string Controller { get; init; }
    public required string Function { get; init; }
    public required string RequestId { get; init; }
    public string? Data { get; init; }
  }

  internal record Response
  {
    public required string RequestId { get; init; }
    public string? Data { get; init; }
  }

  public class RequestDispatcher : IDisposable
  {
    private readonly CompositeDisposable _disposables = new ();

    private readonly IMessaging _messaging;
    private readonly SampleController _sampleController;
    public RequestDispatcher(IMessaging messaging, SampleController sampleController)
    {
      _messaging = messaging;
      _sampleController = sampleController;

      var serializerOptions = new JsonSerializerOptions()
      {
        PropertyNameCaseInsensitive = true,
      };

      _disposables.Add(
        messaging.MessageReceivedObservable
          .SelectMany(message =>
          {
            try
            {
              var deserialized = JsonSerializer.Deserialize<Request>(message, serializerOptions);
              return (deserialized.Controller switch
              {
                "sample" => deserialized.Function switch
                {
                  "sampleFunction" => Observable
                    .FromAsync((ct) => sampleController
                      .SampleFunction(JsonSerializer.Deserialize<SampleRequestDto>(deserialized.Data, serializerOptions), ct))
                    .Select(res => JsonSerializer.Serialize(res, serializerOptions)),
                  _ => Observable.Throw<string>(new InvalidOperationException("Function not found."))
                },
                _ => Observable.Throw<string>(new InvalidOperationException("Controller not found."))
              }).Select(res => JsonSerializer.Serialize(new Response()
              {
                RequestId = deserialized.RequestId,
                Data = res
              }))
              .Catch(Observable.Empty<string>()); // TODO return error an log
            }
            catch (Exception ex)
            {
              // TODO return error and log
              return Observable.Empty<string>();
            }
          })
          .SelectMany(res => Observable.FromAsync(() => _messaging.PostMessage(res)))
          .Subscribe()
        );
    }

    public void Dispose()
    {
      _disposables.Dispose();
    }
  }
}
