using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Gittel.Main;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
  public MainWindow(string spaUri)
  {
    this.InitializeComponent();

    this.MainWebView.Source = new(spaUri);
  }

  public WebView2 WebView => MainWebView;
}
