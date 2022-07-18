namespace DotNet.Testcontainers.Configurations
{
  using System;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <summary>
  /// Unix operating system.
  /// </summary>
  [PublicAPI]
  public sealed class Unix : IOperatingSystem
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Unix" /> class.
    /// </summary>
    [PublicAPI]
    public Unix()
      : this(UnixEndpointAuthenticationProvider.DockerEngine)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Unix" /> class.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    [PublicAPI]
    public Unix(string endpoint)
      : this(new Uri(endpoint))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Unix" /> class.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    [PublicAPI]
    public Unix(Uri endpoint)
      : this(new DockerEndpointAuthenticationConfiguration(endpoint))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Unix" /> class.
    /// </summary>
    /// <param name="dockerEndpointAuthConfig">The Docker endpoint authentication configuration.</param>
    [PublicAPI]
    public Unix(IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig)
    {
      this.DockerEndpointAuthConfig = dockerEndpointAuthConfig;
    }

    /// <inheritdoc />
    public IDockerEndpointAuthenticationConfiguration DockerEndpointAuthConfig { get; }

    /// <inheritdoc />
    public string NormalizePath(string path)
    {
      return path.Replace('\\', '/');
    }
  }
}
