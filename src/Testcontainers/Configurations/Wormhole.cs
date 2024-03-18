namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents a Docker Wormhole mount.
  /// </summary>
  public static class Wormhole
  {
    private const string UnixSocketScheme = "unix";

    private const string NamedPipeScheme = "npipe";

    /// <summary>
    /// Gets the Docker named pipe or socket mount configuration.
    /// </summary>
    /// <remarks>
    /// To override the Docker socket, use the properties file configuration <c>docker.socket.override</c>
    /// or the environment variable <c>TESTCONTAINERS_DOCKER_SOCKET_OVERRIDE</c>.
    /// </remarks>
    /// <param name="dockerEndpoint">The Docker endpoint.</param>
    /// <param name="isWindowsEngineEnabled">A value indicating whether the Windows Engine is enabled or not.</param>
    /// <returns>The Docker named pipe or socket mount configuration.</returns>
    public static IMount GetDockerSocket([NotNull] Uri dockerEndpoint, bool isWindowsEngineEnabled = false)
    {
      return isWindowsEngineEnabled ? (IMount)new NamedPipeMount(dockerEndpoint) : new UnixSocketMount(dockerEndpoint);
    }

    /// <inheritdoc cref="IMount" />
    private sealed class NamedPipeMount : IMount
    {
      public NamedPipeMount([NotNull] Uri dockerEndpoint)
      {
        var namedPipeName = Normalize(NpipeEndpointAuthenticationProvider.DockerEngine);

        // If the Docker endpoint is a named pipe, extract the named pipe from the URI; otherwise, fallback to the default named pipe name.
        Source = NamedPipeScheme.Equals(dockerEndpoint.Scheme, StringComparison.OrdinalIgnoreCase) ? Normalize(dockerEndpoint) : namedPipeName;

        // If the user has overridden the Docker named pipe, use the user-specified name; otherwise, keep the previously determined source.
        Source = !string.IsNullOrEmpty(TestcontainersSettings.DockerSocketOverride) ? TestcontainersSettings.DockerSocketOverride : Source;
        Target = namedPipeName;
        return;

        string Normalize(Uri uri) => Windows.Instance.NormalizePath(uri.ToString().Replace(NamedPipeScheme + ":", string.Empty));
      }

      /// <inheritdoc />
      public MountType Type
        => MountType.NamedPipe;

      /// <inheritdoc />
      public AccessMode AccessMode
        => AccessMode.ReadWrite;

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

    /// <inheritdoc cref="IMount" />
    private sealed class UnixSocketMount : IMount
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="UnixSocketMount" /> class.
      /// </summary>
      /// <param name="dockerEndpoint">The Docker endpoint.</param>
      public UnixSocketMount([NotNull] Uri dockerEndpoint)
      {
        var dockerSocketFilePath = UnixEndpointAuthenticationProvider.DockerEngine.AbsolutePath;

        // If the Docker endpoint is a Unix socket, extract the socket path from the URI; otherwise, fallback to the default Unix socket path.
        Source = UnixSocketScheme.Equals(dockerEndpoint.Scheme, StringComparison.OrdinalIgnoreCase) ? dockerEndpoint.AbsolutePath : dockerSocketFilePath;

        // If the user has overridden the Docker socket path, use the user-specified path; otherwise, keep the previously determined source.
        Source = !string.IsNullOrEmpty(TestcontainersSettings.DockerSocketOverride) ? TestcontainersSettings.DockerSocketOverride : Source;
        Target = dockerSocketFilePath;
      }

      /// <inheritdoc />
      public MountType Type
        => MountType.Bind;

      /// <inheritdoc />
      public AccessMode AccessMode
        => AccessMode.ReadOnly;

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
}
