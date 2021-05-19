namespace DotNet.Testcontainers.Networks
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Networks.Configurations;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerNetwork" />
  internal sealed class NewDockerNetwork : IDockerNetwork
  {
    private readonly IDockerNetworkOperations client;

    private readonly ITestcontainersNetworkConfiguration configuration;

    [NotNull]
    private NetworkResponse network = new NetworkResponse();

    public NewDockerNetwork(ITestcontainersNetworkConfiguration configuration)
    {
      this.client = new DockerNetworkOperations(configuration.Endpoint);
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
