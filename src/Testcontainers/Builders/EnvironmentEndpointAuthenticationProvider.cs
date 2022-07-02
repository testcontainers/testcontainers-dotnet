namespace DotNet.Testcontainers.Builders
{
  using System;
  using DotNet.Testcontainers.Configurations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  internal sealed class EnvironmentEndpointAuthenticationProvider : IDockerEndpointAuthenticationProvider
  {
    private readonly Uri dockerEngine;

    public EnvironmentEndpointAuthenticationProvider()
    {
      ICustomConfiguration propertiesFileConfiguration = new PropertiesFileConfiguration();
      ICustomConfiguration environmentConfiguration = new EnvironmentConfiguration();
      this.dockerEngine = propertiesFileConfiguration.GetDockerHost() ?? environmentConfiguration.GetDockerHost();
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
