using System.Net.Http.Json;
using DotnetManager.Abstraction.SDK;
using DotnetManager.Dto.DotnetManifest;
using DotnetManager.Models.Sdk.SdkManifest.ReleaseIndex;
using DotnetManager.Models.Sdk.SdkManifest.ReleaseManifest;
using DotnetManager.Options;
using Microsoft.Extensions.Options;

namespace DotnetManager.Services;

public class SdkManifestProvider : ISdkManifestProvider
{
    private readonly DotnetManagerOptions _options;
    private readonly HttpClient _httpClient;

    public SdkManifestProvider(IOptions<DotnetManagerOptions> options, HttpClient httpClient)
    {
        _options = options.Value;
        _httpClient = httpClient;
    }

    public async Task<SdkReleaseIndex> GetReleaseIndexAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(_options.ReleaseIndexUrl, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync(DotnetManifestJsonContext.Default.RawRootIndex,
                         cancellationToken: cancellationToken)
                     ?? throw new InvalidDataException("The .NET release index response was null.");


        var channels = new List<SdkChannel>();
        foreach (var release in result.Releasesindex)
        {
            var channel = new SdkChannel
            {
                ChannelVersion = release.ChannelVersion,
                LatestSdk = release.LatestSdk,
                SupportPhase = release.SupportPhase,
                ReleasesUri = new Uri(release.ReleasesJson)
            };
            channels.Add(channel);
        }

        return new SdkReleaseIndex(channels);
    }

    public Task<SdkReleaseManifest> GetReleasesAsync(Uri manifestUri, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}