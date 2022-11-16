namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix
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
      private readonly IDockerEndpointAuthenticationConfiguration authConfig;

      public MTls(DockerMTlsFixture dockerMTlsFixture)
      {
        this.authConfig = GetAuthConfig(dockerMTlsFixture);
      }

      [Fact]
      public async Task GetVersionReturnsVersion()
      {
        // Given
        IDockerSystemOperations dockerSystemOperations = new DockerSystemOperations(Guid.Empty, this.authConfig, NullLogger.Instance);

        // When
        var version = await dockerSystemOperations.GetVersionAsync()
          .ConfigureAwait(false);

        // Then
        Assert.Equal(ProtectDockerDaemonSocket.DockerVersion, version.Version);
      }
    }

    public sealed class Tls : IClassFixture<DockerTlsFixture>
    {
      private readonly IDockerEndpointAuthenticationConfiguration authConfig;

      public Tls(DockerTlsFixture dockerTlsFixture)
      {
        this.authConfig = GetAuthConfig(dockerTlsFixture);
      }

      [Fact]
      public async Task GetVersionReturnsVersion()
      {
        // Given
        IDockerSystemOperations dockerSystemOperations = new DockerSystemOperations(Guid.Empty, this.authConfig, NullLogger.Instance);

        // When
        var version = await dockerSystemOperations.GetVersionAsync()
          .ConfigureAwait(false);

        // Then
        Assert.Equal(ProtectDockerDaemonSocket.DockerVersion, version.Version);
      }
    }
  }
}
