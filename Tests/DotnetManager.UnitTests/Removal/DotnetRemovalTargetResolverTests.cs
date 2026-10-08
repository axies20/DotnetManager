using DotnetManager.Core.Models;
using DotnetManager.InstalledDotnet.Models;
using DotnetManager.Removal.Services;
using Microsoft.Extensions.Logging.Abstractions;
using NuGet.Versioning;

namespace DotnetManager.UnitTests.Removal;

public class DotnetRemovalTargetResolverTests
{
    [Fact]
    public void ResolveSupportsMajorMinorAndExactVersionSelectors()
    {
        using var directory = new TestDirectory();
        var sdk100 = CreateSdk(directory, "10.0.100");
        var sdk101Preview = CreateSdk(directory, "10.1.200-preview.1");
        var sdk110 = CreateSdk(directory, "11.0.100");
        var resolver = CreateResolver(sdks: [sdk100, sdk101Preview, sdk110]);

        Assert.Equal([sdk100.Path, sdk101Preview.Path],
            resolver.Resolve("10", DotnetComponent.Sdk));
        Assert.Equal([sdk101Preview.Path],
            resolver.Resolve("10.1", DotnetComponent.Sdk));
        Assert.Equal([sdk101Preview.Path],
            resolver.Resolve("10.1.200-preview.1", DotnetComponent.Sdk));
    }

    [Fact]
    public void ResolveSeparatesRuntimeFrameworksAndHosts()
    {
        using var directory = new TestDirectory();
        var runtimePath = directory.CreateDirectory("shared", "Microsoft.NETCore.App", "10.0.1");
        var aspNetPath = directory.CreateDirectory("shared", "Microsoft.AspNetCore.App", "10.0.1");
        var hostPath = directory.CreateDirectory("host", "fxr", "10.0.1");
        RuntimeInstallation[] runtimes =
        [
            new("Microsoft.NETCore.App", NuGetVersion.Parse("10.0.1"), runtimePath),
            new("Microsoft.AspNetCore.App", NuGetVersion.Parse("10.0.1"), aspNetPath)
        ];
        HostInstallation[] hosts = [new(NuGetVersion.Parse("10.0.1"), hostPath)];
        var resolver = CreateResolver(hosts, runtimes);

        Assert.Equal([runtimePath], resolver.Resolve("10", DotnetComponent.Runtime));
        Assert.Equal([aspNetPath], resolver.Resolve("10", DotnetComponent.AspNetRuntime));
        Assert.Equal([hostPath], resolver.Resolve("10", DotnetComponent.Host));
    }

    [Fact]
    public void ResolveRejectsInvalidVersion()
    {
        var resolver = CreateResolver();

        var exception = Assert.Throws<ArgumentException>(() =>
            resolver.Resolve("latest", DotnetComponent.Sdk));

        Assert.Contains("latest", exception.Message);
    }

    private static SdkInstallation CreateSdk(TestDirectory directory, string version)
    {
        return new SdkInstallation(NuGetVersion.Parse(version),
            directory.CreateDirectory("sdk", version));
    }

    private static DotnetRemovalTargetResolver CreateResolver(
        IReadOnlyCollection<HostInstallation>? hosts = null,
        IReadOnlyCollection<RuntimeInstallation>? runtimes = null,
        IReadOnlyCollection<SdkInstallation>? sdks = null)
    {
        return new DotnetRemovalTargetResolver(
            new RemovalInstallationLocator<HostInstallation>(hosts ?? [], x => x.Path),
            new RemovalInstallationLocator<RuntimeInstallation>(runtimes ?? [], x => x.Path),
            new RemovalInstallationLocator<SdkInstallation>(sdks ?? [], x => x.Path),
            NullLogger<DotnetRemovalTargetResolver>.Instance);
    }
}
