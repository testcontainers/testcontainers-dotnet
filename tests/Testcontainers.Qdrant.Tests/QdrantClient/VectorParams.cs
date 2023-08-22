using System.Text.Json.Serialization;

namespace Testcontainers.Qdrant.Tests.QdrantClient;

public class VectorParams
{
  /// <summary>
  /// 
  /// </summary>
  [JsonPropertyName("size")] public int Size { get; set; }
  /// <summary>
  /// 
  /// </summary>
  [JsonPropertyName("distance")] public string Distance { get; set; }
  /// <summary>
  /// 
  /// </summary>
  [JsonPropertyName("on_disk")] public bool? OnDisk { get; set; }
  /// <summary>
  /// 
  /// </summary>
  [JsonPropertyName("hnsw_config")] public HnswConfig HnswConfig { get; set; }

  [JsonPropertyName("quantization_config")] public ScalarQuantization QuantizationConfig { get; set; }

  [JsonPropertyName("shard_number")] public int? ShardNumber { get; set; } = null;

  [JsonPropertyName("replication_factor")] public int? ReplicationFactor { get; set; } = null;

  [JsonPropertyName("write_consistency_factor")] public int? WriteConsistencyFactor { get; set; } = null;

  /// <summary>
  /// 
  /// </summary>
  [JsonPropertyName("on_disk_payload")] public bool? OnDiskPayload { get; set; } = null;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="size"></param>
  /// <param name="distance"></param>
  /// <param name="onDisk"></param>
  public VectorParams(int size = 1, string distance = "Cosine", bool? onDisk = null)
  {
    Size = size;
    Distance = distance;
    OnDisk = onDisk;
  }
}
