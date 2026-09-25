using DotnetManager.Exception.Installation;
using DotnetManager.Installation.Models;
using DotnetManager.Installation.Models.Installation.Requests;
using DotnetManager.Installation.Models.Installation.Targets;
using DotnetManager.Installation.Services.Resolver;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Models;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;
using NuGet.Versioning;

namespace DotnetManager.UnitTests.SdkManagement;

public class DotnetInstallResolverTests
{
    [Fact]
    public async Task ExactVersionSelectsLatestSdkAndExactRidArtifact()
    {
        var release = CreateRelease("10.0.2", true, ["10.0.100", "10.0.200"]);
        var resolver = CreateResolver(CreateChannel("10.0", ReleaseTypes.Lts,
            SupportPhases.Active), release);

        var sources = await resolver.ResolveAsync(CreateRequest(
            new VersionSelector(NuGetVersion.Parse("10.0.2")),
            [DotnetComponent.Sdk], "linux-x64"), CancellationToken.None);

        var source = Assert.Single(sources);
        Assert.Equal("dotnet-sdk-linux-x64.tar.gz", source.FileName);
        Assert.Contains("10.0.200", source.Uri.AbsoluteUri);
    }

    [Fact]
    public async Task LatestFiltersChannelAndSecurityRelease()
    {
        var sts = CreateChannel("11.0", ReleaseTypes.Sts, SupportPhases.Preview);
        var lts = CreateChannel("10.0", ReleaseTypes.Lts, SupportPhases.Active);
        var provider = new StubManifestProvider(
            new SdkReleaseIndex([sts, lts]),
            new Dictionary<Uri, SdkReleaseManifest>
            {
                [lts.ReleasesUri] = CreateManifest("10.0",
                    CreateRelease("10.0.1", true),
                    CreateRelease("10.0.2", false)),
                [sts.ReleasesUri] = CreateManifest("11.0", CreateRelease("11.0.1", true))
            });
        var resolver = new DotnetInstallResolver(provider);

        var sources = await resolver.ResolveAsync(CreateRequest(
            new LatestSelector(ReleaseTypes.Lts, SupportPhases.Active, true),
            [DotnetComponent.Runtime], "linux-x64"), CancellationToken.None);

        Assert.Contains("10.0.1", Assert.Single(sources).Uri.AbsoluteUri);
    }

    [Fact]
    public async Task ResolverReturnsBothRequestedRuntimeComponents()
    {
        var release = CreateRelease("10.0.2", true);
        var resolver = CreateResolver(CreateChannel("10.0", ReleaseTypes.Lts,
            SupportPhases.Active), release);

        var sources = await resolver.ResolveAsync(CreateRequest(
                new VersionSelector(NuGetVersion.Parse("10.0.2")),
                [DotnetComponent.Runtime, DotnetComponent.AspNetRuntime], "linux-x64"),
            CancellationToken.None);

        Assert.Equal(2, sources.Count);
        Assert.Contains(sources, x => x.FileName == "dotnet-runtime-linux-x64.tar.gz");
        Assert.Contains(sources, x => x.FileName == "aspnetcore-runtime-linux-x64.tar.gz");
    }

    [Fact]
    public async Task ExactVersionDoesNotRequireSecurityRelease()
    {
        var release = CreateRelease("10.0.2", false);
        var resolver = CreateResolver(CreateChannel("10.0", ReleaseTypes.Lts,
            SupportPhases.Active), release);

        var sources = await resolver.ResolveAsync(CreateRequest(
            new VersionSelector(NuGetVersion.Parse("10.0.2")),
            [DotnetComponent.Sdk], "linux-x64"), CancellationToken.None);

        Assert.Single(sources);
    }

    [Fact]
    public async Task ResolverRejectsMissingRidArtifact()
    {
        var release = CreateRelease("10.0.2", true);
        var resolver = CreateResolver(CreateChannel("10.0", ReleaseTypes.Lts,
            SupportPhases.Active), release);

        await Assert.ThrowsAsync<InstallArtifactNotFoundException>(() => resolver.ResolveAsync(
            CreateRequest(new VersionSelector(NuGetVersion.Parse("10.0.2")),
                [DotnetComponent.Sdk], "freebsd-x64"), CancellationToken.None));
    }

