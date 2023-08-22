using System.Text.Json.Serialization;

namespace Testcontainers.Qdrant.Tests.QdrantClient;

public class OptimizersConfig
{
    [JsonPropertyName("deleted_threshold")] public int DeletedThreshold { get; set; } = 0;
    [JsonPropertyName("vacuum_min_vector_number")] public int VacuumMinVectorNumber { get; set; } = 0;
    [JsonPropertyName("default_segment_number")] public int DefaultSegmentNumber { get; set; } = 0;
    [JsonPropertyName("max_segment_size")] public int MaxSegmentSize { get; set; } = 0;
    [JsonPropertyName("memmap_threshold")] public int MemMapThreshold { get; set; } = 0;
    [JsonPropertyName("indexing_threshold")] public int IndexingThreshold { get; set; } = 0;
    [JsonPropertyName("flush_interval_sec")] public int FlushIntervalSec { get; set; } = 0;
    [JsonPropertyName("max_optimization_threads")] public int MaxOptimizationThreads { get; set; } = 0;

}