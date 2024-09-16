namespace DotNet.Testcontainers.Builders
{
  using System.Text.Json.Serialization;

  [JsonSerializable(typeof(DockerConfig.DockerContextMeta))]
  internal partial class SourceGenerationContext : JsonSerializerContext;
}
