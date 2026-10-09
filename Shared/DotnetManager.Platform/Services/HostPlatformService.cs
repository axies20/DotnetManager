using System.Runtime.InteropServices;
using DotnetManager.Platform.Abstractions;

namespace DotnetManager.Platform.Services;

internal sealed class HostPlatformService : IHostPlatformService
{
    public bool IsLinux => OperatingSystem.IsLinux();

    public bool IsMacOS => OperatingSystem.IsMacOS();

    public bool IsUnix => IsLinux || IsMacOS;

    public Architecture OSArchitecture => RuntimeInformation.OSArchitecture;

    public Architecture ProcessArchitecture => RuntimeInformation.ProcessArchitecture;

    public string RuntimeIdentifier => RuntimeInformation.RuntimeIdentifier;
}