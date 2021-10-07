namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations.Volumes;
  using Microsoft.Extensions.Logging;

  internal sealed class DockerVolumeOperations : DockerApiClient, IDockerVolumeOperations
  {
    private readonly ILogger logger;

    public DockerVolumeOperations(Uri endpoint, ILogger logger)
      : base(endpoint)
    {
      this.logger = logger;
    }

    public async Task<VolumeResponse> CreateAsync(ITestcontainersVolumeConfiguration configuration, CancellationToken ct = default)
    {
      var createParameters = new VolumesCreateParameters
      {
        Name = configuration.Name,
      };

      var response = await this.Docker.Volumes.CreateAsync(createParameters, ct)
        .ConfigureAwait(false);

      this.logger.LogInformation("Volume {Name} created", response.Name);

      return response;
    }

    public Task RemoveAsync(string name, bool? force = null, CancellationToken ct = default)
    {
      this.logger.LogInformation("Deleting volume {Name}", name);
      return this.Docker.Volumes.RemoveAsync(name, force, ct);
    }
  }
}
