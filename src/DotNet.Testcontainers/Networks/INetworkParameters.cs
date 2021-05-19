namespace DotNet.Testcontainers.Networks
{
  using System.Collections.Generic;

  public interface INetworkParameters
  {
    public string Name { get; }

    public string Driver { get; }

    public IDictionary<string, string> Labels { get; }

  }

  public class NetworkParameters : INetworkParameters
  {
    public NetworkParameters(string name, string driver = "bridge", IDictionary<string, string> labels = null)
    {
      this.Name = name;
      this.Driver = driver;
      this.Labels = labels;
    }

    public string Name { get; }
    public string Driver { get; }
    public IDictionary<string, string> Labels { get; }
  }
}
