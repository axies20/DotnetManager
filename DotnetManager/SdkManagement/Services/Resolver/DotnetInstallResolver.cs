using System.Runtime.InteropServices;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Models;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;
using DotnetManager.SdkManagement.Abstractions.Downloads;
using DotnetManager.SdkManagement.Abstractions.Resolver;
using DotnetManager.SdkManagement.Models.Downloads;
using DotnetManager.SdkManagement.Models.Installation;
using NuGet.Versioning;

namespace DotnetManager.SdkManagement.Services.Resolver;

public class DotnetInstallResolver : IDotnetInstallResolver
{
    private readonly IDotnetDownloader _downloader;
    private readonly ISdkManifestProvider _provider;

    public DotnetInstallResolver(IDotnetDownloader downloader, ISdkManifestProvider provider)
    {
        _downloader = downloader;
        _provider = provider;
    }

    public Task ResolveAsync(InstallRequest request,
        CancellationToken cancellationToken = default)
    {
        switch (request.Target)
        {
            case LatestSelector latestSelector:
                return ResolveLatestAsync(latestSelector, request.Components,
                    request.Security, cancellationToken);
            case VersionSelector versionSelector:
                return ResolveVersionAsync(versionSelector, request.Components,
                    request.Security, cancellationToken);
            default:
                throw new ArgumentOutOfRangeException(nameof(request.Target));
        }
    }

    private async Task ResolveLatestAsync(LatestSelector latestSelector,
        IEnumerable<InstallComponent> components,
        bool security,
        CancellationToken cancellationToken)
    {
        var index = await _provider.GetReleaseIndexAsync(cancellationToken);
        IEnumerable<SdkChannel> channels = index.Releases;

        if (latestSelector.SupportPhase is not null)
            channels = channels.Where(x => x.SupportPhase == latestSelector.SupportPhase);
        else
            channels = channels.Where(x => x.SupportPhase == SupportPhases.Active);


        if (latestSelector.ReleaseType is not null)
            channels = channels.Where(x => x.ReleaseTypes == latestSelector.ReleaseType);

        var latestChannel = channels
            .MaxBy(x => x.ChannelVersion, VersionComparer.VersionRelease);

        if (latestChannel is null)
            throw new InvalidOperationException("No matching .NET channel was found.");

        var manifest = await _provider.GetReleasesAsync(
            latestChannel.ReleasesUri, cancellationToken);
        IEnumerable<SdkRelease> releases = manifest.Releases;

        if (security)
            releases = releases.Where(x => x.Security);

        var latestRelease = releases
            .MaxBy(x => x.ReleaseVersion, VersionComparer.VersionRelease);

        if (latestRelease is null)
            throw new InvalidOperationException("No matching .NET release was found.");

        var rid = GetRid();

        foreach (var component in components)
        {
            var file = ResolveFile(latestRelease, component, rid);
            var source = new DotnetDownloadSource(file.Url, file.FileName);
            await _downloader.DownloadAsync(source, cancellationToken);
        }
    }

    private Task ResolveVersionAsync(VersionSelector versionSelector,
        IEnumerable<InstallComponent> components,
        bool security,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static ReleaseFile ResolveFile(SdkRelease release, InstallComponent component, string rid)
    {
        return component switch
        {
            InstallComponent.Sdk =>
                ResolveLatestSdkFile(release, rid),
            InstallComponent.Runtime =>
                ResolveArtifact(release.Runtime, rid, "runtime"),
            InstallComponent.AspNetRuntime =>
                ResolveArtifact(release.AspNetCoreRuntime, rid, "ASP.NET Core runtime"),
            _ => throw new ArgumentOutOfRangeException(nameof(component), component,
                "Unsupported install component.")
        };
    }

    private static ReleaseFile ResolveLatestSdkFile(SdkRelease release, string rid)
    {
        var sdk = release.Sdks.MaxBy(x => x.Version,
            VersionComparer.VersionRelease);

        if (sdk is null)
            throw new InvalidOperationException($"Release {release.ReleaseVersion} contains no SDKs.");

        return ResolveArtifact(sdk, rid, "SDK");
    }

    private static ReleaseFile ResolveArtifact(DotnetVersion version, string rid, string componentName)
    {
        return version.Artifacts.FirstOrDefault(x => x.Rid == rid) ??
               throw new PlatformNotSupportedException(
                   $"{componentName} {version.Version} is not available for RID '{rid}'.");
    }

    private static string GetRid()
    {
        var architecture = RuntimeInformation.OSArchitecture;

        var arch = architecture switch
        {
            Architecture.X64 => "x64",
            Architecture.X86 => "x86",
            Architecture.Arm64 => "arm64",
            Architecture.Arm => "arm",
            _ => throw new PlatformNotSupportedException(
                $"Unsupported architecture: {architecture}")
        };

        string os;

        if (OperatingSystem.IsWindows())
            os = "win";
        else if (OperatingSystem.IsLinux())
            os = "linux";
        else if (OperatingSystem.IsMacOS())
            os = "osx";
        else
            throw new PlatformNotSupportedException();

        return $"{os}-{arch}";
    }
}