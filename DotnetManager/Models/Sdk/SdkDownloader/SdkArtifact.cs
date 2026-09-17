namespace DotnetManager.Models.Sdk.SdkDownloader;

public sealed record SdkArtifact(string Version, Uri DownloadUri, string FileName, string? Hash);