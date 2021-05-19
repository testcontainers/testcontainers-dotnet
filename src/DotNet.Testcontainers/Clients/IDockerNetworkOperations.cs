namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;

  internal interface IDockerNetworkOperations
  {
    Task<NetworksCreateResponse> CreateAsync(NetworksCreateParameters createParameters, CancellationToken ct);
    Task RemoveAsync(string id, CancellationToken ct);
  }
}
