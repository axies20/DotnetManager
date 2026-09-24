using DotnetManager.ReleaseMetadata.Models;
using NuGet.Versioning;

namespace DotnetManager.Exception.Installation;

public sealed class InstallChannelNotFoundException : DotnetInstallException
{
    public InstallChannelNotFoundException(NuGetVersion channelVersion)
        : base($".NET channel '{channelVersion}' was not found.")
    {
        ChannelVersion = channelVersion;
    }

    public InstallChannelNotFoundException(
        ReleaseTypes? releaseType,
        SupportPhases? supportPhase)
        : base(CreateMessage(releaseType, supportPhase))
    {
        ReleaseType = releaseType;
        SupportPhase = supportPhase;
    }

    public NuGetVersion? ChannelVersion { get; }

    public ReleaseTypes? ReleaseType { get; }

    public SupportPhases? SupportPhase { get; }

    private static string CreateMessage(ReleaseTypes? releaseType, SupportPhases? supportPhase)
    {
        var releaseTypeText = releaseType?.ToString() ?? "any release type";
        var supportPhaseText = supportPhase?.ToString() ?? "any support phase";

        return $"No .NET channel matches release type '{releaseTypeText}' and support phase '{supportPhaseText}'.";
    }
}