using DotnetManager.Cli.Commands.Available.Services;
using DotnetManager.Exception;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Models;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;
using NuGet.Versioning;

namespace DotnetManager.UnitTests.Cli;

public class SdkReleaseServiceTests
{
    [Fact]
    public async Task GetChannelsAsyncReturnsProviderChannels()
    {
        var channel = CreateChannel();
        var service = new SdkReleaseService(new StubProvider(channel, CreateManifest()));

        var channels = await service.GetChannelsAsync(CancellationToken.None);

        Assert.Equal([channel], channels);
    }

    [Fact]
    public async Task GetReleasesAsyncUsesUriFromMatchingChannel()
    {
        var channel = CreateChannel();
        var manifest = CreateManifest();
        var provider = new StubProvider(channel, manifest);
        var service = new SdkReleaseService(provider);

        var result = await service.GetReleasesAsync(NuGetVersion.Parse("10.0"),
            CancellationToken.None);

        Assert.Same(manifest, result);
        Assert.Equal(channel.ReleasesUri, provider.RequestedManifestUri);
    }

    [Fact]
    public async Task GetReleasesAsyncRejectsUnknownChannel()
    {
        var service = new SdkReleaseService(new StubProvider(CreateChannel(), CreateManifest()));

        await Assert.ThrowsAsync<SdkChannelNotFoundException>(() =>
            service.GetReleasesAsync(NuGetVersion.Parse("9.0"), CancellationToken.None));
    }

    private static SdkChannel CreateChannel()
    {
        return new SdkChannel
        {
            ChannelVersion = NuGetVersion.Parse("10.0"),
            Security = true,
            LatestSdk = "10.0.100",
            LatestRuntime = "10.0.1",
            SupportPhase = SupportPhases.Active,
            ReleaseTypes = ReleaseTypes.Lts,
            ReleasesUri = new Uri("https://example.test/10.0/releases.json")
        };
    }

    private static SdkReleaseManifest CreateManifest()
    {
        return new SdkReleaseManifest
        {
            ChannelVersion = NuGetVersion.Parse("10.0"),
            LatestSdk = NuGetVersion.Parse("10.0.100"),
            LatestRuntime = NuGetVersion.Parse("10.0.1"),
            LatestRelease = NuGetVersion.Parse("10.0.1"),
            SupportPhase = SupportPhases.Active,
            ReleaseType = ReleaseTypes.Lts,
            Releases = []
        };
    }

    private sealed class StubProvider(SdkChannel channel, SdkReleaseManifest manifest)
        : ISdkManifestProviderService
    {
        public Uri? RequestedManifestUri { get; private set; }

        public Task<SdkReleaseIndex> GetReleaseIndexAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new SdkReleaseIndex([channel]));
        }

        public Task<SdkReleaseManifest> GetReleasesAsync(Uri manifestUri,
            CancellationToken cancellationToken)
        {
            RequestedManifestUri = manifestUri;
            return Task.FromResult(manifest);
        }
    }
}
