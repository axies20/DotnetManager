using System.Runtime.InteropServices;
using DotnetManager.Installation.Exceptions;
using DotnetManager.Installation.Abstractions.Resolver;
using DotnetManager.Core.Models;
using DotnetManager.Installation.Models.Downloads;
using DotnetManager.Installation.Models.Installation.Requests;
using DotnetManager.Installation.Models.Installation.Targets;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Models.Releases;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace DotnetManager.Installation.Services.Resolver;

internal sealed class DotnetInstallResolver : IDotnetInstallResolverService
{
    private readonly ISdkManifestProviderService _provider;
    private readonly IInstallReleaseResolverService<LatestSelector> _latestResolver;
    private readonly IInstallReleaseResolverService<VersionSelector> _versionResolver;
    private readonly ILogger<DotnetInstallResolver> _logger;

    public DotnetInstallResolver(ISdkManifestProviderService provider,
        IInstallReleaseResolverService<LatestSelector> latestResolver,
        IInstallReleaseResolverService<VersionSelector> versionResolver,
        ILogger<DotnetInstallResolver> logger)
    {
        _provider = provider;
        _latestResolver = latestResolver;
        _versionResolver = versionResolver;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<DotnetDownloadSource>> ResolveAsync(InstallRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resolving .NET artifacts for {TargetType}",
            request.Target.GetType().Name);
        var index = await _provider.GetReleaseIndexAsync(cancellationToken);
        SdkRelease? release;

        switch (request.Target)
        {
            case LatestSelector latestSelector:
                release = await _latestResolver.ResolveAsync(index.Releases,request.Options, latestSelector, cancellationToken);
                break;
            case VersionSelector versionSelector:
                release = await _versionResolver.ResolveAsync(index.Releases, versionSelector, cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(request.Target));
        }

        _logger.LogInformation("Selected .NET release {ReleaseVersion}", release.ReleaseVersion);
        _logger.LogDebug("Resolving artifacts for runtime identifier {RuntimeIdentifier}",
            string.IsNullOrWhiteSpace(request.Options.RuntimeIdentifier)
                ? RuntimeInformation.RuntimeIdentifier
                : request.Options.RuntimeIdentifier);

        var sources = CreateDownloadSources(release, request.Options);

        _logger.LogInformation("Resolved {ArtifactCount} artifact(s) for download",
            sources.Count);
        return sources;
    }


    private static IReadOnlyCollection<DotnetDownloadSource> CreateDownloadSources(SdkRelease release,
        InstallOptions options)
    {
        string rid;

        if (string.IsNullOrWhiteSpace(options.RuntimeIdentifier))
            rid = RuntimeInformation.RuntimeIdentifier;
        else
            rid = options.RuntimeIdentifier;

        return options.Components
            .Select(component => ResolveFile(release, component, rid))
            .Select(file => new DotnetDownloadSource(file.Url, file.FileName, file.Hash))
            .ToList();
    }


    private static ReleaseFile ResolveFile(SdkRelease release, DotnetComponent component, string rid)
    {
        return component switch
        {
            DotnetComponent.Sdk => ResolveLatestSdkFile(release, rid),
            DotnetComponent.Runtime => ResolveArtifact(release.Runtime, rid, "runtime", "dotnet-runtime"),
            DotnetComponent.AspNetRuntime => ResolveArtifact(release.AspNetCoreRuntime, rid, "ASP.NET Core runtime",
                "aspnetcore-runtime"),
            _ => throw new ArgumentOutOfRangeException(nameof(component), component,
                "Unsupported install component.")
        };
    }

    private static ReleaseFile ResolveLatestSdkFile(SdkRelease release, string rid)
    {
        var sdk = release.Sdks.MaxBy(x => x.Version,
            VersionComparer.VersionRelease);

        if (sdk is null)
            throw new InstallSdkNotFoundException(release.ReleaseVersion);

        return ResolveArtifact(sdk, rid, "SDK", "dotnet-sdk");
    }

    private static ReleaseFile ResolveArtifact(DotnetVersion version,
        string rid,
        string componentName,
        string archiveName)
    {
        var tarGzName = $"{archiveName}-{rid}.tar.gz";
        var zipName = $"{archiveName}-{rid}.zip";

        var first = version.Artifacts.Where(file => file.Rid == rid)
            .FirstOrDefault(file =>
            {
                if (file.FileName.Equals(tarGzName, StringComparison.OrdinalIgnoreCase))
                    return true;

                return file.FileName.Equals(zipName, StringComparison.OrdinalIgnoreCase);
            });

        return first ?? throw new InstallArtifactNotFoundException(componentName, version.Version, rid);
    }

}