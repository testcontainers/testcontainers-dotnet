namespace DotNet.Testcontainers.Networks
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="IDockerNetwork" />
  internal sealed class NonExistingDockerNetwork : IDockerNetwork
  {
    private readonly IDockerNetworkOperations client;

    private readonly ITestcontainersNetworkConfiguration configuration;

    [NotNull]
    private NetworkResponse network = new NetworkResponse();

    /// <summary>
    /// Initializes a new instance of the <see cref="NonExistingDockerNetwork" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    public NonExistingDockerNetwork(ITestcontainersNetworkConfiguration configuration, ILogger logger)
    {
      this.client = new DockerNetworkOperations(configuration.SessionId, configuration.DockerEndpointAuthConfig, logger);
      this.configuration = configuration;
    }

    /// <inheritdoc />
    public string Id
    {
      get
      {
        this.ThrowIfNetworkHasNotBeenCreated();
        return this.network.ID;
      }
    }

    /// <inheritdoc />
    public string Name
    {
      get
      {
        this.ThrowIfNetworkHasNotBeenCreated();
        return this.network.Name;
      }
    }

    /// <inheritdoc />
    public async Task CreateAsync(CancellationToken ct = default)
    {
      var id = await this.client.CreateAsync(this.configuration, ct)
        .ConfigureAwait(false);

      this.network = await this.client.ByIdAsync(id, ct)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(CancellationToken ct = default)
    {
      await this.client.DeleteAsync(this.Id, ct)
        .ConfigureAwait(false);
      this.network = new NetworkResponse();
    }

    private void ThrowIfNetworkHasNotBeenCreated()
    {
      if (string.IsNullOrEmpty(this.network.ID))
      {
        throw new InvalidOperationException("Network has not been created.");
      }
    }
  }
}
