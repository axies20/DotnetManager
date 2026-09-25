using System.Net;
using System.Security.Cryptography;
using DotnetManager.Exception.Installation;
using DotnetManager.Installation.Models.Downloads;
using DotnetManager.Installation.Services.Downloads;

namespace DotnetManager.UnitTests.SdkManagement;

public class DotnetDownloaderTests
{
    [Fact]
    public async Task DownloadAsyncWritesFileWhenHashMatches()
    {
        var payload = "downloaded payload"u8.ToArray();
        var fileName = $"dotnet-manager-{Guid.NewGuid():N}.tar.gz";
        var source = new DotnetDownloadSource(
            new Uri("https://example.test/archive"), fileName,
            Convert.ToHexString(SHA512.HashData(payload)));
        var downloader = CreateDownloader(payload);

        var result = await downloader.DownloadAsync(source, CancellationToken.None);

        try
        {
            Assert.Equal(payload, await File.ReadAllBytesAsync(result.FilePath,
                CancellationToken.None));
        }
        finally
        {
            File.Delete(result.FilePath);
        }
    }

    [Fact]
    public async Task DownloadAsyncDeletesFileAndThrowsWhenHashDoesNotMatch()
    {
        var fileName = $"dotnet-manager-{Guid.NewGuid():N}.tar.gz";
        var filePath = Path.Combine(Path.GetTempPath(), fileName);
        var source = new DotnetDownloadSource(
            new Uri("https://example.test/archive"), fileName, "BAD-HASH");
        var downloader = CreateDownloader("payload"u8.ToArray());

        await Assert.ThrowsAsync<DownloadHashMismatchException>(() =>
            downloader.DownloadAsync(source, CancellationToken.None));

        Assert.False(File.Exists(filePath));
    }

    private static DotnetDownloader CreateDownloader(byte[] payload)
    {
        return new DotnetDownloader(new HttpClient(new StubHandler(payload)));
    }

    private sealed class StubHandler(byte[] payload) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(payload)
            });
        }
    }
}