namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Runtime.InteropServices;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  internal sealed class UnixEndpointAuthenticationProvider : IDockerEndpointAuthenticationProvider
  {
    /// <summary>
    /// Gets the Unix socket Docker Engine endpoint.
    /// </summary>
    [NotNull]
    public static Uri DockerEngine { get; }
      = new Uri("unix:/var/run/docker.sock");

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
