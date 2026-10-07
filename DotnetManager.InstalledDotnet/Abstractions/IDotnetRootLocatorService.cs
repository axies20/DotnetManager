namespace DotnetManager.InstalledDotnet.Abstractions;

internal interface IDotnetRootLocatorService
{
    IReadOnlyCollection<string> GetRoots();
}
