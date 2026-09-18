using System.Text.Json.Serialization;
using DotnetManager.Dto.DotnetManifest.Response.RawIndex;
using DotnetManager.Dto.DotnetManifest.Response.RawReleases;

namespace DotnetManager.Dto.DotnetManifest;

[JsonSerializable(typeof(RawRootIndex))]
[JsonSerializable(typeof(RawReleasesRoot))]
internal partial class DotnetManifestJsonContext : JsonSerializerContext;
