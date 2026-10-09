using DotnetManager.Core.Models;
using DotnetManager.Installation.Resolver.Exceptions;
using DotnetManager.Installation.Resolver.Models.Results;
using DotnetManager.Installation.Resolver.Models.Targets;
using DotnetManager.Platform.Abstractions;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;

namespace DotnetManager.Installation.Resolver.Services;

internal class LatestResolver
{
    private readonly ISdkManifestProviderService _provider;
    private readonly IHostPlatformService _platform;

    public LatestResolver(ISdkManifestProviderService provider, IHostPlatformService platform)
    {
        _provider = provider;
        _platform = platform;
    }

    public async Task<ResolvedInstallation> ResolveLatestAsync(LatestSelector latest,
        IEnumerable<DotnetComponent> requestComponents,
        string? requestRid,
        CancellationToken cancellationToken)
    {
        var index = await _provider.GetReleaseIndexAsync(cancellationToken);
        IEnumerable<SdkChannel> channels = index.Releases;

        if (latest.ReleaseType is {} releaseType)
            channels = channels.Where(x => x.ReleaseTypes == releaseType);

        if (latest.SupportPhase is {} supportPhase)
            channels = channels.Where(x => x.SupportPhase == supportPhase);

        var sdkChannel = channels.MaxBy(x => x.ChannelVersion);

        if (sdkChannel is null)
        {
            throw new InstallChannelNotFoundException(latest.ReleaseType, latest.SupportPhase);
        }

        var release = await _provider.GetReleasesAsync(sdkChannel.ReleasesUri, cancellationToken);
        return ResolveLatestReleaseAsync(release, requestComponents, requestRid);
    }

    private static ReleaseFile ResolveArtifact(DotnetComponent component,
        DotnetVersion? dotnetVersion,
        string? rid)
    {
        var packageName = component switch
        {
            DotnetComponent.Sdk => "dotnet-sdk",
            DotnetComponent.Runtime => "dotnet-runtime",
            DotnetComponent.AspNetRuntime => "aspnetcore-runtime",
            _ => throw new ArgumentOutOfRangeException(nameof(component), component,
                "Unsupported installation component.")
        };

        var tarGzName = $"{packageName}-{rid}.tar.gz";

        return dotnetVersion?.Artifacts
                   .Where(x => x.Rid == rid)
                   .FirstOrDefault(x =>
                       x.FileName.Equals(tarGzName, StringComparison.OrdinalIgnoreCase)) ??
               throw new Exception($"Could not find a {packageName} artifact for RID '{rid}'.");
    }

    private ResolvedInstallation ResolveLatestReleaseAsync(SdkReleaseManifest manifest,
        IEnumerable<DotnetComponent> components,
        string? rid)
    {
        List<ResolvedComponent> result = [];
        var latestRelease = manifest.Releases.MaxBy(x => x.ReleaseVersion);
        var ridResolve = string.IsNullOrWhiteSpace(rid) ? _platform.RuntimeIdentifier : rid;

        foreach (var dotnetComponent in components)
        {
            switch (dotnetComponent)
            {
                case DotnetComponent.Sdk:
                    var sdk = ResolveSdk(latestRelease, dotnetComponent, manifest, ridResolve);
                    result.Add(sdk);
                    break;

                case DotnetComponent.Runtime:
                    var runtime = ResolveRuntime(latestRelease, dotnetComponent, manifest, ridResolve);
                    result.Add(runtime);
                    break;
                case DotnetComponent.AspNetRuntime:
                    var asp = ResolveAspNetRuntime(latestRelease, dotnetComponent, manifest, ridResolve);
                    result.Add(asp);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return new ResolvedInstallation(result);
    }

    private ResolvedComponent ResolveSdk(SdkRelease? latestRelease,
        DotnetComponent dotnetComponent,
        SdkReleaseManifest manifest,
        string? rid)
    {
        var dotnetVersion = latestRelease?.Sdks.FirstOrDefault(x => x.Version == manifest.LatestSdk);
        return SharedResolvedComponent(dotnetComponent, manifest, dotnetVersion, rid);
    }

    private ResolvedComponent ResolveRuntime(SdkRelease? latestRelease,
        DotnetComponent dotnetComponent,
        SdkReleaseManifest manifest,
        string? rid)
    {
        var dotnetVersion = latestRelease?.Runtime;
        return SharedResolvedComponent(dotnetComponent, manifest, dotnetVersion, rid);
    }

    private ResolvedComponent ResolveAspNetRuntime(SdkRelease? latestRelease,
        DotnetComponent dotnetComponent,
        SdkReleaseManifest manifest,
        string? rid)
    {
        var dotnetVersion = latestRelease?.AspNetCoreRuntime;
        return SharedResolvedComponent(dotnetComponent, manifest, dotnetVersion, rid);
    }


    private ResolvedComponent SharedResolvedComponent(DotnetComponent dotnetComponent,
        SdkReleaseManifest manifest,
        DotnetVersion? dotnetVersion,
        string? rid)
    {
        var dotnet = ResolveArtifact(dotnetComponent, dotnetVersion, rid);

        return new ResolvedComponent
        {
            Component = dotnetComponent,
            Version = manifest.LatestSdk,
            DownloadUri = dotnet.Url,
            FileName = dotnet.FileName,
            Hash = dotnet.Hash
        };
    }
}