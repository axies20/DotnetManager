using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using DotnetManager.Cli.Commands.Available.Abstraction;
using DotnetManager.Cli.Output;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;
using NuGet.Versioning;

namespace DotnetManager.Cli.Commands.Available;

public class AvailableCommand : ICommand
{
    private readonly ISdkReleaseService _sdkReleaseService;

    private readonly Argument<NuGetVersion?> _channelArgument = new("channel")
    {
        Description = "The .NET channel to show available SDKs for."
    };

    public AvailableCommand(ISdkReleaseService sdkReleaseService)
    {
        _sdkReleaseService = sdkReleaseService;
    }

    public Command Initialize()
    {
        var command = new Command("available",
            "Browse .NET SDK versions available for installation from the configured release metadata source, including releases for a selected channel and support policy.");

        command.Arguments.Add(_channelArgument);
        command.SetAction(Execute);
        return command;
    }

    private async Task Execute(ParseResult result, CancellationToken ct)
    {
        var arg = result.GetValue(_channelArgument);

        if (arg is null)
        {
            var channels = await _sdkReleaseService.GetChannelsAsync(ct);
            PrintAllReleases(channels);
        }
        else
        {
            var releases = await _sdkReleaseService.GetReleasesAsync(arg, ct);
            PrintCurrentRelease(releases);
        }
    }

    private static void PrintAllReleases(IReadOnlyCollection<SdkChannel> channels)
    {
        TableColumn<SdkChannel>[] columns =
        [
            new(nameof(SdkChannel.ChannelVersion), x => x.ChannelVersion.ToString()),
            new(nameof(SdkChannel.Security), x => x.Security.ToString()),
            new(nameof(SdkChannel.LatestSdk), x => x.LatestSdk),
            new(nameof(SdkChannel.SupportPhase), x => x.SupportPhase.ToString()),
            new(nameof(SdkChannel.ReleaseTypes), x => x.ReleaseTypes.ToString())
        ];
        TablePrinter.Print("All available channels:", channels, columns);
    }

    private static void PrintCurrentRelease(IReadOnlyCollection<SdkRelease> releases)
    {
        throw new NotImplementedException();
    }
}