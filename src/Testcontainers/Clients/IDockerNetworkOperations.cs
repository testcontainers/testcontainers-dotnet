namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;

  internal interface IDockerNetworkOperations : IHasListOperations<NetworkResponse, NetworkResponse>
  {
    Task<string> CreateAsync(INetworkConfiguration configuration, CancellationToken ct = default);

    Task DeleteAsync(string id, CancellationToken ct = default);

    Task ConnectAsync(string networkId, string containerId, CancellationToken ct = default);
  }
}
