namespace DotnetManager.ReleaseMetadata.Models.Index;

public sealed record SdkReleaseIndex(IReadOnlyCollection<SdkChannel> Releases);