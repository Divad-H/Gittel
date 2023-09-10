using Microsoft.Web.WebView2.Core;
using System.Reactive.Linq;
using Windows.Foundation;

namespace ApiGenerator.WebView2;

public class WebView2Messaging : IMessaging
{
  private readonly Microsoft.UI.Xaml.Controls.WebView2 _webView2;

  public WebView2Messaging(Microsoft.UI.Xaml.Controls.WebView2 webView2)
  {
    _webView2 = webView2;
  }

  public IObservable<string> MessageReceivedObservable =>
      Observable.FromEventPattern<
          TypedEventHandler<Microsoft.UI.Xaml.Controls.WebView2, CoreWebView2WebMessageReceivedEventArgs>,
          CoreWebView2WebMessageReceivedEventArgs
      >(
          handler => _webView2.WebMessageReceived += handler,
          handler => _webView2.WebMessageReceived -= handler
      )
      .Select(evt => evt.EventArgs.WebMessageAsJson);


  public void PostMessage(string message)
  {
    if (_webView2.DispatcherQueue is null || _webView2.DispatcherQueue.HasThreadAccess)
    {
      _webView2.CoreWebView2.PostWebMessageAsJson(message);
    }
    else
    {
      _webView2.DispatcherQueue.TryEnqueue(() =>
      {
        _webView2.CoreWebView2.PostWebMessageAsJson(message);
      });
    }
  }
}
