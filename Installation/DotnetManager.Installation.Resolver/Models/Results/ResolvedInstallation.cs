namespace DotnetManager.Installation.Resolver.Models.Results;

public sealed record ResolvedInstallation(IReadOnlyList<ResolvedComponent> Components);
