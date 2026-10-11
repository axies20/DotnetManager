using DotnetManager.Core.Models;
using DotnetManager.Installation.Resolver.Exceptions;
using DotnetManager.Installation.Resolver.Models.Results;
using DotnetManager.Platform.Abstractions;
using DotnetManager.ReleaseMetadata.Models.Releases;
using NuGet.Versioning;

namespace DotnetManager.Installation.Resolver.Abstraction;

public abstract class InstallResolverBase
{
    private readonly IHostPlatformService _platform;

    protected InstallResolverBase(IHostPlatformService platform)
    {
        _platform = platform;
    }

    protected ResolvedInstallation ResolveRelease(SdkReleaseManifest manifest,
        IEnumerable<DotnetComponent> components,
        SdkRelease release,
        string? rid)
    {
        List<ResolvedComponent> result = [];

        foreach (var dotnetComponent in components)
        {
            switch (dotnetComponent)
            {
                case DotnetComponent.Sdk:
                    var sdk = release.Sdks.FirstOrDefault(x => x.Version == manifest.LatestSdk) ??
                              throw new InstallSdkNotFoundException(release.ReleaseVersion);
                    result.Add(ResolveComponent(dotnetComponent, sdk, rid));
                    break;

                case DotnetComponent.Runtime:
                    var runtime = release.Runtime;
                    result.Add(ResolveComponent(dotnetComponent, runtime, rid));
                    break;

                case DotnetComponent.AspNetRuntime:
                    var asp = release.AspNetCoreRuntime;
                    result.Add(ResolveComponent(dotnetComponent, asp, rid));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(dotnetComponent), dotnetComponent,
                        "Unsupported installation component.");
            }
        }

        return new ResolvedInstallation(result);
    }

    private string GetRid(string? rid) => 
        string.IsNullOrWhiteSpace(rid) ? _platform.RuntimeIdentifier : rid;

    private ResolvedComponent ResolveComponent(DotnetComponent dotnetComponent,
        DotnetVersion dotnetVersion,
        string? rid)
    {
        var ridResolve = GetRid(rid);
        var dotnet = ResolveArtifact(dotnetComponent, dotnetVersion, ridResolve);

        return new ResolvedComponent
        {
            Component = dotnetComponent,
            Version = dotnetVersion.Version,
            DownloadUri = dotnet.Url,
            FileName = dotnet.FileName,
            Hash = dotnet.Hash
        };
    }

    private static ReleaseFile ResolveArtifact(DotnetComponent component, DotnetVersion dotnetVersion, string rid)
    {
        var (componentName, packageName) = component switch
        {
            DotnetComponent.Sdk => ("SDK", "dotnet-sdk"),
            DotnetComponent.Runtime => (".NET runtime", "dotnet-runtime"),
            DotnetComponent.AspNetRuntime => ("ASP.NET Core runtime", "aspnetcore-runtime"),
            _ => throw new ArgumentOutOfRangeException(nameof(component), component,
                "Unsupported installation component.")
        };

        var tarGzName = $"{packageName}-{rid}.tar.gz";

        return dotnetVersion.Artifacts
                   .Where(x => x.Rid == rid)
                   .FirstOrDefault(x => x.FileName.Equals(tarGzName, StringComparison.OrdinalIgnoreCase)) ??
               throw new InstallArtifactNotFoundException(componentName, dotnetVersion.Version, rid);
    }
}