using DotnetManager.InstalledDotnet.Abstractions;

namespace DotnetManager.UnitTests.InstalledDotnet;

internal sealed class LocatorStubRootLocator(params string[] roots) : IDotnetRootLocatorService
{
    public IReadOnlyCollection<string> GetRoots() => roots;
}
