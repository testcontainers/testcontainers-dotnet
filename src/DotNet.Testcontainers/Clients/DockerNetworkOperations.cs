namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;

  internal class DockerNetworkOperations : DockerApiClient, IDockerNetworkOperations
  {
    public DockerNetworkOperations(Uri endpoint) : base(endpoint)
    {
    }

    public Task<NetworksCreateResponse> CreateAsync(NetworksCreateParameters createParameters, CancellationToken ct = default)
    {
      return this.Docker.Networks.CreateNetworkAsync(createParameters);
    }

    public Task RemoveAsync(string id, CancellationToken ct = default)
    {
      return this.Docker.Networks.DeleteNetworkAsync(id, ct);
    }
  }
}
