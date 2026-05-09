namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json;
  using System.Text.Json.Serialization;

  /// <summary>
  /// Excludes the port-forwarding extra host entry (host.testcontainers.internal)
  /// from the reuse hash, its IP changes every test session, which would otherwise
  /// make identical configurations appear different.
  /// </summary>
  internal sealed class JsonIgnoreRuntimeExtraHosts : JsonConverter<IEnumerable<string>>
  {
    private const string PortForwardingHostEntry = "host.testcontainers.internal:";

    public override IEnumerable<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      return JsonSerializer.Deserialize<IEnumerable<string>>(ref reader);
    }

    public override void Write(Utf8JsonWriter writer, IEnumerable<string> value, JsonSerializerOptions options)
    {
      var extraHosts = value.Where(host => !host.StartsWith(PortForwardingHostEntry, StringComparison.OrdinalIgnoreCase)).OrderBy(host => host);
      JsonSerializer.Serialize(writer, extraHosts, options);
    }
  }
}
