namespace DotNet.Testcontainers.Builders
{
  using System;
  using DotNet.Testcontainers.Configurations;

  /// <inheritdoc />
  internal sealed class EnvironmentEndpointAuthenticationProvider : IDockerEndpointAuthenticationProvider
  {
    private readonly Uri dockerEngine;

    public EnvironmentEndpointAuthenticationProvider()
    {
      this.dockerEngine = Uri.TryCreate(Environment.GetEnvironmentVariable("DOCKER_HOST"), UriKind.RelativeOrAbsolute, out var dockerHost) ? dockerHost : null;
    }

    /// <inheritdoc />
    public bool IsApplicable()
    {
      return this.dockerEngine != null;
    }

    /// <inheritdoc />
    public IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      return new DockerEndpointAuthenticationConfiguration(this.dockerEngine);
    }
  }
}
