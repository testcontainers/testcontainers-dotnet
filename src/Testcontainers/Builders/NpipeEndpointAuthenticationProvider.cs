namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Runtime.InteropServices;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  [PublicAPI]
  internal sealed class NpipeEndpointAuthenticationProvider : DockerEndpointAuthenticationProvider
  {
    /// <summary>
    /// Gets the named pipe Docker Engine endpoint.
    /// </summary>
    public static Uri DockerEngine { get; }
      = new Uri("npipe://./pipe/docker_engine");

    /// <inheritdoc />
    public override bool IsApplicable()
    {
      return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    /// <inheritdoc />
    public override IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      return new DockerEndpointAuthenticationConfiguration(DockerEngine);
    }
  }
}
