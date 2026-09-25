using DotnetManager.InstalledDotnet.Abstractions;

namespace DotnetManager.UnitTests.InstalledDotnet;

internal sealed class LocatorStubRootSource(params string[] roots) : IDotnetRootSource
{
    public IEnumerable<string> DiscoverRoots() => roots;
}
