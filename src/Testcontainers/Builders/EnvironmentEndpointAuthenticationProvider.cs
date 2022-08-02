namespace DotNet.Testcontainers.Builders
{
  using System;
  using DotNet.Testcontainers.Configurations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  internal sealed class EnvironmentEndpointAuthenticationProvider : DockerEndpointAuthenticationProvider
  {
    private readonly Uri dockerEngine;

    public EnvironmentEndpointAuthenticationProvider()
    {
      this.dockerEngine = PropertiesFileConfiguration.Instance.GetDockerHost() ?? EnvironmentConfiguration.Instance.GetDockerHost();
    }

    /// <inheritdoc />
    public override bool IsApplicable()
    {
      return this.dockerEngine != null;
    }

    /// <inheritdoc />
    public override IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      return new DockerEndpointAuthenticationConfiguration(this.dockerEngine);
    }
  }
}
