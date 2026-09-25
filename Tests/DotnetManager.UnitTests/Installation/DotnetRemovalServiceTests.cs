using DotnetManager.Installation.Services.Removal;
using DotnetManager.InstalledDotnet.Models;
using NuGet.Versioning;

namespace DotnetManager.UnitTests.Installation;

public class DotnetRemovalServiceTests
{
    [Fact]
    public async Task RemoveMajorDeletesMatchingVersionsOnly()
    {
        using var directory = new TestDirectory();
        var sdk10 = directory.CreateDirectory("sdk", "10.0.100");
        var sdk11 = directory.CreateDirectory("sdk", "11.0.100");
        var environment = new RemovalRecordingEnvironmentConfigurator();
        var service = CreateService(directory.Path, environment,
            new SdkInstallation(NuGetVersion.Parse("10.0.100"), sdk10),
            new SdkInstallation(NuGetVersion.Parse("11.0.100"), sdk11));

        await service.RemoveAsync("10", false, CancellationToken.None);

        Assert.False(Directory.Exists(sdk10));
        Assert.True(Directory.Exists(sdk11));
        Assert.False(environment.RemoveCalled);
    }

    [Fact]
    public async Task CleanupPathRemovesConfigurationAfterLastInstallation()
    {
        using var directory = new TestDirectory();
        var sdk = directory.CreateDirectory("sdk", "10.0.100");
        var environment = new RemovalRecordingEnvironmentConfigurator();
        var service = CreateService(directory.Path, environment,
            new SdkInstallation(NuGetVersion.Parse("10.0.100"), sdk));

        await service.RemoveAsync("10", true, CancellationToken.None);

        Assert.True(environment.RemoveCalled);
        Assert.Equal(Path.GetFullPath(directory.Path), environment.RemovedRoot);
    }

    [Fact]
    public async Task CleanupPathKeepsConfigurationWhenInstallationRemains()
    {
        using var directory = new TestDirectory();
        var sdk10 = directory.CreateDirectory("sdk", "10.0.100");
        var sdk11 = directory.CreateDirectory("sdk", "11.0.100");
        var environment = new RemovalRecordingEnvironmentConfigurator();
        var service = CreateService(directory.Path, environment,
            new SdkInstallation(NuGetVersion.Parse("10.0.100"), sdk10),
            new SdkInstallation(NuGetVersion.Parse("11.0.100"), sdk11));

        await service.RemoveAsync("10", true, CancellationToken.None);

        Assert.False(environment.RemoveCalled);
    }

    private static DotnetRemovalService CreateService(
        string installRoot,
        RemovalRecordingEnvironmentConfigurator environment,
        params SdkInstallation[] sdks)
    {
        return new DotnetRemovalService(
            new RemovalInstallationLocator<HostInstallation>([], x => x.Path),
            new RemovalInstallationLocator<RuntimeInstallation>([], x => x.Path),
            new RemovalInstallationLocator<SdkInstallation>(sdks, x => x.Path),
            new RemovalPathProvider(installRoot),
            environment);
    }
}
