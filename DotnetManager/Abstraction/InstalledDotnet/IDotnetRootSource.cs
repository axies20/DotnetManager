namespace DotnetManager.Abstraction.InstalledDotnet;

public interface IDotnetRootSource
{
    IEnumerable<string> DiscoverRoots();
}