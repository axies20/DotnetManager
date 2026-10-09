namespace DotnetManager.Installation.Services.Resolver;
/*
internal sealed class VersionInstallResolver(ISdkManifestProviderService provider)
{
    public async Task<SdkRelease> ResolveAsync(IEnumerable<SdkChannel> channels,
        VersionSelector selector,
        CancellationToken cancellationToken)
    {
        var version = selector.Version;
        var channel = channels.FirstOrDefault(x => x.ChannelVersion.Major == version.Major);

        if (channel is null)
            throw new InstallChannelNotFoundException(version);

        var manifest = await provider.GetReleasesAsync(channel.ReleasesUri, cancellationToken);


        return release ?? throw new InstallReleaseNotFoundException(version);
    }

    private SdkRelease? FindLatestVersion(SdkReleaseManifest manifest, NuGetVersion version)
    {
        if (version.Major != 0 && version is { Minor: 0, Patch: 0 })
        {
            return manifest.Releases.FirstOrDefault(x =>
                x.ReleaseVersion == manifest.LatestRelease);
        }

        if (version.Major != 0 && version.Minor != 0)
        {
            var range = new VersionRange(
                new NuGetVersion(version.Major, version.Minor, 0),
                includeMinVersion: true,
                new NuGetVersion(version.Major, version.Minor + 1, 0),
                includeMaxVersion: true);

            return manifest.Releases
                .Where(x => range.Satisfies(x.ReleaseVersion))
                .MaxBy(x => x.ReleaseVersion);
        }
        return manifest.Releases.FirstOrDefault(x=>x.)


    }
}*/