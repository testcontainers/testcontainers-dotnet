namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json;
  using System.Text.Json.Serialization;

  internal class JsonOrderedKeysConverter : JsonConverter<IReadOnlyDictionary<string, string>>
  {
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
      writer.WriteStartObject();

      foreach (var item in value.OrderBy(item => item.Key))
      {
        writer.WriteString(item.Key, item.Value);
      }

      writer.WriteEndObject();
    }
  }
}
