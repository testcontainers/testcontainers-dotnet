namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class DockerEndpointDiscoveryTest
  {
    private static readonly Uri DiscoveredEndpoint = new Uri("tcp://discovered.invalid:2375");

    private static readonly Uri ExplicitEndpoint = new Uri("tcp://explicit.invalid:2375");

    [Fact]
    public async Task CreatingDeferredConfigurationDoesNotDiscoverEndpoint()
    {
      // Given
      var provider = new CountingProvider(DiscoveredEndpoint);
      var discovery = new DockerEndpointDiscovery(() => new[] { provider });

      // When
      var configuration = new DeferredDockerEndpointAuthenticationConfiguration(discovery);

      // Then
      Assert.Equal(0, provider.AvailabilityChecks);
      Assert.False(discovery.IsStarted);

      var resolved = await DeferredDockerEndpointAuthenticationConfiguration.EnsureResolvedAsync(configuration, TestContext.Current.CancellationToken);

      Assert.Equal(DiscoveredEndpoint, resolved.Endpoint);
      Assert.Equal(1, provider.AvailabilityChecks);
    }

    [Fact]
    public async Task ResolvingDeferredConfigurationDiscoversEndpointOnce()
    {
      // Given
      var provider = new CountingProvider(DiscoveredEndpoint);
      var discovery = new DockerEndpointDiscovery(() => new[] { provider });
      var configuration = new DeferredDockerEndpointAuthenticationConfiguration(discovery);

      // When
      _ = await DeferredDockerEndpointAuthenticationConfiguration.EnsureResolvedAsync(configuration, TestContext.Current.CancellationToken);
      _ = await DeferredDockerEndpointAuthenticationConfiguration.EnsureResolvedAsync(configuration, TestContext.Current.CancellationToken);

      // Then
      Assert.Equal(1, provider.AvailabilityChecks);
      Assert.Equal(DiscoveredEndpoint, configuration.Endpoint);
      Assert.Equal(1, provider.AvailabilityChecks);
    }

    [Fact]
    public async Task ExplicitEndpointWinsAndSkipsDiscovery()
    {
      // Given
      var provider = new CountingProvider(DiscoveredEndpoint);
      var discovery = new DockerEndpointDiscovery(() => new[] { provider });

      IDockerEndpointAuthenticationConfiguration explicitConfiguration = new DockerEndpointAuthenticationConfiguration(ExplicitEndpoint);

      // When
      var resolved = await DeferredDockerEndpointAuthenticationConfiguration.EnsureResolvedAsync(explicitConfiguration, TestContext.Current.CancellationToken);

      // Then
      Assert.Equal(ExplicitEndpoint, resolved.Endpoint);
      Assert.Equal(0, provider.AvailabilityChecks);
      Assert.False(discovery.IsStarted);
    }

    [Fact]
    public async Task ResolvingWithoutAvailableEndpointThrowsDockerUnavailableException()
    {
      // Given
      var provider = new CountingProvider(null);
      var discovery = new DockerEndpointDiscovery(() => new[] { provider });
      var configuration = new DeferredDockerEndpointAuthenticationConfiguration(discovery);

      // When
      var exception = await Record.ExceptionAsync(() => DeferredDockerEndpointAuthenticationConfiguration.EnsureResolvedAsync(configuration, TestContext.Current.CancellationToken));

      // Then
      Assert.IsType<DockerUnavailableException>(exception);
    }

    [Fact]
    public void DefaultOperatingSystemEndpointIsDeferred()
    {
      Assert.IsType<DeferredDockerEndpointAuthenticationConfiguration>(TestcontainersSettings.OS.DockerEndpointAuthConfig);
    }

    private sealed class CountingProvider : DockerEndpointAuthenticationProvider
    {
      private readonly Uri _endpoint;

      private int _availabilityChecks;

      public CountingProvider(Uri endpoint)
      {
        _endpoint = endpoint;
      }

      public int AvailabilityChecks => _availabilityChecks;

      public override bool IsApplicable()
      {
        return true;
      }

      public override Task<bool> IsAvailableAsync(CancellationToken ct = default)
      {
        Interlocked.Increment(ref _availabilityChecks);
        return Task.FromResult(_endpoint != null);
      }

      public override IDockerEndpointAuthenticationConfiguration GetAuthConfig()
      {
        return _endpoint == null ? null : new DockerEndpointAuthenticationConfiguration(_endpoint);
      }
    }
  }
}
