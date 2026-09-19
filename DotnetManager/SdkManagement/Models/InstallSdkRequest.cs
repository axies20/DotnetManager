using DotnetManager.ReleaseMetadata.Models;

namespace DotnetManager.SdkManagement.Models;

public sealed record InstallSdkRequest(string? Channel, string? Version, SupportPhases Policy);