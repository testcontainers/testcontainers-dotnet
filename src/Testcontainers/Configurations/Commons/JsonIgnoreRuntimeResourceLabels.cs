namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json;
  using System.Text.Json.Serialization;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers;

  internal sealed class JsonIgnoreRuntimeResourceLabels : JsonConverter<IReadOnlyDictionary<string, string>>
  {
    private static readonly ISet<string> IgnoreLabels = new HashSet<string> { ResourceReaper.ResourceReaperSessionLabel, TestcontainersClient.TestcontainersVersionLabel, TestcontainersClient.TestcontainersSessionIdLabel };

    public override bool CanConvert(Type typeToConvert)
    {
      return typeof(IEnumerable<KeyValuePair<string, string>>).IsAssignableFrom(typeToConvert);
    }

    public override IReadOnlyDictionary<string, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      return JsonSerializer.Deserialize<IReadOnlyDictionary<string, string>>(ref reader);
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyDictionary<string, string> value, JsonSerializerOptions options)
    {
      var labels = value.Where(label => !IgnoreLabels.Contains(label.Key)).ToDictionary(label => label.Key, label => label.Value);

      writer.WriteStartObject();

      foreach (var label in labels)
      {
        writer.WriteString(label.Key, label.Value);
      }

      writer.WriteEndObject();
    }
  }
}
