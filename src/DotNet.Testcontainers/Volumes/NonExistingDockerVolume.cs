namespace DotNet.Testcontainers.Volumes
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations.Volumes;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  internal sealed class NonExistingDockerVolume : IDockerVolume
  {
    private readonly IDockerVolumeOperations client;
    private readonly ITestcontainersVolumeConfiguration configuration;

    [NotNull]
    private VolumeResponse volume = new VolumeResponse();

    public NonExistingDockerVolume(ITestcontainersVolumeConfiguration configuration, ILogger logger)
    {
      this.client = new DockerVolumeOperations(configuration.Endpoint, logger);
      this.configuration = configuration;
    }

    /// <inheritdoc />
    public string Name
    {
      get
      {
        this.ThrowIfVolumeHasNotBeenCreated();
        return this.volume.Name;
      }
    }

    /// <inheritdoc />
    public async Task CreateAsync(CancellationToken ct = default)
    {
      this.volume = await this.client.CreateAsync(this.configuration, ct)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(bool? force = null, CancellationToken ct = default)
    {
      await this.client.RemoveAsync(this.Name, force, ct)
        .ConfigureAwait(false);
      this.volume = new VolumeResponse();
    }

    private void ThrowIfVolumeHasNotBeenCreated()
    {
      if (string.IsNullOrEmpty(this.volume.Name))
      {
        throw new InvalidOperationException("Volume has not been created.");
      }
    }
  }
}
