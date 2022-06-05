namespace DotNet.Testcontainers.Configurations
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// Windows operating system.
  /// </summary>
  [PublicAPI]
  public sealed class Windows : IOperatingSystem
  {
#pragma warning disable S1075

    private static readonly Uri DockerEngine = new Uri("npipe://./pipe/docker_engine");

#pragma warning restore S1075

    /// <summary>
    /// Initializes a new instance of the <see cref="Windows" /> class.
    /// </summary>
    [PublicAPI]
    public Windows()
      : this(DockerEngine)
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
    {
      this.DockerEndpointAuthConfig = new DockerEndpointAuthenticationConfiguration(endpoint);
    }

    /// <inheritdoc />
    public IDockerEndpointAuthenticationConfiguration DockerEndpointAuthConfig { get; }

    /// <inheritdoc />
    public string NormalizePath(string path)
    {
      return path.Replace('/', '\\');
    }
  }
}
