using CommandLine;

namespace Gittel.Main;

internal class CommandLineOptions
{
    [Option(shortName: 's', longName: "spa-uri", Required = false, HelpText = "The Uri of the spa application that is the gui", Default = "./Gittel.Ui/index.html")]
    public required string SpaUri { get; init; }
}
