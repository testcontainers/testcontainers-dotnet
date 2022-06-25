namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Runtime.InteropServices;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  internal sealed class NpipeEndpointAuthenticationProvider : IDockerEndpointAuthenticationProvider
  {
#pragma warning disable S1075

    /// <summary>
    /// Gets the named pipe Docker Engine endpoint.
    /// </summary>
    [PublicAPI]
    public static Uri DockerEngine { get; }
      = new Uri("npipe://./pipe/docker_engine");

#pragma warning restore S1075

    /// <inheritdoc />
    public bool IsApplicable()
    {
      return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    /// <inheritdoc />
    public IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      return new DockerEndpointAuthenticationConfiguration(DockerEngine);
    }
  }
}
