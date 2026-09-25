using DotnetManager.Installation.Services.Removal;
using DotnetManager.InstalledDotnet.Models;
using NuGet.Versioning;

namespace DotnetManager.UnitTests.Installation;

public class DotnetRemovalServiceTests
{
    [Fact]
    public void RemoveMajorDeletesMatchingVersionsOnly()
    {
        using var directory = new TestDirectory();
        var sdk10 = directory.CreateDirectory("sdk", "10.0.100");
        var sdk11 = directory.CreateDirectory("sdk", "11.0.100");
        var service = CreateService(
            new SdkInstallation(NuGetVersion.Parse("10.0.100"), sdk10),
            new SdkInstallation(NuGetVersion.Parse("11.0.100"), sdk11));

        service.RemoveAsync("10");

        Assert.False(Directory.Exists(sdk10));
        Assert.True(Directory.Exists(sdk11));
    }

    [Fact]
    public void RemoveMajorDeletesPrereleaseVersion()
    {
        using var directory = new TestDirectory();
        var sdk11Preview = directory.CreateDirectory("sdk", "11.0.100-rc.1");
        var service = CreateService(
            new SdkInstallation(NuGetVersion.Parse("11.0.100-rc.1"), sdk11Preview));

        service.RemoveAsync("11");

        Assert.False(Directory.Exists(sdk11Preview));
    }

    private static DotnetRemovalService CreateService(params SdkInstallation[] sdks)
    {
        return new DotnetRemovalService(
            new RemovalInstallationLocator<HostInstallation>([], x => x.Path),
            new RemovalInstallationLocator<RuntimeInstallation>([], x => x.Path),
            new RemovalInstallationLocator<SdkInstallation>(sdks, x => x.Path));
    }
}
