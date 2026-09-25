using System.Net.Http.Json;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Mapping;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;
using DotnetManager.Configuration;
using Microsoft.Extensions.Options;
using DotnetManifestJsonContext = DotnetManager.ReleaseMetadata.Serialization.DotnetManifestJsonContext;

namespace DotnetManager.ReleaseMetadata.Services;

public class SdkManifestProvider : ISdkManifestProviderService
{
    private readonly HttpClient _httpClient;
    private readonly DotnetManagerOptions _options;

    public SdkManifestProvider(IOptions<DotnetManagerOptions> options, HttpClient httpClient)
    {
        _options = options.Value;
        _httpClient = httpClient;
    }

    public async Task<SdkReleaseIndex> GetReleaseIndexAsync(CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(_options.ReleaseIndexUrl, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync(DotnetManifestJsonContext.Default.RawRootIndex,
            cancellationToken);

        if (result is null)
            throw new InvalidDataException("The .NET release index response was null.");

        return SdkReleaseIndexMapper.Map(result);
    }

    public async Task<SdkReleaseManifest> GetReleasesAsync(Uri manifestUri, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(manifestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync(DotnetManifestJsonContext.Default.RawReleasesRoot,
            cancellationToken);
        if (result is null)
            throw new InvalidDataException("The .NET release manifest response was null.");
        return SdkReleaseManifestMapper.Map(result);
    }
}
