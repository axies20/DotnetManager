using DotnetManager.Core.Models;
using DotnetManager.Installation.Resolver.Models.Targets;

namespace DotnetManager.Installation.Resolver.Models.Requests;

public sealed record InstallRequest(InstallTarget Target, IReadOnlyCollection<DotnetComponent> Components, string? Rid = null);