namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;

  internal sealed class DefaultLabels : ReadOnlyDictionary<string, string>
  {
    public DefaultLabels() : base(new Dictionary<string, string>
    {
      { TestcontainersClient.TestcontainersLabel, "true" },
      { TestcontainersClient.TestcontainersCleanUpLabel, "true" }
    })
    {
    }
  }
}
