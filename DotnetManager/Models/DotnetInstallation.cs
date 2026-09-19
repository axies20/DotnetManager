namespace DotnetManager.Models;

public sealed record DotnetInstallation
{
    public string? Root { get; init; }
    public IReadOnlyCollection<SdkInstallation>? Sdks { get; init; }
    public IReadOnlyCollection<RuntimeInstallation>? Runtimes { get; init; }
    public IReadOnlyCollection<HostInstallation>? Hosts { get; init; }
}