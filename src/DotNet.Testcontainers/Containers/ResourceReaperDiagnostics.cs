namespace DotNet.Testcontainers.Containers
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Net.NetworkInformation;
  using System.Threading;
  using Docker.DotNet.Models;
  using Newtonsoft.Json;

  public class ResourceReaperDiagnostics
  {
    private int connectionAttempts;

    internal ResourceReaperDiagnostics()
    {
    }

    public int ConnectionAttempts => this.connectionAttempts;

    public string ExpectedHost { get; internal set; }

    public ushort ExpectedPort { get; internal set; }

    public Dictionary<string, string> LocalNetworkInterfaces =>
      NetworkInterface.GetAllNetworkInterfaces()
      .ToDictionary(x => x.Name, x => string.Join(",", x.GetIPProperties().UnicastAddresses.Select(ip => ip.Address.ToString())));

    public ContainerInspectResponse ContainerInspection { get; internal set; }

    public void IncrementConnectionAttempts()
    {
      Interlocked.Increment(ref this.connectionAttempts);
    }

    public override string ToString()
    {
      return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
  }
}
