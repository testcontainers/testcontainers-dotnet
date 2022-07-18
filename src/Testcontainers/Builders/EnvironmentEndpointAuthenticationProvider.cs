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
      ICustomConfiguration propertiesFileConfiguration = new PropertiesFileConfiguration();
      ICustomConfiguration environmentConfiguration = new EnvironmentConfiguration();
      this.dockerEngine = propertiesFileConfiguration.GetDockerHost() ?? environmentConfiguration.GetDockerHost();
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
