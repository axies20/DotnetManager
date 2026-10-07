using DotnetManager.Cli.Output;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;

namespace DotnetManager.Cli.Commands.Available;

internal sealed partial class AvailableCommand
{
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
}
