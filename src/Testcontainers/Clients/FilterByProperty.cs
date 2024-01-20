namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Configurations;

  public class FilterByProperty : ConcurrentDictionary<string, IDictionary<string, bool>>
  {
    public FilterByProperty Add(string property, string value)
    {
      var values = GetOrAdd(property, _ => new Dictionary<string, bool>());
      values[value] = true;
      return this;
    }
  }

  public sealed class FilterByReuseHash : FilterByProperty
  {
    public FilterByReuseHash(IContainerConfiguration resourceConfiguration)
      : this(resourceConfiguration.GetReuseHash())
    {
    }

    public FilterByReuseHash(INetworkConfiguration resourceConfiguration)
      : this(resourceConfiguration.GetReuseHash())
    {
    }

    public FilterByReuseHash(IVolumeConfiguration resourceConfiguration)
      : this(resourceConfiguration.GetReuseHash())
    {
    }

    private FilterByReuseHash(string hash)
    {
      Add("label", string.Join("=", TestcontainersClient.TestcontainersReuseHashLabel, hash));
    }
  }
}
