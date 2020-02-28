namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.OutputConsumers;
  using DotNet.Testcontainers.Images.Configurations;

  internal interface ITestcontainersClient
  {
    /// <summary>
    /// True if the container is running inside another Docker container, otherwise false.
    /// </summary>
    bool IsRunningInsideDocker { get; }

    /// <summary>
    /// Returns true if the Docker Windows engine is enabled, otherwise false.
    /// </summary>
    /// <returns>Task that returns true if the Docker Windows engine is enabled, otherwise false.</returns>
    Task<bool> GetIsWindowsEngineEnabled(CancellationToken ct = default);

    /// <summary>
    /// Gets the Testcontainer exit code.
    /// </summary>
    /// <param name="id">Docker container id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that gets the Testcontainer exit code.</returns>
    Task<long> GetContainerExitCode(string id, CancellationToken ct = default);

    /// <summary>
    /// Gets the Testcontainer.
    /// </summary>
    /// <param name="id">Docker container id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that gets the Testcontainer.</returns>
    Task<ContainerListResponse> GetContainer(string id, CancellationToken ct = default);

    /// <summary>
    /// Starts a container.
    /// </summary>
    /// <param name="id">Docker container id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been started.</returns>
    Task StartAsync(string id, CancellationToken ct = default);

    /// <summary>
    /// Stops a container.
    /// </summary>
    /// <param name="id">Docker container id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been stopped.</returns>
    Task StopAsync(string id, CancellationToken ct = default);

    /// <summary>
    /// Removes a container.
    /// </summary>
    /// <param name="id">Docker container id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been removed.</returns>
    Task RemoveAsync(string id, CancellationToken ct = default);

    /// <summary>
    /// Attaches to a container and copies the output to <see cref="IOutputConsumer" />.
    /// </summary>
    /// <param name="id">Docker container id.</param>
    /// <param name="outputConsumer">Output consumer.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that copies the Docker container output to <see cref="IOutputConsumer" />.</returns>
    Task AttachAsync(string id, IOutputConsumer outputConsumer, CancellationToken ct = default);

    /// <summary>
    /// Executes a command in a container.
    /// </summary>
    /// <param name="id">Docker container id.</param>
    /// <param name="command">Shell command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the shell command has been executed.</returns>
    Task<long> ExecAsync(string id, IList<string> command, CancellationToken ct = default);

    /// <summary>
    /// Creates a container.
    /// </summary>
    /// <param name="configuration">Testcontainer configuration.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the Docker container has been created.</returns>
    Task<string> RunAsync(ITestcontainersConfiguration configuration, CancellationToken ct = default);

    /// <summary>
    /// Builds a Docker image from a Dockerfile.
    /// </summary>
    /// <param name="configuration">Dockerfile configuration.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the Docker image has been built.</returns>
    Task<string> BuildAsync(IImageFromDockerfileConfiguration configuration, CancellationToken ct = default);
  }
}
