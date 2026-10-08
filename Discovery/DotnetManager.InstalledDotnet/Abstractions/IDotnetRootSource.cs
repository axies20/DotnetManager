namespace DotnetManager.InstalledDotnet.Abstractions;

internal interface IDotnetRootSource
{
    IEnumerable<string> DiscoverRoots();
}