using DotnetManager.Installation.Abstractions.Resolver;
using DotnetManager.Installation.Models.Downloads;
using DotnetManager.Installation.Models.Installation.Requests;

namespace DotnetManager.UnitTests.SdkManagement;

internal sealed class OrchestratorStubResolver(IReadOnlyCollection<DotnetDownloadSource> sources)
    : IDotnetInstallResolverService
{
    public Task<IReadOnlyCollection<DotnetDownloadSource>> ResolveAsync(InstallRequest request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(sources);
    }
}
