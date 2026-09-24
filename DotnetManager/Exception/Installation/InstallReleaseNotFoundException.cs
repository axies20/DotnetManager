using NuGet.Versioning;

namespace DotnetManager.Exception.Installation;

public sealed class InstallReleaseNotFoundException : DotnetInstallException
{
    public InstallReleaseNotFoundException(NuGetVersion version)
        : base($".NET release '{version}' was not found.")
    {
        Version = version;
    }

    public InstallReleaseNotFoundException(NuGetVersion channelVersion, bool securityOnly)
        : base(securityOnly
            ? $"No security release was found in .NET channel '{channelVersion}'."
            : $"No release was found in .NET channel '{channelVersion}'.")
    {
        ChannelVersion = channelVersion;
        SecurityOnly = securityOnly;
    }

    public NuGetVersion? Version { get; }

    public NuGetVersion? ChannelVersion { get; }

    public bool SecurityOnly { get; }
}