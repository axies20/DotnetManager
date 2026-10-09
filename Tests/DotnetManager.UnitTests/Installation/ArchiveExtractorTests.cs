using DotnetManager.Installation.Services.Archives;
using Microsoft.Extensions.Logging.Abstractions;

namespace DotnetManager.UnitTests.Installation;

public class ArchiveExtractorTests
{
    [Fact]
    public async Task ExtractAsyncRejectsUnknownArchiveFormat()
    {
        var exception = await Assert.ThrowsAsync<NotSupportedException>(() =>
            new ArchiveExtractor(NullLogger<ArchiveExtractor>.Instance)
                .ExtractAsync("archive.7z", "destination", CancellationToken.None));

        Assert.Contains("archive.7z", exception.Message);
    }
}
