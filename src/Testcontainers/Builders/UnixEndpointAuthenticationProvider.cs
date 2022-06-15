namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Runtime.InteropServices;
  using DotNet.Testcontainers.Configurations;

  /// <inheritdoc cref="IAuthenticationProvider{TAuthenticationConfiguration}" />
  internal sealed class UnixEndpointAuthenticationProvider : IAuthenticationProvider<IDockerEndpointAuthenticationConfiguration>
  {
    private static readonly Uri DockerEngine = new Uri("unix:/var/run/docker.sock");

    /// <inheritdoc />
    public bool IsApplicable()
    {
      return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    /// <inheritdoc />
    public IDockerEndpointAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      return new DockerEndpointAuthenticationConfiguration(DockerEngine);
    }
  }
}
