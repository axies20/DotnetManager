namespace DotnetManager.Models.Sdk.SdkManifest;

public sealed record SdkChannel(string ChannelVersion, string LatestSdk, string SupportPhase, Uri ReleasesUri);