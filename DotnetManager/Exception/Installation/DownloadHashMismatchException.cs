namespace DotnetManager.Exception.Installation;

public sealed class DownloadHashMismatchException(
    string fileName,
    string expectedHash,
    string actualHash)
    : DotnetInstallException($"SHA-512 verification failed for '{fileName}'.")
{
    public string FileName { get; } = fileName;

    public string ExpectedHash { get; } = expectedHash;

    public string ActualHash { get; } = actualHash;
}