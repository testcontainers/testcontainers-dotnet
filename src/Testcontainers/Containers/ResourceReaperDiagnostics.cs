namespace DotNet.Testcontainers.Containers
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Net.NetworkInformation;
  using System.Text.Json;
  using System.Threading;
  using Docker.DotNet.Models;

  internal sealed class ResourceReaperDiagnostics
  {
    private int connectionAttempts;

    public int ConnectionAttempts
      => this.connectionAttempts;

#pragma warning disable CA1822

    public IReadOnlyDictionary<string, IEnumerable<string>> LocalNetworkInterfaces
      => NetworkInterface.GetAllNetworkInterfaces()
        .ToDictionary(networkInterface => networkInterface.Id, networkInterface => networkInterface.GetIPProperties().UnicastAddresses.Select(unicastAddress => unicastAddress.Address.ToString()));

#pragma warning restore CA1822

    public string ExpectedHost { get; set; }

    public ushort ExpectedPort { get; set; }

    public ContainerInspectResponse ContainerInspection { get; set; }

    public void IncrementConnectionAttempts()
    {
      Interlocked.Increment(ref this.connectionAttempts);
    }

    public override string ToString()
    {
      var options = new JsonSerializerOptions();
      options.WriteIndented = true;
      return JsonSerializer.Serialize(this, options);
    }
  }
}
