using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Services;

namespace DotnetManager.UnitTests.InstalledDotnet;

public class LocatorTests
{
    [Fact]
    public void RootLocatorFiltersMissingAndDuplicateDirectories()
    {
        using var directory = new TestDirectory();
        var existing = directory.CreateDirectory("dotnet");
        IDotnetRootSource[] sources =
        [
            new LocatorStubRootSource(existing, Path.Combine(existing, ".")),
            new LocatorStubRootSource(Path.Combine(directory.Path, "missing"))
        ];

        var roots = new DotnetRootLocator(sources).GetRoots();

        Assert.Equal([Path.GetFullPath(existing)], roots);
    }

    [Fact]
    public void ComponentLocatorsDiscoverSdkRuntimeAndHostVersions()
    {
        using var directory = new TestDirectory();
        directory.CreateDirectory("sdk", "10.0.100");
        directory.CreateDirectory("shared", "Microsoft.NETCore.App", "10.0.1");
        directory.CreateDirectory("host", "fxr", "10.0.1");
        var rootLocator = new LocatorStubRootLocator(directory.Path);

        var sdk = Assert.Single(new SdkLocator(rootLocator).Find());
        var runtime = Assert.Single(new RuntimeLocator(rootLocator).Find());
        var host = Assert.Single(new HostLocator(rootLocator).Find());

        Assert.Equal("10.0.100", sdk.Version.ToString());
        Assert.Equal("Microsoft.NETCore.App", runtime.Framework);
        Assert.Equal("10.0.1", runtime.Version.ToString());
        Assert.Equal("10.0.1", host.Version.ToString());
    }

}
