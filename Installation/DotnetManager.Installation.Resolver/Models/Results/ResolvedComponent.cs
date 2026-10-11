using DotnetManager.Core.Models;
using NuGet.Versioning;

namespace DotnetManager.Installation.Resolver.Models.Results;

public sealed record ResolvedComponent
{
    public DotnetComponent Component { get; init; }
    public required NuGetVersion Version { get; init; }
    public required Uri DownloadUri { get; init; }
    public required string FileName { get; init; }
    public string? Hash { get; init; }
}