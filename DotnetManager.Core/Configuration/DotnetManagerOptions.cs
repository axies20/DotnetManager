namespace DotnetManager.Core.Configuration;

public class DotnetManagerOptions
{
    public required Uri ReleaseIndexUrl { get; set; }

    public required string InstallRoot { get; set; }
}