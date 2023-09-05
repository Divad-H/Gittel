using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Gittel.Main;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class MainWindow : Window
{
  public MainWindow(CommandLineOptions commandLineOptions)
  {
    this.InitializeComponent();

    this.MainWebView.Source = new(commandLineOptions.SpaUri);
  }

  public WebView2 WebView => MainWebView;
}
