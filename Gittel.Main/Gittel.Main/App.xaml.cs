using ApiGenerator;
using ApiGenerator.WebView2;
using CommandLine;
using Gittel.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using Libgit2Bindings;

namespace Gittel.Main;

public partial class App : Application
{
  private ServiceProvider _serviceProvider;
  
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

    var services = new ServiceCollection();

    services.AddSingleton<MainWindow>();
    services.AddSingleton(commandLineArgs.Value);
    services.AddSingleton(sp => sp.GetRequiredService<MainWindow>().WebView);
    services.AddSingleton<IMessaging, WebView2Messaging>();
    services.AddSingleton<IRequestDispatcherImpl, ApiGeneration.Generated.RequestDispatcherImpl>();
    services.AddSingleton<RequestDispatcher>();
    services.RegisterControllers();
    services.RegisterLibgit2Bindings();

    _serviceProvider = services.BuildServiceProvider();
    _window = _serviceProvider.GetRequiredService<MainWindow>();
    _window.Closed += WindowClosed;

    _serviceProvider.GetRequiredService<RequestDispatcher>();

    _window.Activate();
  }

  private void WindowClosed(object sender, WindowEventArgs args)
  {
    _serviceProvider?.Dispose();
  }

  private MainWindow _window;
}
