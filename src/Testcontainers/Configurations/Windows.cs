namespace DotNet.Testcontainers.Configurations
{
  using System;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <summary>
  /// Windows operating system.
  /// </summary>
  [PublicAPI]
  public sealed class Windows : IOperatingSystem
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Windows" /> class.
    /// </summary>
    [PublicAPI]
    public Windows()
      : this(NpipeEndpointAuthenticationProvider.DockerEngine)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Windows" /> class.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    [PublicAPI]
    public Windows(string endpoint)
      : this(new Uri(endpoint))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Windows" /> class.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    [PublicAPI]
    public Windows(Uri endpoint)
      : this(new DockerEndpointAuthenticationConfiguration(endpoint))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Windows" /> class.
    /// </summary>
    /// <param name="dockerEndpointAuthConfig">The Docker endpoint authentication configuration.</param>
    [PublicAPI]
    public Windows(IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig)
    {
      DockerEndpointAuthConfig = dockerEndpointAuthConfig;
    }

    /// <summary>
    /// Gets the <see cref="IOperatingSystem" /> instance.
    /// </summary>
    public static IOperatingSystem Instance { get; }
      = new Windows(dockerEndpointAuthConfig: null);

    /// <inheritdoc />
    public IDockerEndpointAuthenticationConfiguration DockerEndpointAuthConfig { get; }

    /// <inheritdoc />
    public string NormalizePath(string path)
    {
      return path?.Replace('/', '\\');
    }
  }
}
