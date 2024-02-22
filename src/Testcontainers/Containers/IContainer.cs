namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// A container instance.
  /// </summary>
  [PublicAPI]
  public interface IContainer : IAsyncDisposable
  {
    /// <summary>
    /// Subscribes to the creating event.
    /// </summary>
    [CanBeNull]
    event EventHandler Creating;

    /// <summary>
    /// Subscribes to the starting event.
    /// </summary>
    [CanBeNull]
    event EventHandler Starting;

    /// <summary>
    /// Subscribes to the stopping event.
    /// </summary>
    [CanBeNull]
    event EventHandler Stopping;

    /// <summary>
    /// Subscribes to the created event.
    /// </summary>
    [CanBeNull]
    event EventHandler Created;

    /// <summary>
    /// Subscribes to the started event.
    /// </summary>
    [CanBeNull]
    event EventHandler Started;

    /// <summary>
    /// Subscribes to the stopped event.
    /// </summary>
    [CanBeNull]
    event EventHandler Stopped;

    /// <summary>
    /// Gets the logger.
    /// </summary>
    [NotNull]
    ILogger Logger { get; }

    /// <summary>
    /// Gets the created timestamp.
    /// </summary>
    DateTime CreatedTime { get; }

    /// <summary>
    /// Gets the started timestamp.
    /// </summary>
    DateTime StartedTime { get; }

    /// <summary>
    /// Gets the stopped timestamp.
    /// </summary>
    DateTime StoppedTime { get; }

    /// <summary>
    /// Gets the container id.
    /// </summary>
    /// <exception cref="InvalidOperationException">Container has not been created.</exception>
    [NotNull]
    string Id { get; }

    /// <summary>
    /// Gets the container name.
    /// </summary>
    /// <exception cref="InvalidOperationException">Container has not been created.</exception>
    [NotNull]
    string Name { get; }

    /// <summary>
    /// Gets the container IP address.
    /// </summary>
    /// <exception cref="InvalidOperationException">Container has not been created.</exception>
    [NotNull]
    string IpAddress { get; }

    /// <summary>
    /// Gets the container MAC address.
    /// </summary>
    /// <exception cref="InvalidOperationException">Container has not been created.</exception>
    [NotNull]
    string MacAddress { get; }

    /// <summary>
    /// Gets the container hostname.
    /// </summary>
    /// <exception cref="InvalidOperationException">Container has not been created.</exception>
    [NotNull]
    string Hostname { get; }

    /// <summary>
    /// Gets the container image.
    /// </summary>
    [NotNull]
    IImage Image { get; }

    /// <summary>
    /// Gets the container state.
    /// </summary>
    TestcontainersStates State { get; }

    /// <summary>
    /// Gets the container health status.
    /// </summary>
    TestcontainersHealthStatus Health { get; }

    /// <summary>
    /// Gets the container health check failing streak.
    /// </summary>
    long HealthCheckFailingStreak { get; }

    /// <summary>
    /// Resolves the public assigned host port.
    /// </summary>
    /// <param name="containerPort">The container port.</param>
    /// <returns>Returns the public assigned host port.</returns>
    /// <exception cref="InvalidOperationException">Container has not been created.</exception>
    ushort GetMappedPublicPort(int containerPort);

    /// <summary>
    /// Resolves the public assigned host port.
    /// </summary>
    /// <param name="containerPort">The container port.</param>
    /// <returns>Returns the public assigned host port.</returns>
    /// <exception cref="InvalidOperationException">Container has not been created.</exception>
    ushort GetMappedPublicPort(string containerPort);

    /// <summary>
    /// Gets the container exit code.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Returns the container exit code.</returns>
    Task<long> GetExitCodeAsync(CancellationToken ct = default);

    /// <summary>
    /// Gets the container logs.
    /// </summary>
    /// <param name="since">Only logs since this time.</param>
    /// <param name="until">Only logs until this time.</param>
    /// <param name="timestampsEnabled">Determines whether every log line contains a timestamp or not.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Returns the container logs.</returns>
    Task<(string Stdout, string Stderr)> GetLogsAsync(DateTime since = default, DateTime until = default, bool timestampsEnabled = true, CancellationToken ct = default);

    /// <summary>
    /// Starts the container.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been started.</returns>
    /// <exception cref="OperationCanceledException">Thrown when a Docker API call gets canceled.</exception>
    /// <exception cref="TaskCanceledException">Thrown when a Testcontainers task gets canceled.</exception>
    /// <exception cref="TimeoutException">Thrown when the wait strategy task gets canceled or the timeout expires.</exception>
    Task StartAsync(CancellationToken ct = default);

    /// <summary>
    /// Stops the container.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been stopped.</returns>
    /// <exception cref="OperationCanceledException">Thrown when a Docker API call gets canceled.</exception>
    /// <exception cref="TaskCanceledException">Thrown when a Testcontainers task gets canceled.</exception>
    Task StopAsync(CancellationToken ct = default);

    /// <summary>
    /// Copies a test host file to the container.
    /// </summary>
    /// <param name="fileContent">The byte array content of the file.</param>
    /// <param name="filePath">The target file path to copy the file to.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns></returns>
    Task CopyAsync(byte[] fileContent, string filePath, UnixFileModes fileMode = Unix.FileMode644, CancellationToken ct = default);

    /// <summary>
    /// Copies a test host directory or file to the container.
    /// </summary>
    /// <param name="source">The source directory or file to be copied.</param>
    /// <param name="target">The target directory path to copy the files to.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that completes when the directory or file has been copied.</returns>
    Task CopyAsync(string source, string target, UnixFileModes fileMode = Unix.FileMode644, CancellationToken ct = default);

    /// <summary>
    /// Copies a test host directory to the container.
    /// </summary>
    /// <param name="source">The source directory to be copied.</param>
    /// <param name="target">The target directory path to copy the files to.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that completes when the directory has been copied.</returns>
    Task CopyAsync(DirectoryInfo source, string target, UnixFileModes fileMode = Unix.FileMode644, CancellationToken ct = default);

    /// <summary>
    /// Copies a test host file to the container.
    /// </summary>
    /// <param name="source">The source file to be copied.</param>
    /// <param name="target">The target directory path to copy the file to.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that completes when the file has been copied.</returns>
    Task CopyAsync(FileInfo source, string target, UnixFileModes fileMode = Unix.FileMode644, CancellationToken ct = default);

    /// <summary>
    /// Reads a file from the container.
    /// </summary>
    /// <param name="filePath">An absolute path or a name value within the container.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the file has been read.</returns>
    Task<byte[]> ReadFileAsync(string filePath, CancellationToken ct = default);

    /// <summary>
    /// Executes a command in the container.
    /// </summary>
    /// <param name="command">Shell command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the shell command has been executed.</returns>
    Task<ExecResult> ExecAsync(IList<string> command, CancellationToken ct = default);
  }
}
