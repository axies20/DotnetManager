using System.Text.Json.Serialization;
using DotnetManager.Dto.DotNetManifest.Response.RawIndex;
using DotnetManager.Dto.DotNetManifest.Response.RawReleases;

namespace DotnetManager.Dto.DotNetManifest;

[JsonSerializable(typeof(RawRootIndex))]
[JsonSerializable(typeof(RawReleasesRoot))]
internal partial class DotnetManifestJsonContext : JsonSerializerContext;