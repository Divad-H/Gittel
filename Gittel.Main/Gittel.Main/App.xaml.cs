using ApiGenerator;
using ApiGenerator.WebView2;
using CommandLine;
using Gittel.Controllers;
using Microsoft.UI.Xaml;
using System;

namespace Gittel.Main;

public partial class App : Application
{
  public App()
  {
    this.InitializeComponent();
  }

  /// <summary>
  /// Invoked when the application is launched normally by the end user.  Other entry points
  /// will be used such as when the application is launched to open a specific file.
  /// </summary>
  /// <param name="args">Details about the launch request and process.</param>
  protected override void OnLaunched(LaunchActivatedEventArgs args)
  {
    var commandLineArgs = Parser.Default.ParseArguments<CommandLineOptions>(Environment.GetCommandLineArgs());

    m_window = new MainWindow(commandLineArgs.Value.SpaUri);


    WebView2Messaging webView2Messaging = new(m_window.WebView);
    RequestDispatcher requestDispatcher = new(webView2Messaging, new ApiGeneration.Generated.RequestDispatcherImpl(new SampleController()));

    m_window.Activate();
  }

  private MainWindow m_window;
}
