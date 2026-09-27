using System.Diagnostics;
using System.Net.Http.Json;
using DotnetManager.Configuration;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Mapping;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DotnetManifestJsonContext = DotnetManager.ReleaseMetadata.Serialization.DotnetManifestJsonContext;

namespace DotnetManager.ReleaseMetadata.Services;

public class SdkManifestProvider : ISdkManifestProviderService
{
    private readonly HttpClient _httpClient;
    private readonly DotnetManagerOptions _options;
    private readonly ILogger<SdkManifestProvider> _logger;

    public SdkManifestProvider(IOptions<DotnetManagerOptions> options,
        HttpClient httpClient,
        ILogger<SdkManifestProvider> logger)
    {
        _options = options.Value;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<SdkReleaseIndex> GetReleaseIndexAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Fetching .NET release index from {ReleaseIndexUrl}",
            _options.ReleaseIndexUrl);
        using var response = await _httpClient.GetAsync(_options.ReleaseIndexUrl, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync(DotnetManifestJsonContext.Default.RawRootIndex,
            cancellationToken);

        if (result is null)
            throw new InvalidDataException("The .NET release index response was null.");

        var index = SdkReleaseIndexMapper.Map(result);
        _logger.LogInformation(
            "Fetched .NET release index with {ChannelCount} channel(s) in {ElapsedSeconds:F1} seconds",
            index.Releases.Count, stopwatch.Elapsed.TotalSeconds);
        return index;
    }

    public async Task<SdkReleaseManifest> GetReleasesAsync(Uri manifestUri, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Fetching .NET release manifest from {ManifestUri}", manifestUri);
        using var response = await _httpClient.GetAsync(manifestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync(DotnetManifestJsonContext.Default.RawReleasesRoot,
            cancellationToken);
        if (result is null)
            throw new InvalidDataException("The .NET release manifest response was null.");

        var manifest = SdkReleaseManifestMapper.Map(result);
        _logger.LogInformation(
            "Fetched release manifest with {ReleaseCount} release(s) in {ElapsedSeconds:F1} seconds",
            manifest.Releases.Count, stopwatch.Elapsed.TotalSeconds);
        return manifest;
    }
}