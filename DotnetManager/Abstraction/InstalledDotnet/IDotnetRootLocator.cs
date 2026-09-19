namespace DotnetManager.Abstraction.InstalledDotnet;

public interface IDotnetRootLocator
{
    IReadOnlyCollection<string> GetRoots();
}