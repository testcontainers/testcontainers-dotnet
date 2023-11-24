using DotNet.Testcontainers.Clients;
using DotNet.Testcontainers.Containers;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNet.Testcontainers.Configurations
{
  internal class ExcludeDynamicLabelsConverter : JsonConverter<IReadOnlyDictionary<string, string>>
  {
    public override bool CanConvert(Type typeToConvert)
    {
      return true;
    }

    public override IReadOnlyDictionary<string, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      return JsonSerializer.Deserialize<IReadOnlyDictionary<string, string>>(ref reader);
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyDictionary<string, string> value, JsonSerializerOptions options)
    {
      var labels = JsonSerializer.SerializeToNode(value).AsObject();

      labels.Remove(TestcontainersClient.TestcontainersSessionIdLabel);
      labels.Remove(ResourceReaper.ResourceReaperSessionLabel);
      labels.Remove(TestcontainersClient.TestcontainersReuseHashLabel);

      labels.WriteTo(writer);
    }
  }
}