    [Fact]
    public async Task LatestSecuritySelectionFailsWhenOnlyNonSecurityExists()
    {
        var release = CreateRelease("10.0.2", false);
        var resolver = CreateResolver(CreateChannel("10.0", ReleaseTypes.Lts,
            SupportPhases.Active), release);

        await Assert.ThrowsAsync<InstallReleaseNotFoundException>(() => resolver.ResolveAsync(
            CreateRequest(new LatestSelector(null, null, true),
                [DotnetComponent.Sdk], "linux-x64"), CancellationToken.None));
    }

    private static DotnetInstallResolver CreateResolver(SdkChannel channel, params SdkRelease[] releases)
    {
        return new DotnetInstallResolver(new StubManifestProvider(
            new SdkReleaseIndex([channel]),
            new Dictionary<Uri, SdkReleaseManifest>
            {
                [channel.ReleasesUri] = CreateManifest(channel.ChannelVersion.ToString(), releases)
            }));
    }

    private static InstallRequest CreateRequest(InstallTarget target,
        IReadOnlyCollection<DotnetComponent> components,
        string rid)
    {
        return new InstallRequest
        {
            Target = target,
            Options = new InstallOptions
            {
                Components = components,
                RuntimeIdentifier = rid
            }
        };
    }

    private static SdkChannel CreateChannel(string version,
        ReleaseTypes releaseType,
        SupportPhases supportPhase)
    {
        return new SdkChannel
        {
            ChannelVersion = NuGetVersion.Parse(version),
            Security = true,
            LatestSdk = "10.0.200",
            LatestRuntime = "10.0.2",
            ReleaseTypes = releaseType,
            SupportPhase = supportPhase,
            ReleasesUri = new Uri($"https://example.test/{version}/releases.json")
        };
    }

    private static SdkReleaseManifest CreateManifest(string channel, params SdkRelease[] releases)
    {
        return new SdkReleaseManifest
        {
            ChannelVersion = NuGetVersion.Parse(channel),
            LatestSdk = NuGetVersion.Parse("10.0.200"),
            LatestRelease = releases.MaxBy(x => x.ReleaseVersion)!.ReleaseVersion,
            LatestRuntime = releases.MaxBy(x => x.ReleaseVersion)!.Runtime.Version,
            ReleaseType = ReleaseTypes.Lts,
            SupportPhase = SupportPhases.Active,
            Releases = releases
        };
    }

    private static SdkRelease CreateRelease(string releaseVersion,
        bool security,
        string[]? sdkVersions = null)
    {
        sdkVersions ??= ["10.0.100"];
        return new SdkRelease
        {
            ReleaseVersion = NuGetVersion.Parse(releaseVersion),
            ReleaseDate = new DateOnly(2026, 1, 1),
            Security = security,
            Runtime = CreateVersion(releaseVersion, "dotnet-runtime"),
            AspNetCoreRuntime = CreateVersion(releaseVersion, "aspnetcore-runtime"),
            Sdks = sdkVersions.Select(x => CreateVersion(x, "dotnet-sdk")).ToList()
        };
    }

    private static DotnetVersion CreateVersion(string version, string archiveName)
    {
        return new DotnetVersion
        {
            Version = NuGetVersion.Parse(version),
            Artifacts =
            [
                new ReleaseFile
                {
                    Rid = "linux-x64",
                    FileName = $"{archiveName}-linux-x64.tar.gz",
                    Hash = $"hash-{version}",
                    Url = new Uri($"https://example.test/{version}/{archiveName}-linux-x64.tar.gz")
                },
                new ReleaseFile
                {
                    Rid = "linux-x64",
                    FileName = $"{archiveName}-linux-x64.exe",
                    Hash = "wrong-artifact",
                    Url = new Uri("https://example.test/wrong-artifact")
                }
            ]
        };
    }

    private sealed class StubManifestProvider(
        SdkReleaseIndex index,
        IReadOnlyDictionary<Uri, SdkReleaseManifest> manifests) : ISdkManifestProviderService
    {
        public Task<SdkReleaseIndex> GetReleaseIndexAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(index);
        }

        public Task<SdkReleaseManifest> GetReleasesAsync(Uri manifestUri,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(manifests[manifestUri]);
        }
    }
}
