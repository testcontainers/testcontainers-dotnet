namespace DotNet.Testcontainers.Networks
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Clients;
  using Docker.DotNet.Models;

  public class TestcontainersNetwork : IDockerNetwork
  {
    private readonly TestcontainersClient client;
    private readonly INetworkParameters networkParameters;

    public TestcontainersNetwork(INetworkParameters networkParameters)
    {
      this.networkParameters = networkParameters;
      this.client = new TestcontainersClient();
    }

    public TestcontainersNetwork(INetworkParameters networkParameters, Uri dockerEndpoint)
    {
      this.networkParameters = networkParameters;
      this.client = new TestcontainersClient(dockerEndpoint);
    }

    public async Task<IStartedNetwork> StartAsync()
    {
      var networkResponse = await this.client.CreateNetworkAsync(new NetworksCreateParameters
      {
        Name = this.networkParameters.Name,
        Driver = this.networkParameters.Driver,
        Labels = this.networkParameters.Labels
      });
      return new StartedNetwork(networkResponse.ID, this.networkParameters, this.client);
    }
  }

  internal class StartedNetwork : IStartedNetwork
  {
    private readonly TestcontainersClient client;
    public string Id { get; }
    public string Name { get; }
    internal StartedNetwork(string id, INetworkParameters networkParameters, TestcontainersClient client)
    {
      this.Id = id;
      this.Name = networkParameters.Name;
      this.client = client;
    }

    public Task StopAsync(CancellationToken ct = default)
    {
      return this.client.RemoveNetworkAsync(this.Id, ct);
    }

    public async ValueTask DisposeAsync()
    {
      await this.StopAsync();
    }
  }

}
