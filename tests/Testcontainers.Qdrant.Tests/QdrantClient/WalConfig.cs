using System.Text.Json.Serialization;

namespace Testcontainers.Qdrant.Tests.QdrantClient;

public class WalConfig
{
    [JsonPropertyName("wal_capacity_mb")] public int? WalCapacityMb { get; set; } = null;
    [JsonPropertyName("wal_segments_ahead")] public int? WalSegmentsAhead { get; set; } = null;
}