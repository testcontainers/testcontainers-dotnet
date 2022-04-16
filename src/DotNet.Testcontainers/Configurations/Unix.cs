namespace DotNet.Testcontainers.Configurations
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// Unix operating system.
  /// </summary>
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
    /// <param name="dockerApiEndpoint">The Docker API endpoint.</param>
    [PublicAPI]
    public Unix(string dockerApiEndpoint)
      : this(new Uri(dockerApiEndpoint))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Unix" /> class.
    /// </summary>
    /// <param name="dockerApiEndpoint">The Docker API endpoint.</param>
    [PublicAPI]
    public Unix(Uri dockerApiEndpoint)
    {
      this.DockerApiEndpoint = dockerApiEndpoint;
    }

    /// <inheritdoc />
    public Uri DockerApiEndpoint { get; }

    /// <inheritdoc />
    public string NormalizePath(string path)
    {
      return path.Replace('\\', '/');
    }
  }
}
