using CommandLine;
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
        m_window.Activate();
    }

    private Window m_window;
}
