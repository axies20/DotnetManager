using System.Runtime.InteropServices;

namespace DotnetManager.Platform.Abstractions;

public interface IHostPlatformService
{
    bool IsLinux { get; }

    bool IsMacOS { get; }

    bool IsUnix { get; }

    Architecture OSArchitecture { get; }

    Architecture ProcessArchitecture { get; }

    string RuntimeIdentifier { get; }
}
