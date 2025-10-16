namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers;

  internal sealed class JsonIgnoreRuntimeResourceLabels : JsonOrderedKeysConverter
  {
    private static readonly ISet<string> IgnoreLabels = new HashSet<string> { ResourceReaper.ResourceReaperSessionLabel, TestcontainersClient.TestcontainersVersionLabel, TestcontainersClient.TestcontainersSessionIdLabel };

    public override void Write(Utf8JsonWriter writer, IReadOnlyDictionary<string, string> value, JsonSerializerOptions options)
    {
      var labels = value.Where(label => !IgnoreLabels.Contains(label.Key)).ToDictionary(label => label.Key, label => label.Value);

      base.Write(writer, labels, options);
    }
  }
}
