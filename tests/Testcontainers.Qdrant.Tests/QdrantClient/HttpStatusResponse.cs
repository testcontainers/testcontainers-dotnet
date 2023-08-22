using System.Text.Json.Serialization;

namespace Testcontainers.Qdrant.Tests.QdrantClient;

public class HttpStatusResponse
{
  [JsonPropertyName("status")]
  public HttpErrorResponse Status { get; init; }
  [JsonPropertyName("time")]
  public double Time { get; init; }
}
