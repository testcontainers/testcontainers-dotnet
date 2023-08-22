using System.Text.Json.Serialization;

namespace Testcontainers.Qdrant.Tests.QdrantClient;

public class HttpErrorResponse
{
  [JsonPropertyName("error")]
  public string Error { get; init; }
}
