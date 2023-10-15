namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;

  /// <summary>
  /// This class represents a Testcontainers client.
  /// </summary>
  internal interface ITestcontainersClient
  {
    /// <summary>
    /// Gets the Docker container operations endpoint.
    /// </summary>
    IDockerContainerOperations Container { get; }

    /// <summary>
    /// Gets the Docker image operations endpoint.
    /// </summary>
    IDockerImageOperations Image { get; }

    /// <summary>
    /// Gets the Docker network operations endpoint.
    /// </summary>
    IDockerNetworkOperations Network { get; }

    /// <summary>
    /// Gets the Docker volume operations endpoint.
    /// </summary>
    IDockerVolumeOperations Volume { get; }

    /// <summary>
    /// Gets the Docker system operations endpoint.
    /// </summary>
    IDockerSystemOperations System { get; }

    /// <summary>
    /// Gets a value indicating whether the container is running inside another container or not.
    /// </summary>
    bool IsRunningInsideDocker { get; }

    /// <summary>
    /// Gets the container exit code.
    /// </summary>
    /// <param name="id">The container id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that gets the container exit code.</returns>
    Task<long> GetContainerExitCodeAsync(string id, CancellationToken ct = default);

    /// <summary>
    /// Gets the container logs.
    /// </summary>
    /// <param name="id">The container id.</param>
    /// <param name="since">Only logs since this time.</param>
    /// <param name="until">Only logs until this time.</param>
    /// <param name="timestampsEnabled">Determines whether every log line contains a timestamp or not.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that gets the container logs.</returns>
    Task<(string Stdout, string Stderr)> GetContainerLogsAsync(string id, DateTime since = default, DateTime until = default, bool timestampsEnabled = true, CancellationToken ct = default);

    /// <summary>
    /// Starts the container.
    /// </summary>
    /// <param name="id">The container id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been started.</returns>
    Task StartAsync(string id, CancellationToken ct = default);

    /// <summary>
    /// Stops the container.
    /// </summary>
    /// <param name="id">The container id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been stopped.</returns>
    Task StopAsync(string id, CancellationToken ct = default);

    /// <summary>
    /// Removes the container.
    /// </summary>
    /// <param name="id">The container id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been removed.</returns>
    Task RemoveAsync(string id, CancellationToken ct = default);

    /// <summary>
    /// Attaches to the container and copies the output to the <see cref="IOutputConsumer" />.
    /// </summary>
    /// <param name="id">The container id.</param>
    /// <param name="outputConsumer">The stdout and stderr consumer.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container's stdout and stderr has been copied to the consumer.</returns>
    Task AttachAsync(string id, IOutputConsumer outputConsumer, CancellationToken ct = default);

    /// <summary>
    /// Executes a command in the container.
    /// </summary>
    /// <param name="id">The container id.</param>
    /// <param name="command">The shell command to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the shell command has been executed.</returns>
    Task<ExecResult> ExecAsync(string id, IList<string> command, CancellationToken ct = default);

    /// <summary>
    /// Copies the content of an implementation of <see cref="IResourceMapping" /> to the container.
    /// </summary>
    /// <param name="id">The container id.</param>
    /// <param name="resourceMapping">The resource mapping to add to the archive.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that completes when the content has been copied.</returns>
    Task CopyAsync(string id, IResourceMapping resourceMapping, CancellationToken ct = default);

    /// <summary>
    /// Copies a test host directory to the container.
    /// </summary>
    /// <param name="id">The container id.</param>
    /// <param name="source">The source directory to be copied.</param>
    /// <param name="target">The target directory path to copy the files to.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that completes when the directory has been copied.</returns>
    Task CopyAsync(string id, DirectoryInfo source, string target, UnixFileModes fileMode, CancellationToken ct = default);

    /// <summary>
    /// Copies a test host file to the container.
    /// </summary>
    /// <param name="id">The container id.</param>
    /// <param name="source">The source file to be copied.</param>
    /// <param name="target">The target directory path to copy the file to.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that completes when the file has been copied.</returns>
    Task CopyAsync(string id, FileInfo source, string target, UnixFileModes fileMode, CancellationToken ct = default);

    /// <summary>
    /// Reads a file from the container.
    /// </summary>
    /// <param name="id">The container id.</param>
    /// <param name="filePath">The path to the file in the container.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the file has been read.</returns>
    Task<byte[]> ReadFileAsync(string id, string filePath, CancellationToken ct = default);

    /// <summary>
    /// Creates the container.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been created.</returns>
    Task<string> RunAsync(IContainerConfiguration configuration, CancellationToken ct = default);

    /// <summary>
    /// Builds a Docker image from a Dockerfile.
    /// </summary>
    /// <param name="configuration">The Dockerfile configuration.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the Docker image has been built.</returns>
    Task<string> BuildAsync(IImageFromDockerfileConfiguration configuration, CancellationToken ct = default);
  }
}
