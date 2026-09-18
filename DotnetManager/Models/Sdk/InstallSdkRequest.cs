namespace DotnetManager.Models.Sdk;

public sealed record InstallSdkRequest(string? Channel, string? Version, SupportPhases Policy);