using DotnetManager.Installation.Models.Downloads;

namespace DotnetManager.UnitTests.Installation;

internal sealed class OrchestratorStubResolver(IReadOnlyCollection<DotnetDownloadSource> sources)
    : IDotnetInstallResolverService
{
    public Task<IReadOnlyCollection<DotnetDownloadSource>> ResolveAsync(InstallRequest request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(sources);
    }
}