namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Networks.Configurations;
  using DotNet.Testcontainers.Services;
  using Microsoft.Extensions.Logging;

  internal sealed class DockerNetworkOperations : DockerApiClient, IDockerNetworkOperations
  {
    private static readonly ILogger<DockerNetworkOperations> Logger = TestcontainersHostService.GetLogger<DockerNetworkOperations>();

    public DockerNetworkOperations(Uri endpoint) : base(endpoint)
    {
    }

    public async Task<IEnumerable<NetworkResponse>> GetAllAsync(CancellationToken ct = default)
    {
      return (await this.Docker.Networks.ListNetworksAsync(new NetworksListParameters(), ct)
        .ConfigureAwait(false)).ToArray();
    }

    public Task<IEnumerable<NetworkResponse>> GetOrphanedObjects(CancellationToken ct = default)
    {
      IEnumerable<NetworkResponse> networks = Array.Empty<NetworkResponse>();
      return Task.FromResult(networks);
    }

    public async Task<NetworkResponse> ByIdAsync(string id, CancellationToken ct = default)
    {
      return (await this.GetAllAsync(ct)
        .ConfigureAwait(false)).FirstOrDefault(image => image.ID.Equals(id));
    }

    public Task<NetworkResponse> ByNameAsync(string name, CancellationToken ct = default)
    {
      return this.ByPropertyAsync("name", name, ct);
    }

    public async Task<NetworkResponse> ByPropertyAsync(string property, string value, CancellationToken ct = default)
    {
      var filters = new FilterByProperty { { property, value } };
      return (await this.Docker.Networks.ListNetworksAsync(new NetworksListParameters { Filters = filters }, ct)
        .ConfigureAwait(false)).FirstOrDefault();
    }

    public async Task<bool> ExistsWithIdAsync(string id, CancellationToken ct = default)
    {
      return await this.ByIdAsync(id, ct)
        .ConfigureAwait(false) != null;
    }

    public async Task<bool> ExistsWithNameAsync(string name, CancellationToken ct = default)
    {
      return await this.ByNameAsync(name, ct)
        .ConfigureAwait(false) != null;
    }

    public async Task<string> CreateAsync(ITestcontainersNetworkConfiguration configuration, CancellationToken ct = default)
    {
      var id = (await this.Docker.Networks.CreateNetworkAsync(new NetworksCreateParameters { Name = configuration.Name, Driver = configuration.Driver.Value }, ct)
        .ConfigureAwait(false)).ID;

      Logger.LogInformation("Network {id} created", id);

      return id;
    }

    public Task DeleteAsync(string id, CancellationToken ct = default)
    {
      Logger.LogInformation("Deleting network {id}", id);
      return this.Docker.Networks.DeleteNetworkAsync(id, ct);
    }
  }
}
