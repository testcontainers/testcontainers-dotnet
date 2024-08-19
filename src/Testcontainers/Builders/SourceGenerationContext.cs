using System.Text.Json.Serialization;

namespace DotNet.Testcontainers.Builders
{
  [JsonSerializable(typeof(DockerConfig.DockerContextMeta))]
  internal partial class SourceGenerationContext : JsonSerializerContext;
}
