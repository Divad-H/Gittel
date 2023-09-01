using ApiGenerator;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;

namespace Gittel.Controllers
{

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
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      };

      _disposables.Add(
        messaging.MessageReceivedObservable
          .SelectMany(message =>
          {
            try
            {
              var deserialized = JsonSerializer.Deserialize<RequestDto>(message, serializerOptions);
              if (deserialized is null)
              {
                return Observable.Return(JsonSerializer.Serialize(new ResponseDto()
                {
                  RequestId = "",
                  Success = false,
                  Data = "Could not parse request data."
                }));
              }

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
              }).Select(res => JsonSerializer.Serialize(new ResponseDto()
              {
                RequestId = deserialized.RequestId,
                Success = true,
                Data = res
              }, serializerOptions))
              .Catch((Exception err) => Observable
                .Return(JsonSerializer.Serialize(new ResponseDto()
                {
                  RequestId = deserialized.RequestId,
                  Success = false,
                  Data = err.Message
                }))
                .Catch(Observable.Empty<string>())
              );
            }
            catch (Exception ex)
            {
              return Observable
                .Return(JsonSerializer.Serialize(new ResponseDto()
                {
                  RequestId = "",
                  Success = false,
                  Data = ex.Message
                }))
                .Catch(Observable.Empty<string>());
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
