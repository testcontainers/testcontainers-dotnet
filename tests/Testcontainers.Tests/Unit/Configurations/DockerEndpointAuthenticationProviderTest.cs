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
      _ = File.Create(Path.Combine(CertificatesDirectoryPath, "ca.pem"));
      _ = File.Create(Path.Combine(CertificatesDirectoryPath, "cert.pem"));
      _ = File.Create(Path.Combine(CertificatesDirectoryPath, "key.pem"));
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

    private sealed class AuthProviderTestData : List<object[]>
    {
      public AuthProviderTestData()
      {
        var defaultConfiguration = new PropertiesFileConfiguration(Array.Empty<string>());
        var dockerTlsConfiguration = new PropertiesFileConfiguration("docker.tls=true", $"docker.cert.path={CertificatesDirectoryPath}");
        var dockerMTlsConfiguration = new PropertiesFileConfiguration("docker.tls.verify=true", $"docker.cert.path={CertificatesDirectoryPath}");
        this.Add(new object[] { new MTlsEndpointAuthenticationProvider(defaultConfiguration), false });
        this.Add(new object[] { new MTlsEndpointAuthenticationProvider(dockerMTlsConfiguration), true });
        this.Add(new object[] { new MTlsEndpointAuthenticationProvider(Array.Empty<ICustomConfiguration>()), false });
        this.Add(new object[] { new MTlsEndpointAuthenticationProvider(defaultConfiguration, dockerMTlsConfiguration), true });
        this.Add(new object[] { new TlsEndpointAuthenticationProvider(defaultConfiguration), false });
        this.Add(new object[] { new TlsEndpointAuthenticationProvider(dockerTlsConfiguration), true });
        this.Add(new object[] { new TlsEndpointAuthenticationProvider(Array.Empty<ICustomConfiguration>()), false });
        this.Add(new object[] { new TlsEndpointAuthenticationProvider(defaultConfiguration, dockerTlsConfiguration), true });
        this.Add(new object[] { new EnvironmentEndpointAuthenticationProvider(defaultConfiguration), false });
        this.Add(new object[] { new EnvironmentEndpointAuthenticationProvider(DockerHostConfiguration), true });
        this.Add(new object[] { new EnvironmentEndpointAuthenticationProvider(Array.Empty<ICustomConfiguration>()), false });
        this.Add(new object[] { new EnvironmentEndpointAuthenticationProvider(defaultConfiguration, DockerHostConfiguration), true });
        this.Add(new object[] { new NpipeEndpointAuthenticationProvider(), RuntimeInformation.IsOSPlatform(OSPlatform.Windows) });
        this.Add(new object[] { new UnixEndpointAuthenticationProvider(), !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) });
      }
    }

    private sealed class AuthConfigTestData : List<object[]>
    {
      public AuthConfigTestData()
      {
        this.Add(new object[] { new TlsEndpointAuthenticationProvider(DockerTlsHostConfiguration).GetAuthConfig(), new Uri(DockerTlsHost) });
        this.Add(new object[] { new EnvironmentEndpointAuthenticationProvider(DockerHostConfiguration).GetAuthConfig(), new Uri(DockerHost) });
        this.Add(new object[] { new NpipeEndpointAuthenticationProvider().GetAuthConfig(), new Uri("npipe://./pipe/docker_engine") });
        this.Add(new object[] { new UnixEndpointAuthenticationProvider().GetAuthConfig(), new Uri("unix:/var/run/docker.sock") });
      }
    }
  }
}
