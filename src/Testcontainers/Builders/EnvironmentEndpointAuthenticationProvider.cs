namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Linq;
  using DotNet.Testcontainers.Configurations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  internal sealed class EnvironmentEndpointAuthenticationProvider : DockerEndpointAuthenticationProvider
  {
    private readonly Uri dockerEngine;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentEndpointAuthenticationProvider" /> class.
    /// </summary>
    public EnvironmentEndpointAuthenticationProvider()
      : this(PropertiesFileConfiguration.Instance, EnvironmentConfiguration.Instance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentEndpointAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="customConfigurations">A list of custom configurations.</param>
    public EnvironmentEndpointAuthenticationProvider(params ICustomConfiguration[] customConfigurations)
    {
      this.dockerEngine = customConfigurations
        .Select(customConfiguration => customConfiguration.GetDockerHost())
        .FirstOrDefault(dockerHost => dockerHost != null);
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
