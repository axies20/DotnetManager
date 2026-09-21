using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using DotnetManager.Cli.Commands.Available.Abstraction;
using DotnetManager.Cli.Output;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;
using NuGet.Versioning;

namespace DotnetManager.Cli.Commands.Available;

public class AvailableCommand(ISdkReleaseService sdkReleaseService) : ICommand
{
    private readonly Argument<NuGetVersion?> _channelArgument = new("channel")
    {
        Description = "The .NET channel to show available SDKs for.",
        Arity = ArgumentArity.ZeroOrOne,
        CustomParser = result =>
        {
            var value = result.Tokens.Single().Value;

            if (NuGetVersion.TryParse(value, out var version))
                return version;

            result.AddError($"'{value}' is not a valid .NET channel version.");
            return null;
        }
    };

    public Command Initialize()
    {
        var command = new Command("available",
            "Browse .NET SDK versions available for installation from the configured release metadata source, including releases for a selected channel and support policy.");

        command.Arguments.Add(_channelArgument);
        command.SetAction(Execute);
        return command;
    }

    private static void PrintAllReleases(IReadOnlyCollection<SdkChannel> channels)
    {
        TableColumn<SdkChannel>[] columns =
        [
            new("Channel", x => x.ChannelVersion.ToString()),
            new("Security", x => x.Security ? "Yes" : "No"),
            new("Latest Runtime", x => x.LatestRuntime),
            new("Latest SDK", x => x.LatestSdk),
            new("Support Phase", x => x.SupportPhase.ToString()),
            new("Release Type", x => x.ReleaseTypes.ToString())
        ];
        TablePrinter.Print("All available channels:", channels, columns);
    }

    private static void PrintCurrentRelease(SdkReleaseManifest manifest)
    {
        PrintChannelInfo(manifest);
        PrintAvailableReleases(manifest.Releases);
    }


    private static void PrintChannelInfo(SdkReleaseManifest releases)
    {
        TableColumn<SdkReleaseManifest>[] sdkReleases =
        [
            new("Channel", x => x.ChannelVersion.ToString()),
            new("Latest Release", x => x.LatestRelease.ToString()),
            new("Latest Runtime", x => x.LatestRuntime.ToString()),
            new("Latest SDK", x => x.LatestSdk.ToString()),
            new("Support Phase", x => x.SupportPhase.ToString()),
            new("Release Type", x => x.ReleaseType.ToString())
        ];
        TablePrinter.Print("Channel information", releases, sdkReleases);
    }

    private static void PrintAvailableReleases(IReadOnlyCollection<SdkRelease> releases)
    {
        TableColumn<SdkRelease>[] columns =
        [
            new("Release", x => x.ReleaseVersion.ToString()),
            new("Security", x => x.Security ? "Yes" : "No"),
            new("SDKs", x => string.Join(Environment.NewLine, x.Sdks.Select(sdk => sdk.Version))),
            new("Runtime", x => x.Runtime.Version.ToString()),
            new("ASP.NET Core", x => x.AspNetCoreRuntime.Version.ToString())
        ];

        TablePrinter.Print("Available releases", releases, columns);

    }

    private async Task Execute(ParseResult result, CancellationToken ct)
    {
        var arg = result.GetValue(_channelArgument);

        if (arg is null)
        {
            var channels = await sdkReleaseService.GetChannelsAsync(ct);
            PrintAllReleases(channels);
        }
        else
        {
            var releases = await sdkReleaseService.GetReleasesAsync(arg, ct);
            PrintCurrentRelease(releases);
        }
    }
}