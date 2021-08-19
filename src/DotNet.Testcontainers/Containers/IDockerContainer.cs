namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using JetBrains.Annotations;

  /// <summary>
  /// This class represents a Docker container.
  /// </summary>
  public interface IDockerContainer : IRunningDockerContainer, IAsyncDisposable
  {
    /// <summary>
    /// Gets the Testcontainer exit code.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Returns the Docker container exit code.</returns>
    Task<long> GetExitCode(CancellationToken ct = default);

    /// <summary>
    /// Starts the Testcontainer.
    /// </summary>
    /// <remarks>
    /// If the image does not exist, it will be downloaded automatically. Non-existing containers are created at first start.
    /// </remarks>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous start operation of a Testcontainer.</returns>
    /// <exception cref="OperationCanceledException">Thrown when a Docker API call gets canceled.</exception>
    /// <exception cref="TaskCanceledException">Thrown when a Testcontainer task gets canceled.</exception>
    /// <exception cref="TimeoutException">Thrown when the wait strategy task gets canceled or the timeout expires.</exception>
    Task StartAsync(CancellationToken ct = default);

    /// <summary>
    /// Stops the Testcontainer.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous stop operation of a Testcontainer.</returns>
    /// <exception cref="OperationCanceledException">Thrown when a Docker API call gets canceled.</exception>
    /// <exception cref="TaskCanceledException">Thrown when a Testcontainer task gets canceled.</exception>
    Task StopAsync(CancellationToken ct = default);
  }

  /// <summary>
  /// This class represents a running Docker container.
  /// </summary>
  public interface IRunningDockerContainer
  {
    /// <summary>
    /// Gets the Testcontainer id.
    /// </summary>
    /// <value>
    /// Returns the Docker container id if present or an empty string instead.
    /// </value>
    /// <exception cref="InvalidOperationException">If container was not created.</exception>
    [NotNull]
    string Id { get; }

    /// <summary>
    /// Gets the Testcontainer name.
    /// </summary>
    /// <value>
    /// Returns the Docker container name if present or an empty string instead.
    /// </value>
    /// <exception cref="InvalidOperationException">If container was not created.</exception>
    [NotNull]
    string Name { get; }

    /// <summary>
    /// Gets the Testcontainer ip address.
    /// </summary>
    /// <value>
    /// Returns the Docker container ip address if present or an empty string instead.
    /// </value>
    /// <exception cref="InvalidOperationException">If container was not created.</exception>
    [NotNull]
    string IpAddress { get; }

    /// <summary>
    /// Gets the Testcontainer mac address.
    /// </summary>
    /// <value>
    /// Returns the Docker container mac address if present or an empty string instead.
    /// </value>
    /// <exception cref="InvalidOperationException">If container was not created.</exception>
    [NotNull]
    string MacAddress { get; }

    /// <summary>
    /// Gets the Testcontainer hostname.
    /// </summary>
    /// <value>
    /// Returns the Docker container hostname if present or an empty string instead.
    /// </value>
    /// <exception cref="InvalidOperationException">If container was not created.</exception>
    [NotNull]
    string Hostname { get; }

    /// <summary>
    /// Gets the public host port associated with the private container port.
    /// </summary>
    /// <param name="privatePort">Private container port.</param>
    /// <returns>Returns the public host port associated with the private container port.</returns>
    /// <exception cref="InvalidOperationException">If container was not created.</exception>
    ushort GetMappedPublicPort(int privatePort);

    /// <summary>
    /// Gets the public host port associated with the private container port.
    /// </summary>
    /// <param name="privatePort">Private container port.</param>
    /// <returns>Returns the public host port associated with the private container port.</returns>
    /// <exception cref="InvalidOperationException">If container was not created.</exception>
    ushort GetMappedPublicPort(string privatePort);

    /// <summary>
    /// Copies a file into the container.
    /// </summary>
    /// <param name="filePath">Path to the file in the container.</param>
    /// <param name="fileContent">Content of the file as bytes.</param>
    /// <param name="accessMode">Access mode for the file (default: 0600).</param>
    /// <param name="userId">Owner of the file.</param>
    /// <param name="groupId">Group of the file.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the file has been copied.</returns>
    Task CopyFileAsync(string filePath, byte[] fileContent, int accessMode = 384, int userId = 0, int groupId = 0, CancellationToken ct = default);

    /// <summary>
    /// Executes a command in the running Testcontainer.
    /// </summary>
    /// <param name="command">Shell command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the shell command has been executed.</returns>
    Task<ExecResult> ExecAsync(IList<string> command, CancellationToken ct = default);
  }
}
