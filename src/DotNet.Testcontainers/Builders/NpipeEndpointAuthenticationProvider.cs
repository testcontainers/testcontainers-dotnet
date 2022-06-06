namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Runtime.InteropServices;
  using DotNet.Testcontainers.Configurations;

  /// <inheritdoc cref="IAuthenticationProvider{TAuthenticationConfiguration}" />
  internal sealed class NpipeEndpointAuthenticationProvider : IAuthenticationProvider<IDockerEndpointAuthenticationConfiguration>
  {
#pragma warning disable S1075

    private static readonly Uri DockerEngine = new Uri("npipe://./pipe/docker_engine");

#pragma warning restore S1075

    /// <inheritdoc />
    public bool IsApplicable()
    {
      return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    /// <inheritdoc />
    public IDockerEndpointAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      return new DockerEndpointAuthenticationConfiguration(DockerEngine);
    }
  }
}
