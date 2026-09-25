namespace DotnetManager.InstalledDotnet.Abstractions;

public interface IDotnetRootLocatorService
{
    IReadOnlyCollection<string> GetRoots();
}
