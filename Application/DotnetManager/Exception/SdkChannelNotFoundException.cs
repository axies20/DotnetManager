using NuGet.Versioning;

namespace DotnetManager.Exception;

public sealed class SdkChannelNotFoundException : global::System.Exception
{

    public NuGetVersion ChannelVersion { get; }

    public SdkChannelNotFoundException(NuGetVersion channelVersion) : base(
        $"The .NET SDK channel '{channelVersion}' was not found.")
    {
        ChannelVersion = channelVersion;
    }
}