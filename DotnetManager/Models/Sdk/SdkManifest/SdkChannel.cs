namespace DotnetManager.Models.Sdk.SdkManifest;

public sealed record SdkChannel
{
    public string ChannelVersion { get; init; }
    public string LatestSdk { get; init; }
    public string SupportPhase { get; init; }
    public Uri ReleasesUri { get; init; }

    public SdkChannel(string ChannelVersion, string LatestSdk, string SupportPhase, Uri ReleasesUri)
    {
       
    }


}