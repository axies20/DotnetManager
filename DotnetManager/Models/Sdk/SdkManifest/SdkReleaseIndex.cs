namespace DotnetManager.Models.Sdk.SdkManifest;

public sealed record SdkReleaseIndex(IReadOnlyCollection<SdkChannel> Releases);