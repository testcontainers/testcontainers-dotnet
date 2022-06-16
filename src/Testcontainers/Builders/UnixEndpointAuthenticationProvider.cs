namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Runtime.InteropServices;
  using DotNet.Testcontainers.Configurations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  internal sealed class UnixEndpointAuthenticationProvider : IDockerEndpointAuthenticationProvider
  {
    private static readonly Uri DockerEngine = new Uri("unix:/var/run/docker.sock");

    /// <inheritdoc />
    public bool IsApplicable()
    {
      return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    /// <inheritdoc />
    public IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      return new DockerEndpointAuthenticationConfiguration(DockerEngine);
    }
  }
}
