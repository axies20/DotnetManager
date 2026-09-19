using DotnetManager.InstalledDotnet.Abstractions;

namespace DotnetManager.InstalledDotnet.RootSources;

public class EnvironmentDotnetRootSource : IDotnetRootSource
{
    private static readonly string[] Variables =
    [
        "DOTNET_ROOT",
        "DOTNET_ROOT_X64",
        "DOTNET_ROOT_X86",
        "DOTNET_ROOT_ARM64",
        "DOTNET_ROOT(x86)"
    ];

    public IEnumerable<string> DiscoverRoots()
    {
        foreach (var variable in Variables)
        {
            var value = Environment.GetEnvironmentVariable(variable);

            if (!string.IsNullOrWhiteSpace(value))
                yield return value;
        }
    }
}