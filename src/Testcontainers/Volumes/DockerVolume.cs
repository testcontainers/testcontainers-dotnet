namespace DotNet.Testcontainers.Volumes
{
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="IVolume" />
  [PublicAPI]
  internal sealed class DockerVolume : Resource, IVolume
  {
    private readonly IDockerVolumeOperations dockerVolumeOperations;

    private readonly IVolumeConfiguration configuration;

    private VolumeResponse volume = new VolumeResponse();

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerVolume" /> class.
    /// </summary>
    /// <param name="configuration">The volume configuration.</param>
    /// <param name="logger">The logger.</param>
    public DockerVolume(IVolumeConfiguration configuration, ILogger logger)
    {
      this.dockerVolumeOperations = new DockerVolumeOperations(configuration.SessionId, configuration.DockerEndpointAuthConfig, logger);
      this.configuration = configuration;
    }

    /// <inheritdoc />
    public string Name
    {
      get
      {
        this.ThrowIfResourceNotFound();
        return this.volume.Name;
      }
    }

    /// <inheritdoc />
    public async Task CreateAsync(CancellationToken ct = default)
    {
      var name = await this.dockerVolumeOperations.CreateAsync(this.configuration, ct)
        .ConfigureAwait(false);

      this.volume = await this.dockerVolumeOperations.ByNameAsync(name, ct)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(CancellationToken ct = default)
    {
      await this.dockerVolumeOperations.DeleteAsync(this.Name, ct)
        .ConfigureAwait(false);

      this.volume = new VolumeResponse();
    }

    /// <inheritdoc />
    protected override bool Exists()
    {
      return !string.IsNullOrEmpty(this.volume.Name);
    }
  }
}
