namespace DotNet.Testcontainers.Configurations
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// Unix operating system.
  /// </summary>
  [PublicAPI]
  public sealed class Unix : IOperatingSystem
  {
    private static readonly Uri DockerEngine = new Uri("unix:/var/run/docker.sock");

    /// <summary>
    /// Initializes a new instance of the <see cref="Unix" /> class.
    /// </summary>
    [PublicAPI]
    public Unix()
      : this(DockerEngine)
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
    {
      this.DockerEndpointAuthConfig = new DockerEndpointAuthenticationConfiguration(endpoint);
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
