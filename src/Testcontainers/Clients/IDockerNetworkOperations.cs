namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;

  internal interface IDockerNetworkOperations : IHasListOperations<NetworkResponse>
  {
    Task<string> CreateAsync(ITestcontainersNetworkConfiguration configuration, CancellationToken ct = default);

    Task DeleteAsync(string id, CancellationToken ct = default);
  }
}
