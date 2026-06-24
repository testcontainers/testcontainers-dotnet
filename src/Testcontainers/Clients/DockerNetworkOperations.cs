namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using Microsoft.Extensions.Logging;

  internal sealed class DockerNetworkOperations : DockerApiClient, IDockerNetworkOperations
  {
    public DockerNetworkOperations(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, ILogger logger)
      : base(sessionId, dockerEndpointAuthConfig, logger)
    {
    }

    public async Task<IEnumerable<NetworkResponse>> GetAllAsync(CancellationToken ct = default)
    {
      return await DockerClient.Networks.ListNetworksAsync(new NetworksListParameters(), ct)
        .ConfigureAwait(false);
    }

    public async Task<IEnumerable<NetworkResponse>> GetAllAsync(FilterByProperty filters, CancellationToken ct = default)
    {
      return await DockerClient.Networks.ListNetworksAsync(new NetworksListParameters { Filters = filters }, ct)
        .ConfigureAwait(false);
    }

    public async Task<NetworkResponse> ByIdAsync(string id, CancellationToken ct = default)
    {
      return await DockerClient.Networks.InspectNetworkAsync(id, ct)
        .ConfigureAwait(false);
    }

    public async Task<bool> ExistsWithIdAsync(string id, CancellationToken ct = default)
    {
      try
      {
        _ = await ByIdAsync(id, ct)
          .ConfigureAwait(false);

        return true;
      }
      catch (DockerNetworkNotFoundException)
      {
        return false;
      }
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

      if (configuration.Reuse.HasValue && configuration.Reuse.Value)
      {
        createParameters.Labels.Add(TestcontainersClient.TestcontainersReuseHashLabel, configuration.GetReuseHash());
      }

      if (configuration.ParameterModifiers != null)
      {
        foreach (var parameterModifier in configuration.ParameterModifiers)
        {
          parameterModifier(createParameters);
        }
      }

      var createNetworkResponse = await DockerClient.Networks.CreateNetworkAsync(createParameters, ct)
        .ConfigureAwait(false);

      Logger.DockerNetworkCreated(createNetworkResponse.ID);
      return createNetworkResponse.ID;
    }

    public Task DeleteAsync(string id, CancellationToken ct = default)
    {
      Logger.DeleteDockerNetwork(id);
      return DockerClient.Networks.DeleteNetworkAsync(id, ct);
    }

    public Task ConnectAsync(string networkId, string containerId, CancellationToken ct = default)
    {
      Logger.ConnectToDockerNetwork(networkId, containerId);
      return DockerClient.Networks.ConnectNetworkAsync(networkId, new NetworkConnectParameters { Container = containerId }, ct);
    }
  }
}
