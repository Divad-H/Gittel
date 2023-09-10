using ApiGenerator;
using Microsoft.Extensions.DependencyInjection;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;

namespace ApiGenerator
{

  public class RequestDispatcher : IDisposable
  {
    private readonly CompositeDisposable _disposables = new();

    private readonly IMessaging _messaging;
    private readonly IRequestDispatcherImpl _dispatcherImpl;
    public RequestDispatcher(IMessaging messaging, IRequestDispatcherImpl dispatcherImpl, IServiceProvider serviceProvider)
    {
      _messaging = messaging;
      _dispatcherImpl = dispatcherImpl;

      var serializerOptions = new JsonSerializerOptions()
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      };

      Subject<string> unsubscribe = new();

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
                  ResponseType = ResponseType.Error,
                  Data = "Could not parse request data."
                }));
              }

              if (deserialized.RequestType == RequestType.Unsubscription)
              {
                unsubscribe.OnNext(deserialized.RequestId);
                return Observable.Return(JsonSerializer.Serialize(new ResponseDto()
                {
                  RequestId = deserialized.RequestId,
                  ResponseType = ResponseType.Success
                }, serializerOptions));
              }
              else
              {
                var serializedResponse = (deserialized.RequestType == RequestType.FunctionCall
                  ? Observable
                    .FromAsync(ct => dispatcherImpl.DispatchRequest(deserialized, serializerOptions, ct))
                  : Observable.Using(() => serviceProvider.CreateScope(), scope => dispatcherImpl.DispatchObservableRequest(deserialized, serializerOptions, scope.ServiceProvider))
                    .TakeUntil(unsubscribe.Where(id => id == deserialized.RequestId))
                  )
                    .Select(res => JsonSerializer.Serialize(new ResponseDto()
                    {
                      RequestId = deserialized.RequestId,
                      ResponseType = ResponseType.Success,
                      Data = res
                    }, serializerOptions));

                if (deserialized.RequestType == RequestType.Subscription)
                {
                  serializedResponse = serializedResponse
                    .Concat(Observable.Return(JsonSerializer.Serialize(new ResponseDto()
                    {
                      RequestId = deserialized.RequestId,
                      ResponseType = ResponseType.Completed,
                      Data = null
                    }, serializerOptions)));
                }

                return serializedResponse
                    .Catch((Exception err) => Observable
                      .Return(JsonSerializer.Serialize(new ResponseDto()
                      {
                        RequestId = deserialized.RequestId,
                        ResponseType = ResponseType.Error,
                        Data = err.Message
                      }, serializerOptions))
                      .Catch(Observable.Empty<string>())
                    );
              }
            }
            catch (Exception ex)
            {
              return Observable
                .Return(JsonSerializer.Serialize(new ResponseDto()
                {
                  RequestId = "",
                  ResponseType = ResponseType.Error,
                  Data = ex.Message
                }, serializerOptions))
                .Catch(Observable.Empty<string>());
            }
          })
          .Select(res =>
          {
            _messaging.PostMessage(res);
            return Unit.Default;
          })
          .Subscribe()
        );
    }

    public void Dispose()
    {
      _disposables.Dispose();
    }
  }
}
