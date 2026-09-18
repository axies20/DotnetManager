using System.Globalization;
using DotnetManager.Dto.DotNetManifest.Response.RawReleases;
using DotnetManager.Helper;
using DotnetManager.Models.Sdk.SdkManifest.ReleaseManifest;

namespace DotnetManager.Mapper;

public static class SdkReleaseManifestMapper
{
    public static SdkReleaseManifest Map(RawReleasesRoot rawReleasesRoot)
    {
        ArgumentNullException.ThrowIfNull(rawReleasesRoot);

        return new SdkReleaseManifest
        {
            ChannelVersion = MappingGuard.Required(rawReleasesRoot.ChannelVersion),
            LatestSdk = MappingGuard.Required(rawReleasesRoot.LatestSdk),
            SupportPhase = SupportPhasesMapper.Map(MappingGuard.Required(rawReleasesRoot.SupportPhase)),
            ReleaseType = ReleaseTypeMapper.Map(MappingGuard.Required(rawReleasesRoot.ReleaseType)),
            Releases = MappingGuard.Required(rawReleasesRoot.Releases).Select(MapRelease).ToList(),
        };
    }

    private static SdkRelease MapRelease(RawReleases rawReleases)
    {
        var runtime = MapRuntime(MappingGuard.Required(rawReleases.Runtime));
        var sdks = MapSdks(rawReleases.Sdks,
            MappingGuard.Required(rawReleases.Sdk));
        var aspNetCoreRuntime = MapAspNetCoreRuntime(
            MappingGuard.Required(rawReleases.RawAspNetCoreRuntime));

        return new SdkRelease
        {
            ReleaseVersion = MappingGuard.Required(rawReleases.ReleaseVersion),
            ReleaseDate = MapDate(rawReleases.ReleaseDate),
            Security = MappingGuard.Required(rawReleases.Security),
            Runtime = runtime,
            Sdks = sdks,
            AspNetCoreRuntime = aspNetCoreRuntime,
        };
    }

    private static DotnetVersion MapAspNetCoreRuntime(RawReleasesAspNetCoreRuntime rawReleasesAspNetCoreRuntime)
    {
        return new DotnetVersion
        {
            Version = MappingGuard.Required(rawReleasesAspNetCoreRuntime.Version),
            Artifacts = MappingGuard.Required(rawReleasesAspNetCoreRuntime.Files)
                .Select(MapArtifact).ToList()
        };
    }

    private static DotnetVersion MapSdk(RawReleasesSDK rawReleasesSdk)
    {
        var artifacts = MappingGuard.Required(rawReleasesSdk.Files)
            .Select(MapArtifact).ToList();

        return new DotnetVersion
        {
            Version = MappingGuard.Required(rawReleasesSdk.Version),
            Artifacts = artifacts
        };
    }

    private static List<DotnetVersion> MapSdks(List<RawReleasesSDK>? rawSdks, RawReleasesSDK rawSdk)
    {
        var sdks = rawSdks is null
            ? []
            : rawSdks.Select(MapSdk).ToList();

        return sdks.Append(MapSdk(rawSdk))
            .DistinctBy(x => x.Version)
            .ToList();
    }

    private static DotnetVersion MapRuntime(RawReleasesRuntime rawReleasesRuntime)
    {
        var artifacts = MappingGuard.Required(rawReleasesRuntime.Files)
            .Select(MapArtifact).ToList();

        return new DotnetVersion
        {
            Version = MappingGuard.Required(rawReleasesRuntime.Version),
            Artifacts = artifacts
        };
    }

    private static ReleaseFile MapArtifact(RawReleasesFile rawReleasesFile)
    {
        return new ReleaseFile
        {
            Rid = MappingGuard.Required(rawReleasesFile.Rid),
            Url = new Uri(MappingGuard.Required(rawReleasesFile.Url), UriKind.Absolute),
            FileName = MappingGuard.Required(rawReleasesFile.Name),
            Hash = MappingGuard.Required(rawReleasesFile.Hash)
        };
    }

    private static DateOnly MapDate(string? value)
    {
        return DateOnly.ParseExact(MappingGuard.Required(value),
            "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}