namespace DotnetManager.Models.Sdk.SdkManifest.ReleaseManifest;

public sealed record ReleaseFile
{
    public required string Rid { get; init; }
    public required Uri Url { get; init; }
    public required string FileName { get; init; }
    public required string Hash { get; init; }
}