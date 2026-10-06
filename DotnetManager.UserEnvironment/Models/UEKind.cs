namespace DotnetManager.UserEnvironment.Models;

[Flags]
public enum UEKind
{
    None = 0,

    // User
    FishUser    = 1 << 0,
    OhMyZsh     = 1 << 1,
    SystemdUser = 1 << 2,

    // System
    PosixSystem = 1 << 3,
    FishSystem  = 1 << 4
}
