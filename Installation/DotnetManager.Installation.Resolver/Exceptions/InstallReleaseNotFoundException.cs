using NuGet.Versioning;

namespace DotnetManager.Installation.Resolver.Exceptions;

public sealed class InstallReleaseNotFoundException : Exception
{
    public NuGetVersion? Version { get; }

    public NuGetVersion? ChannelVersion { get; }

    public InstallReleaseNotFoundException(NuGetVersion version)
        : this(version, null)
    {
    }

    private InstallReleaseNotFoundException(NuGetVersion? version, NuGetVersion? channelVersion)
        : base(version is not null
            ? $".NET release '{version}' was not found."
            : $"No release was found in .NET channel '{channelVersion}'.")
    {
        Version = version;
        ChannelVersion = channelVersion;
    }

    public static InstallReleaseNotFoundException ForChannel(NuGetVersion channelVersion)
    {
        return new InstallReleaseNotFoundException(null, channelVersion);
    }
}
