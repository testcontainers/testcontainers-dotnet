namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public static class ProtectDockerDaemonSocketTest
  {
    private static IDockerEndpointAuthenticationConfiguration GetAuthConfig(ProtectDockerDaemonSocket protectDockerDaemonSocket)
    {
      var customConfiguration = new PropertiesFileConfiguration(protectDockerDaemonSocket.CustomProperties.ToArray());
      return new IDockerEndpointAuthenticationProvider[] { new MTlsEndpointAuthenticationProvider(customConfiguration), new TlsEndpointAuthenticationProvider(customConfiguration) }.First(authProvider => authProvider.IsApplicable()).GetAuthConfig();
    }

    public sealed class MTls : IClassFixture<DockerMTlsFixture>
    {
      private readonly IDockerEndpointAuthenticationConfiguration _authConfig;

      public MTls(DockerMTlsFixture dockerMTlsFixture)
      {
        _authConfig = GetAuthConfig(dockerMTlsFixture);
      }

      [Fact]
      public async Task GetVersionReturnsVersion()
      {
        // Given
        var client = new TestcontainersClient(Guid.Empty, _authConfig, NullLogger.Instance);

        // When
        var version = await client.System.GetVersionAsync()
          .ConfigureAwait(false);

        // Then
        Assert.Equal(ProtectDockerDaemonSocket.DockerVersion, version.Version);
      }
    }

    public sealed class Tls : IClassFixture<DockerTlsFixture>
    {
      private readonly IDockerEndpointAuthenticationConfiguration _authConfig;

      public Tls(DockerTlsFixture dockerTlsFixture)
      {
        _authConfig = GetAuthConfig(dockerTlsFixture);
      }

      [Fact]
      public async Task GetVersionReturnsVersion()
      {
        // Given
        var client = new TestcontainersClient(Guid.Empty, _authConfig, NullLogger.Instance);

        // When
        var version = await client.System.GetVersionAsync()
          .ConfigureAwait(false);

        // Then
        Assert.Equal(ProtectDockerDaemonSocket.DockerVersion, version.Version);
      }
    }
  }
}
