using System.Text.Json.Serialization;

namespace Testcontainers.Qdrant.Tests.QdrantClient;

public class HnswConfig
{
    [JsonPropertyName("m")] public int M { get; set; } = 0;

    [JsonPropertyName("ef_construct")] public int EfConstruct { get; set; } = 4;

    [JsonPropertyName("full_scan_threshold")]
    public int FullScanThreshold { get; set; } = 10;

    [JsonPropertyName("max_indexing_threads")]
    public int MaxIndexingThreads { get; set; } = 0;

    [JsonPropertyName("on_disk")] public bool OnDisk { get; set; } = true;

    [JsonPropertyName("payload_m")] public int PayloadM { get; set; } = 0;
}