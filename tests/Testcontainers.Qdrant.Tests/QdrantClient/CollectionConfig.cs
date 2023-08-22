using System.Text.Json.Serialization;

namespace Testcontainers.Qdrant.Tests.QdrantClient;

public class CollectionConfig
{
    [JsonPropertyName("params")] public CollectionParams Params { get; set; }
}