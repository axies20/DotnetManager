using Xunit;

namespace DotnetManager.IntegrationTests;

public sealed class PodmanFactAttribute : FactAttribute
{
    public PodmanFactAttribute()
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("DOTNET_MANAGER_RUN_CONTAINER_TESTS"),
                "1", StringComparison.Ordinal))
        {
            Skip = "Set DOTNET_MANAGER_RUN_CONTAINER_TESTS=1 to run Podman integration tests.";
        }
    }
}