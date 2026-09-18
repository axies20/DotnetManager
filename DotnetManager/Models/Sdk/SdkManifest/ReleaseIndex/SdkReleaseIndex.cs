namespace DotnetManager.Models.Sdk.SdkManifest.ReleaseIndex;

public sealed record SdkReleaseIndex(IReadOnlyCollection<SdkChannel> Releases);
