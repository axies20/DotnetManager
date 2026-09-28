namespace DotnetManager.InstalledDotnet.Abstractions;

public interface IDotnetRootSource
{
    IEnumerable<string> DiscoverRoots();
}