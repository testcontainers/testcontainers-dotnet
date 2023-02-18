namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// A container instance.
  /// </summary>
  [PublicAPI]
  public interface IContainer : ITestcontainersContainer
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
    new ILogger Logger { get; }

    /// <summary>
    /// Gets the container id.
    /// </summary>
    /// <exception cref="InvalidOperationException">Container has not been created.</exception>
    [NotNull]
    new string Id { get; }

    /// <summary>
    /// Gets the container name.
    /// </summary>
    /// <exception cref="InvalidOperationException">Container has not been created.</exception>
    [NotNull]
    new string Name { get; }

    /// <summary>
    /// Gets the container IP address.
    /// </summary>
    /// <exception cref="InvalidOperationException">Container has not been created.</exception>
    [NotNull]
    new string IpAddress { get; }

    /// <summary>
    /// Gets the container MAC address.
    /// </summary>
    /// <exception cref="InvalidOperationException">Container has not been created.</exception>
    [NotNull]
    new string MacAddress { get; }

    /// <summary>
    /// Gets the container hostname.
    /// </summary>
    /// <exception cref="InvalidOperationException">Container has not been created.</exception>
    [NotNull]
    new string Hostname { get; }

    /// <summary>
    /// Gets the container image.
    /// </summary>
    [NotNull]
    new IImage Image { get; }

    /// <summary>
    /// Gets the container state.
    /// </summary>
    new TestcontainersStates State { get; }

    /// <summary>
    /// Gets the container health status.
    /// </summary>
    new TestcontainersHealthStatus Health { get; }

    /// <summary>
    /// Gets the container health check failing streak.
    /// </summary>
    new long HealthCheckFailingStreak { get; }

    /// <summary>
    /// Resolves the public assigned host port.
    /// </summary>
    /// <param name="containerPort">The container port.</param>
    /// <returns>Returns the public assigned host port.</returns>
    /// <exception cref="InvalidOperationException">Container has not been created.</exception>
    new ushort GetMappedPublicPort(int containerPort);

    /// <summary>
    /// Resolves the public assigned host port.
    /// </summary>
    /// <param name="containerPort">The container port.</param>
    /// <returns>Returns the public assigned host port.</returns>
    /// <exception cref="InvalidOperationException">Container has not been created.</exception>
    new ushort GetMappedPublicPort(string containerPort);

    /// <inheritdoc cref="GetExitCodeAsync" />
    new Task<long> GetExitCode(CancellationToken ct = default);

    /// <summary>
    /// Gets the container exit code.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Returns the container exit code.</returns>
    new Task<long> GetExitCodeAsync(CancellationToken ct = default);

    /// <inheritdoc cref="GetLogsAsync" />
    new Task<(string Stdout, string Stderr)> GetLogs(DateTime since = default, DateTime until = default, bool timestampsEnabled = true, CancellationToken ct = default);

    /// <summary>
    /// Gets the container logs.
    /// </summary>
    /// <param name="since">Only logs since this time.</param>
    /// <param name="until">Only logs until this time.</param>
    /// <param name="timestampsEnabled">Determines whether every log line contains a timestamp or not.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Returns the container logs.</returns>
    new Task<(string Stdout, string Stderr)> GetLogsAsync(DateTime since = default, DateTime until = default, bool timestampsEnabled = true, CancellationToken ct = default);

    /// <summary>
    /// Starts the container.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been started.</returns>
    /// <exception cref="OperationCanceledException">Thrown when a Docker API call gets canceled.</exception>
    /// <exception cref="TaskCanceledException">Thrown when a Testcontainers task gets canceled.</exception>
    /// <exception cref="TimeoutException">Thrown when the wait strategy task gets canceled or the timeout expires.</exception>
    new Task StartAsync(CancellationToken ct = default);

    /// <summary>
    /// Stops the container.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been stopped.</returns>
    /// <exception cref="OperationCanceledException">Thrown when a Docker API call gets canceled.</exception>
    /// <exception cref="TaskCanceledException">Thrown when a Testcontainers task gets canceled.</exception>
    new Task StopAsync(CancellationToken ct = default);

    /// <summary>
    /// Copies a file to the container.
    /// </summary>
    /// <param name="filePath">An absolute path as destination in the container.</param>
    /// <param name="fileContent">The byte array content of the file.</param>
    /// <param name="accessMode">The access mode for the file (default: 0600).</param>
    /// <param name="userId">The owner of the file.</param>
    /// <param name="groupId">The group of the file.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the file has been copied.</returns>
    /// <remarks>
    ///   <paramref name="accessMode" /> is a decimal value. Covert chmod (octal) to decimal.
    ///   <ul>
    ///     <li>777 octal 🠒 111_111_111 binary 🠒 511 decimal</li>
    ///     <li>755 octal 🠒 111_101_101 binary 🠒 493 decimal</li>
    ///     <li>644 octal 🠒 110_100_100 binary 🠒 420 decimal</li>
    ///   </ul>
    /// </remarks>
    new Task CopyFileAsync(string filePath, byte[] fileContent, int accessMode = 384, int userId = 0, int groupId = 0, CancellationToken ct = default);

    /// <summary>
    /// Reads a file from the container.
    /// </summary>
    /// <param name="filePath">An absolute path or a name value within the container.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the file has been read.</returns>
    new Task<byte[]> ReadFileAsync(string filePath, CancellationToken ct = default);

    /// <summary>
    /// Executes a command in the container.
    /// </summary>
    /// <param name="command">Shell command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the shell command has been executed.</returns>
    new Task<ExecResult> ExecAsync(IList<string> command, CancellationToken ct = default);
  }
}
