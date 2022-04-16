namespace DotNet.Testcontainers.Configurations
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// Windows operating system.
  /// </summary>
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
    /// <param name="dockerApiEndpoint">The Docker API endpoint.</param>
    [PublicAPI]
    public Windows(string dockerApiEndpoint)
      : this(new Uri(dockerApiEndpoint))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Windows" /> class.
    /// </summary>
    /// <param name="dockerApiEndpoint">The Docker API endpoint.</param>
    [PublicAPI]
    public Windows(Uri dockerApiEndpoint)
    {
      this.DockerApiEndpoint = dockerApiEndpoint;
    }

    /// <inheritdoc />
    public Uri DockerApiEndpoint { get; }

    /// <inheritdoc />
    public string NormalizePath(string path)
    {
      return path.Replace('/', '\\');
    }
  }
}
