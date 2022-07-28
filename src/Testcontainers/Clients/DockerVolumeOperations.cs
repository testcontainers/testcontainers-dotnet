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
    private readonly ILogger logger;

    public DockerVolumeOperations(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, ILogger logger)
      : base(sessionId, dockerEndpointAuthConfig)
    {
      this.logger = logger;
    }

    public async Task<IEnumerable<VolumeResponse>> GetAllAsync(CancellationToken ct = default)
    {
      return (await this.Docker.Volumes.ListAsync(ct)
        .ConfigureAwait(false)).Volumes.ToArray();
    }

    public Task<VolumeResponse> ByIdAsync(string id, CancellationToken ct = default)
    {
      throw new NotImplementedException();
    }

    public async Task<VolumeResponse> ByNameAsync(string name, CancellationToken ct = default)
    {
      return (await this.GetAllAsync(ct)
        .ConfigureAwait(false)).FirstOrDefault(volume => volume.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public Task<VolumeResponse> ByPropertyAsync(string property, string value, CancellationToken ct = default)
    {
      throw new NotImplementedException();
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

    public async Task<string> CreateAsync(ITestcontainersVolumeConfiguration configuration, CancellationToken ct = default)
    {
      var createParameters = new VolumesCreateParameters
      {
        Name = configuration.Name,
        Labels = configuration.Labels.ToDictionary(item => item.Key, item => item.Value),
      };

      var name = (await this.Docker.Volumes.CreateAsync(createParameters, ct)
        .ConfigureAwait(false)).Name;

      this.logger.DockerVolumeCreated(name);

      return name;
    }

    public Task DeleteAsync(string name, CancellationToken ct = default)
    {
      this.logger.DeleteDockerVolume(name);
      return this.Docker.Volumes.RemoveAsync(name, false, ct);
    }
  }
}
