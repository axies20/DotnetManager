namespace DotnetManager.UserEnvironment.Models;

[Flags]
public enum UEKind
{
    None = 0,
    FishUser = 1 << 0,
    OhMyZsh = 1 << 1,
    EnvironmentD = 1 << 2
}