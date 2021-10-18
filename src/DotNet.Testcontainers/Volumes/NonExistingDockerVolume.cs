namespace DotNet.Testcontainers.Volumes
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations.Volumes;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="IDockerVolume" />
  internal sealed class NonExistingDockerVolume : IDockerVolume
  {
    private readonly IDockerVolumeOperations client;

    private readonly ITestcontainersVolumeConfiguration configuration;

    [NotNull]
    private VolumeResponse volume = new VolumeResponse();

    /// <summary>
    /// Initializes a new instance of the <see cref="NonExistingDockerVolume" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
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
    public IDictionary<string, string> Labels
    {
      get
      {
        this.ThrowIfVolumeHasNotBeenCreated();
        return this.volume.Labels;
      }
    }

    /// <inheritdoc />
    public async Task CreateAsync(CancellationToken ct = default)
    {
      this.volume = await this.client.CreateAsync(this.configuration, ct)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(CancellationToken ct = default)
    {
      await this.client.RemoveAsync(this.Name, ct)
        .ConfigureAwait(false);
      this.volume = new VolumeResponse();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
      if (string.IsNullOrEmpty(this.volume.Name))
      {
        return default;
      }

      return new ValueTask(this.DeleteAsync());
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
