using System.Text.Json.Serialization;

namespace Testcontainers.Qdrant.Tests.QdrantClient;

public class CreateCollectionWithVectorRequest
{
  /// <summary>
  /// 
  /// </summary>
  [JsonPropertyName("hnsw_config")] public HnswConfig HnswConfig { get; set; } = default!;
  /// <summary>
  /// Create collection from another collection
  /// <a href="https://qdrant.tech/documentation/concepts/collections/">Create collection from another collection</a>
  /// </summary>
  [JsonPropertyName("init_from")] public string InitFrom { get; set; } = default!;
  /// <summary>
  /// 
  /// </summary>
  [JsonPropertyName("quantization_config")] public string QuantizationConfig { get; set; } = default!;
  /// <summary>
  /// 
  /// </summary>
  [JsonPropertyName("wal_config")] public WalConfig WalConfig { get; set; } = default!;
  /// <summary>
  /// 
  /// </summary>
  [JsonPropertyName("optimizers_config")] public OptimizersConfig OptimizersConfig { get; set; } = default!;

  /// <summary>
  /// Vectors collection name
  /// <a href="https://qdrant.tech/documentation/reference/collections.html#collection-name">Collection name</a>
  /// </summary>
  [JsonPropertyName("vectors")] public VectorParams Vectors { get; init; } = default!;
}
