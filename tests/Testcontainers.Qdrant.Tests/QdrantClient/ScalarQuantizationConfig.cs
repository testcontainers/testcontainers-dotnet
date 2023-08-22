using System.Text.Json.Serialization;

namespace Testcontainers.Qdrant.Tests.QdrantClient;

public class ScalarQuantizationConfig
{
    [JsonPropertyName("type")] public string Type { get; set; } = ScalarType.INT8;

    [JsonPropertyName("quantile")] public float Quantile { get; set; } = 0.5f;

    [JsonPropertyName("always_ram")] public bool AlwaysRam { get; set; } = true;
}