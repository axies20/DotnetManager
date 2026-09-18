namespace DotnetManager.Options;

public class DotnetManagerOptions
{
    public required Uri ReleaseIndexUrl { get; init; }

    public required string InstallRoot { get; init; }
}