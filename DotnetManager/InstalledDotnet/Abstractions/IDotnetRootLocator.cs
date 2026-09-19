namespace DotnetManager.InstalledDotnet.Abstractions;

public interface IDotnetRootLocator
{
    IReadOnlyCollection<string> GetRoots();
}