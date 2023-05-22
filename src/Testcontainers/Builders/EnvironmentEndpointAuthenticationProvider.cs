namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Linq;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  [PublicAPI]
  internal sealed class EnvironmentEndpointAuthenticationProvider : DockerEndpointAuthenticationProvider
  {
    private readonly Uri _dockerEngine;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentEndpointAuthenticationProvider" /> class.
    /// </summary>
    public EnvironmentEndpointAuthenticationProvider()
      : this(EnvironmentConfiguration.Instance, PropertiesFileConfiguration.Instance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentEndpointAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="customConfigurations">A list of custom configurations.</param>
    public EnvironmentEndpointAuthenticationProvider(params ICustomConfiguration[] customConfigurations)
    {
      _dockerEngine = customConfigurations
        .Select(customConfiguration => customConfiguration.GetDockerHost())
        .FirstOrDefault(dockerHost => dockerHost != null);
    }

    /// <inheritdoc />
    public override bool IsApplicable()
    {
      return _dockerEngine != null;
    }

    /// <inheritdoc />
    public override IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      return new DockerEndpointAuthenticationConfiguration(_dockerEngine);
    }
  }
}
