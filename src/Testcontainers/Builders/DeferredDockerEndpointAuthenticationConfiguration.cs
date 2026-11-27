namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet;
  using Docker.DotNet.Handler.Abstractions;
  using DotNet.Testcontainers.Configurations;

  /// <summary>
  /// The default Docker endpoint of a builder. It does not discover the endpoint when
  /// it is created. Resources resolve it with <see cref="EnsureResolvedAsync" /> before
  /// they use it. Reading a member earlier discovers the endpoint synchronously.
  /// </summary>
  internal sealed class DeferredDockerEndpointAuthenticationConfiguration : IDockerEndpointAuthenticationConfiguration
  {
    private readonly DockerEndpointDiscovery _discovery;

    private readonly Lazy<IDockerEndpointAuthenticationConfiguration> _resolved;

    public DeferredDockerEndpointAuthenticationConfiguration(DockerEndpointDiscovery discovery)
    {
      _discovery = discovery;

      // Providers such as mTLS load certificate files in GetAuthConfig, so it runs once.
      // A missing endpoint is not cached here; the discovery already caches its result.
      _resolved = new Lazy<IDockerEndpointAuthenticationConfiguration>(Resolve, LazyThreadSafetyMode.PublicationOnly);
    }

    public static IDockerEndpointAuthenticationConfiguration Default { get; }
      = new DeferredDockerEndpointAuthenticationConfiguration(DockerEndpointDiscovery.Default);

    /// <inheritdoc />
    public Version Version => GetAuthConfig().Version;

    /// <inheritdoc />
    public Uri Endpoint => GetAuthConfig().Endpoint;

    /// <inheritdoc />
    public IAuthProvider AuthProvider => GetAuthConfig().AuthProvider;

    /// <inheritdoc />
    public DockerClientBuilder GetDockerClientBuilder(Guid sessionId = default)
    {
      return GetAuthConfig().GetDockerClientBuilder(sessionId);
    }

    /// <summary>
    /// Resolves the Docker endpoint of a deferred configuration. Explicit
    /// configurations are returned as they are.
    /// </summary>
    /// <param name="configuration">The Docker endpoint authentication configuration.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The resolved Docker endpoint authentication configuration.</returns>
    /// <exception cref="DockerUnavailableException">Thrown when no Docker endpoint is available.</exception>
    public static async Task<IDockerEndpointAuthenticationConfiguration> EnsureResolvedAsync(IDockerEndpointAuthenticationConfiguration configuration, CancellationToken ct = default)
    {
      if (!(configuration is DeferredDockerEndpointAuthenticationConfiguration deferred))
      {
        return configuration;
      }

      // After this, the discovery has completed and GetAuthConfig does not block.
      _ = await deferred._discovery.GetProviderAsync(ct)
        .ConfigureAwait(false);

      return deferred.GetAuthConfig();
    }

    private IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      return _resolved.Value;
    }

    private IDockerEndpointAuthenticationConfiguration Resolve()
    {
      return _discovery.GetProvider()?.GetAuthConfig() ?? throw _discovery.CreateDockerUnavailableException();
    }
  }
}
