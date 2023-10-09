namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using Microsoft.Extensions.Logging;

  internal sealed class DockerNetworkOperations : DockerApiClient, IDockerNetworkOperations
  {
    private readonly ILogger _logger;

    public DockerNetworkOperations(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, ILogger logger)
      : base(sessionId, dockerEndpointAuthConfig)
    {
      _logger = logger;
    }

    public async Task<IEnumerable<NetworkResponse>> GetAllAsync(CancellationToken ct = default)
    {
      return await Docker.Networks.ListNetworksAsync(new NetworksListParameters(), ct)
        .ConfigureAwait(false);
    }

    public async Task<IEnumerable<NetworkResponse>> GetAllAsync(FilterByProperty filters, CancellationToken ct = default)
    {
      return await Docker.Networks.ListNetworksAsync(new NetworksListParameters { Filters = filters }, ct)
        .ConfigureAwait(false);
    }

    public Task<NetworkResponse> ByIdAsync(string id, CancellationToken ct = default)
    {
      return ByPropertyAsync("id", id, ct);
    }

    public Task<NetworkResponse> ByNameAsync(string name, CancellationToken ct = default)
    {
      return ByPropertyAsync("name", name, ct);
    }

    public Task<NetworkResponse> ByPropertyAsync(string property, string value, CancellationToken ct = default)
    {
      return Docker.Networks.InspectNetworkAsync(value, ct);
    }

    public async Task<bool> ExistsWithIdAsync(string id, CancellationToken ct = default)
    {
      return await ByIdAsync(id, ct)
        .ConfigureAwait(false) != null;
    }

    public async Task<bool> ExistsWithNameAsync(string name, CancellationToken ct = default)
    {
      return await ByNameAsync(name, ct)
        .ConfigureAwait(false) != null;
    }

    public async Task<string> CreateAsync(INetworkConfiguration configuration, CancellationToken ct = default)
    {
      var createParameters = new NetworksCreateParameters
      {
        Name = configuration.Name,
        Driver = configuration.Driver.Value,
        Options = configuration.Options.ToDictionary(item => item.Key, item => item.Value),
        Labels = configuration.Labels.ToDictionary(item => item.Key, item => item.Value),
      };

      if (configuration.ParameterModifiers != null)
      {
        foreach (var parameterModifier in configuration.ParameterModifiers)
        {
          parameterModifier(createParameters);
        }
      }

      var createNetworkResponse = await Docker.Networks.CreateNetworkAsync(createParameters, ct)
        .ConfigureAwait(false);

      _logger.DockerNetworkCreated(createNetworkResponse.ID);
      return createNetworkResponse.ID;
    }

    public Task DeleteAsync(string id, CancellationToken ct = default)
    {
      _logger.DeleteDockerNetwork(id);
      return Docker.Networks.DeleteNetworkAsync(id, ct);
    }

    public Task ConnectAsync(string networkId, string containerId, CancellationToken ct = default)
    {
      _logger.ConnectToDockerNetwork(networkId, containerId);
      return Docker.Networks.ConnectNetworkAsync(networkId, new NetworkConnectParameters { Container = containerId }, ct);
    }
  }
}
