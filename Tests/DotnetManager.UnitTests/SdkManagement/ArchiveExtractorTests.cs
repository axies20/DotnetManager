using System.IO.Compression;
using DotnetManager.Installation.Services.Archives;

namespace DotnetManager.UnitTests.SdkManagement;

public class ArchiveExtractorTests
{
    [Fact]
    public async Task ExtractAsyncExtractsZipArchive()
    {
        using var directory = new TestDirectory();
        var source = directory.CreateDirectory("source");
        await File.WriteAllTextAsync(Path.Combine(source, "dotnet"), "payload");
        var archive = Path.Combine(directory.Path, "dotnet.zip");
        ZipFile.CreateFromDirectory(source, archive);
        var destination = Path.Combine(directory.Path, "destination");

        await new ArchiveExtractor().ExtractAsync(archive, destination, CancellationToken.None);

        Assert.Equal("payload", await File.ReadAllTextAsync(
            Path.Combine(destination, "dotnet"), CancellationToken.None));
    }

    [Fact]
    public async Task ExtractAsyncRejectsUnknownArchiveFormat()
    {
        var exception = await Assert.ThrowsAsync<NotSupportedException>(() =>
            new ArchiveExtractor().ExtractAsync("archive.7z", "destination",
                CancellationToken.None));

        Assert.Contains("archive.7z", exception.Message);
    }
}