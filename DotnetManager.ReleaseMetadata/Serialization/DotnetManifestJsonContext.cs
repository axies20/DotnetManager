using System.Text.Json.Serialization;
using DotnetManager.ReleaseMetadata.Contracts.Index;
using DotnetManager.ReleaseMetadata.Contracts.Releases;

namespace DotnetManager.ReleaseMetadata.Serialization;

[JsonSerializable(typeof(RawRootIndex))]
[JsonSerializable(typeof(RawReleasesRoot))]
internal partial class DotnetManifestJsonContext : JsonSerializerContext;