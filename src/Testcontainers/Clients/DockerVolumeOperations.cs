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

  internal sealed class DockerVolumeOperations : DockerApiClient, IDockerVolumeOperations
  {
    private readonly ILogger _logger;

    public DockerVolumeOperations(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, ILogger logger)
      : base(sessionId, dockerEndpointAuthConfig)
    {
      _logger = logger;
    }

    public async Task<IEnumerable<VolumeResponse>> GetAllAsync(CancellationToken ct = default)
    {
      return (await Docker.Volumes.ListAsync(ct)
        .ConfigureAwait(false)).Volumes.ToArray();
    }

    public Task<VolumeResponse> ByIdAsync(string id, CancellationToken ct = default)
    {
      return Task.FromResult<VolumeResponse>(null);
    }

    public Task<VolumeResponse> ByNameAsync(string name, CancellationToken ct = default)
    {
      return ByPropertyAsync("name", name, ct);
    }

    public async Task<VolumeResponse> ByPropertyAsync(string property, string value, CancellationToken ct = default)
    {
      var filters = new FilterByProperty { { property, value } };
      return (await Docker.Volumes.ListAsync(new VolumesListParameters { Filters = filters }, ct)
        .ConfigureAwait(false)).Volumes.FirstOrDefault();
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

    public async Task<string> CreateAsync(IVolumeConfiguration configuration, CancellationToken ct = default)
    {
      var createParameters = new VolumesCreateParameters
      {
        Name = configuration.Name,
        Labels = configuration.Labels.ToDictionary(item => item.Key, item => item.Value),
      };

      if (configuration.ParameterModifiers != null)
      {
        foreach (var parameterModifier in configuration.ParameterModifiers)
        {
          parameterModifier(createParameters);
        }
      }

      var createVolumeResponse = await Docker.Volumes.CreateAsync(createParameters, ct)
        .ConfigureAwait(false);

      _logger.DockerVolumeCreated(createVolumeResponse.Name);
      return createVolumeResponse.Name;
    }

    public Task DeleteAsync(string name, CancellationToken ct = default)
    {
      _logger.DeleteDockerVolume(name);
      return Docker.Volumes.RemoveAsync(name, false, ct);
    }
  }
}
