namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Runtime.InteropServices;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  [PublicAPI]
  internal sealed class UnixEndpointAuthenticationProvider : DockerEndpointAuthenticationProvider
  {
    /// <summary>
    /// Gets the Unix socket Docker Engine endpoint.
    /// </summary>
    public static Uri DockerEngine { get; }
      = new Uri("unix:///var/run/docker.sock");

    /// <inheritdoc />
    public override bool IsApplicable()
    {
      return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    /// <inheritdoc />
    public override IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      return new DockerEndpointAuthenticationConfiguration(DockerEngine);
    }
  }
}
