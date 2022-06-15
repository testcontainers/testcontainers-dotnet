namespace DotNet.Testcontainers.Builders
{
  using System;
  using DotNet.Testcontainers.Configurations;

  /// <inheritdoc cref="IAuthenticationProvider{TAuthenticationConfiguration}" />
  internal sealed class EnvironmentEndpointAuthenticationProvider : IAuthenticationProvider<IDockerEndpointAuthenticationConfiguration>
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
    public IDockerEndpointAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      return new DockerEndpointAuthenticationConfiguration(this.dockerEngine);
    }
  }
}
