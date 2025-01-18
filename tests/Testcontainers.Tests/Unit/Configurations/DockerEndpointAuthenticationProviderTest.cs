namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Runtime.InteropServices;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using Xunit;

  public sealed class DockerEndpointAuthenticationProviderTest
  {
    private const string DockerHost = "tcp://127.0.0.1:2375";

    private const string DockerTlsHost = "tcp://127.0.0.1:2376";

    private static readonly string CertificatesDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));

    private static readonly ICustomConfiguration DockerHostConfiguration = new PropertiesFileConfiguration(new[] { "docker.host=" + DockerHost });

    private static readonly ICustomConfiguration DockerTlsHostConfiguration = new PropertiesFileConfiguration(new[] { "docker.host=" + DockerTlsHost });

    static DockerEndpointAuthenticationProviderTest()
    {
      _ = Directory.CreateDirectory(CertificatesDirectoryPath);
      using var fileStream1 = File.Create(Path.Combine(CertificatesDirectoryPath, "ca.pem"));
      using var fileStream2 = File.Create(Path.Combine(CertificatesDirectoryPath, "cert.pem"));
      using var fileStream3 = File.Create(Path.Combine(CertificatesDirectoryPath, "key.pem"));
    }

    [Theory]
    [ClassData(typeof(AuthProviderTestData))]
    internal void AuthProviderShouldBeApplicable(IDockerEndpointAuthenticationProvider authProvider, bool isApplicable)
    {
      Assert.Equal(isApplicable, authProvider.IsApplicable());
    }

    [Theory]
    [ClassData(typeof(AuthConfigTestData))]
    internal void AuthConfigShouldGetDockerClientEndpoint(IDockerEndpointAuthenticationConfiguration authConfig, Uri dockerClientEndpoint)
    {
      using (var dockerClientConfiguration = authConfig.GetDockerClientConfiguration())
      {
        Assert.Equal(dockerClientEndpoint, authConfig.Endpoint);
        Assert.Equal(dockerClientEndpoint, dockerClientConfiguration.EndpointBaseUri);
      }
    }

    public sealed class TestcontainersHostEndpointAuthenticationProviderTest
    {
      [Fact]
      public void GetDockerHostOverrideReturnsNull()
      {
        ICustomConfiguration customConfiguration = new TestcontainersEndpointAuthenticationProvider("host.override=host.docker.internal");
        Assert.Null(customConfiguration.GetDockerHostOverride());
      }

      [Fact]
      public void GetDockerSocketOverrideReturnsNull()
      {
        ICustomConfiguration customConfiguration = new TestcontainersEndpointAuthenticationProvider("docker.socket.override=/var/run/docker.sock");
        Assert.Null(customConfiguration.GetDockerSocketOverride());
      }
    }

    private sealed class AuthProviderTestData : List<object[]>
    {
      public AuthProviderTestData()
      {
        var defaultConfiguration = new PropertiesFileConfiguration(Array.Empty<string>());
        var dockerTlsConfiguration = new PropertiesFileConfiguration("docker.tls=true", "docker.cert.path=" + CertificatesDirectoryPath);
        var dockerMTlsConfiguration = new PropertiesFileConfiguration("docker.tls.verify=true", "docker.cert.path=" + CertificatesDirectoryPath);
        Add(new object[] { new MTlsEndpointAuthenticationProvider(defaultConfiguration), false });
        Add(new object[] { new MTlsEndpointAuthenticationProvider(dockerMTlsConfiguration), true });
        Add(new object[] { new MTlsEndpointAuthenticationProvider(Array.Empty<ICustomConfiguration>()), false });
        Add(new object[] { new MTlsEndpointAuthenticationProvider(defaultConfiguration, dockerMTlsConfiguration), true });
        Add(new object[] { new TlsEndpointAuthenticationProvider(defaultConfiguration), false });
        Add(new object[] { new TlsEndpointAuthenticationProvider(dockerTlsConfiguration), true });
        Add(new object[] { new TlsEndpointAuthenticationProvider(Array.Empty<ICustomConfiguration>()), false });
        Add(new object[] { new TlsEndpointAuthenticationProvider(defaultConfiguration, dockerTlsConfiguration), true });
        Add(new object[] { new EnvironmentEndpointAuthenticationProvider(defaultConfiguration), false });
        Add(new object[] { new EnvironmentEndpointAuthenticationProvider(DockerHostConfiguration), true });
        Add(new object[] { new EnvironmentEndpointAuthenticationProvider(Array.Empty<ICustomConfiguration>()), false });
        Add(new object[] { new EnvironmentEndpointAuthenticationProvider(defaultConfiguration, DockerHostConfiguration), true });
        Add(new object[] { new NpipeEndpointAuthenticationProvider(), RuntimeInformation.IsOSPlatform(OSPlatform.Windows) });
        Add(new object[] { new UnixEndpointAuthenticationProvider(), !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) });
        Add(new object[] { new TestcontainersEndpointAuthenticationProvider(string.Empty), false });
        Add(new object[] { new TestcontainersEndpointAuthenticationProvider("tc.host=" + DockerHost), true });
      }
    }

    private sealed class AuthConfigTestData : List<object[]>
    {
      public AuthConfigTestData()
      {
        Add(new object[] { new TlsEndpointAuthenticationProvider(DockerTlsHostConfiguration).GetAuthConfig(), new Uri(DockerTlsHost) });
        Add(new object[] { new EnvironmentEndpointAuthenticationProvider(DockerHostConfiguration).GetAuthConfig(), new Uri(DockerHost) });
        Add(new object[] { new NpipeEndpointAuthenticationProvider().GetAuthConfig(), new Uri("npipe://./pipe/docker_engine") });
        Add(new object[] { new UnixEndpointAuthenticationProvider().GetAuthConfig(), new Uri("unix:///var/run/docker.sock") });
        Add(new object[] { new TestcontainersEndpointAuthenticationProvider("tc.host=" + DockerHost).GetAuthConfig(), new Uri(DockerHost) });
      }
    }
  }
}
