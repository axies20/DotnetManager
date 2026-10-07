using System.CommandLine;

namespace DotnetManager.Cli.Commands.Available;

internal sealed partial class AvailableCommand
{
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
