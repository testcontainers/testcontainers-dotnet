namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <summary>
  /// This class represents a Docker container.
  /// </summary>
  [PublicAPI]
  public interface IDockerContainer : IRunningDockerContainer, IAsyncDisposable
  {
    /// <summary>
    /// Gets the Testcontainers exit code.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Returns the Docker container exit code.</returns>
    [PublicAPI]
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
    /// <exception cref="TaskCanceledException">Thrown when a Testcontainers task gets canceled.</exception>
    /// <exception cref="TimeoutException">Thrown when the wait strategy task gets canceled or the timeout expires.</exception>
    [PublicAPI]
    Task StartAsync(CancellationToken ct = default);

    /// <summary>
    /// Stops the Testcontainer.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous stop operation of a Testcontainer.</returns>
    /// <exception cref="OperationCanceledException">Thrown when a Docker API call gets canceled.</exception>
    /// <exception cref="TaskCanceledException">Thrown when a Testcontainers task gets canceled.</exception>
    [PublicAPI]
    Task StopAsync(CancellationToken ct = default);
  }

  /// <summary>
  /// This class represents a running Docker container.
  /// </summary>
  [PublicAPI]
  public interface IRunningDockerContainer
  {
    /// <summary>
    /// Gets the Testcontainers id.
    /// </summary>
    /// <value>
    /// Returns the Docker container id if present or an empty string instead.
    /// </value>
    /// <exception cref="InvalidOperationException">If container was not created.</exception>
    [PublicAPI]
    [NotNull]
    string Id { get; }

    /// <summary>
    /// Gets the Testcontainers name.
    /// </summary>
    /// <value>
    /// Returns the Docker container name if present or an empty string instead.
    /// </value>
    /// <exception cref="InvalidOperationException">If container was not created.</exception>
    [PublicAPI]
    [NotNull]
    string Name { get; }

    /// <summary>
    /// Gets the Testcontainers ip address.
    /// </summary>
    /// <value>
    /// Returns the Docker container ip address if present or an empty string instead.
    /// </value>
    /// <exception cref="InvalidOperationException">If container was not created.</exception>
    [PublicAPI]
    [NotNull]
    string IpAddress { get; }

    /// <summary>
    /// Gets the Testcontainers mac address.
    /// </summary>
    /// <value>
    /// Returns the Docker container mac address if present or an empty string instead.
    /// </value>
    /// <exception cref="InvalidOperationException">If container was not created.</exception>
    [PublicAPI]
    [NotNull]
    string MacAddress { get; }

    /// <summary>
    /// Gets the Testcontainers hostname.
    /// </summary>
    /// <value>
    /// Returns the Docker container hostname if present or an empty string instead.
    /// </value>
    /// <exception cref="InvalidOperationException">If container was not created.</exception>
    [PublicAPI]
    [NotNull]
    string Hostname { get; }

    /// <summary>
    /// Gets the Testcontainers image name.
    /// </summary>
    /// <value>
    /// Returns the Docker image.
    /// </value>
    [PublicAPI]
    [NotNull]
    IDockerImage Image { get; }

    /// <summary>
    /// Gets the Testcontainers state.
    /// </summary>
    /// <value>
    /// Returns the Docker container state.
    /// </value>
    [PublicAPI]
    TestcontainersState State { get; }

    /// <summary>
    /// Gets the public host port associated with the private container port.
    /// </summary>
    /// <param name="privatePort">Private container port.</param>
    /// <returns>Returns the public host port associated with the private container port.</returns>
    /// <exception cref="InvalidOperationException">If container was not created.</exception>
    [PublicAPI]
    ushort GetMappedPublicPort(int privatePort);

    /// <summary>
    /// Gets the public host port associated with the private container port.
    /// </summary>
    /// <param name="privatePort">Private container port.</param>
    /// <returns>Returns the public host port associated with the private container port.</returns>
    /// <exception cref="InvalidOperationException">If container was not created.</exception>
    [PublicAPI]
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
    /// <remarks>
    /// <see cref="accessMode" /> is a decimal value. Covert chmod (octal) to decimal.
    /// <ul>
    ///   <li>777 octal 🠒 111_111_111 binary 🠒 511 decimal</li>
    ///   <li>755 octal 🠒 111_101_101 binary 🠒 493 decimal</li>
    ///   <li>644 octal 🠒 110_100_100 binary 🠒 420 decimal</li>
    /// </ul>
    /// </remarks>
    [PublicAPI]
    Task CopyFileAsync(string filePath, byte[] fileContent, int accessMode = 384, int userId = 0, int groupId = 0, CancellationToken ct = default);

    /// <summary>
    /// Reads a file from the container.
    /// </summary>
    /// <param name="filePath">Path to the file in the container.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the file has been read.</returns>
    [PublicAPI]
    Task<byte[]> ReadFileAsync(string filePath, CancellationToken ct = default);

    /// <summary>
    /// Executes a command in the running Testcontainer.
    /// </summary>
    /// <param name="command">Shell command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the shell command has been executed.</returns>
    [PublicAPI]
    Task<ExecResult> ExecAsync(IList<string> command, CancellationToken ct = default);
  }
}
