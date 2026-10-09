using DotnetManager.Core.Models;
using NuGet.Versioning;

namespace DotnetManager.Installation.Resolver.Models.Results;

public sealed record ResolvedComponent
{
    public DotnetComponent Component { get; set; }
    public required NuGetVersion Version { get; set; }
    public required Uri DownloadUri { get; set; }
    public required string FileName { get; set; }
    public string? Hash { get; set; }
}