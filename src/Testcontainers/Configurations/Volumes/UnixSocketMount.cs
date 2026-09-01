namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IMount" />
  internal readonly struct UnixSocketMount : IMount
  {
    private const string DockerSocketFilePath = "/var/run/docker.sock";

    /// <summary>
    /// Initializes a new instance of the <see cref="UnixSocketMount" /> struct.
    /// </summary>
    /// <param name="dockerEndpoint">The Docker endpoint.</param>
    public UnixSocketMount([NotNull] Uri dockerEndpoint)
    {
      // If the Docker endpoint is a Unix socket, extract the socket path from the
      // URI; otherwise, fallback to the default Unix socket path.
      var source = "unix".Equals(dockerEndpoint.Scheme, StringComparison.OrdinalIgnoreCase) ? dockerEndpoint.AbsolutePath : DockerSocketFilePath;

      Type = MountType.Bind;

      // If the user has overridden the Docker socket path, use the user-specified
      // path; otherwise, keep the previously determined source.
      Source = !string.IsNullOrEmpty(TestcontainersSettings.DockerSocketOverride) ? TestcontainersSettings.DockerSocketOverride : source;
      Target = DockerSocketFilePath;
      AccessMode = AccessMode.ReadOnly;
    }

    /// <inheritdoc />
    public MountType Type { get; }

    /// <inheritdoc />
    public AccessMode AccessMode { get; }

    /// <inheritdoc />
    public string Source { get; }

    /// <inheritdoc />
    public string Target { get; }

    /// <inheritdoc />
    public Task CreateAsync(CancellationToken ct = default)
    {
      return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(CancellationToken ct = default)
    {
      return Task.CompletedTask;
    }
  }
}
