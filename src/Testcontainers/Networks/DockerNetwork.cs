namespace DotNet.Testcontainers.Networks
{
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="INetwork" />
  [PublicAPI]
  internal sealed class DockerNetwork : Resource, INetwork
  {
    private readonly IDockerNetworkOperations dockerNetworkOperations;

    private readonly INetworkConfiguration configuration;

    private NetworkResponse network = new NetworkResponse();

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerNetwork" /> class.
    /// </summary>
    /// <param name="configuration">The network configuration.</param>
    /// <param name="logger">The logger.</param>
    public DockerNetwork(INetworkConfiguration configuration, ILogger logger)
    {
      this.dockerNetworkOperations = new DockerNetworkOperations(configuration.SessionId, configuration.DockerEndpointAuthConfig, logger);
      this.configuration = configuration;
    }

    /// <inheritdoc />
    public string Name
    {
      get
      {
        this.ThrowIfResourceNotFound();
        return this.network.Name;
      }
    }

    /// <inheritdoc />
    public async Task CreateAsync(CancellationToken ct = default)
    {
      var id = await this.dockerNetworkOperations.CreateAsync(this.configuration, ct)
        .ConfigureAwait(false);

      this.network = await this.dockerNetworkOperations.ByIdAsync(id, ct)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(CancellationToken ct = default)
    {
      await this.dockerNetworkOperations.DeleteAsync(this.network.ID, ct)
        .ConfigureAwait(false);

      this.network = new NetworkResponse();
    }

    /// <inheritdoc />
    protected override bool Exists()
    {
      return !string.IsNullOrEmpty(this.network.ID);
    }
  }
}
