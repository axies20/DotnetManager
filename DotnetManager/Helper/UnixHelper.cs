using System.Runtime.InteropServices;

namespace DotnetManager.Helper;

internal static partial class UnixHelper
{
    public static bool IsRoot()
    {
        return (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()) &&
               GetEffectiveUserId() == 0;
    }

    [LibraryImport("libc", EntryPoint = "geteuid")]
    private static partial uint GetEffectiveUserId();
}