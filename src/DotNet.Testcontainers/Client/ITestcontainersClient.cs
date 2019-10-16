namespace DotNet.Testcontainers.Client
{
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.OutputConsumers;
  using DotNet.Testcontainers.Images.Configurations;

  internal interface ITestcontainersClient
  {
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
    /// Builds a Docker image from a Dockerfile.
    /// </summary>
    /// <param name="config">Dockerfile configuration.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the Docker image has been built.</returns>
    Task<string> BuildAsync(ImageFromDockerfileConfiguration config, CancellationToken ct = default);

    /// <summary>
    /// Creates a container.
    /// </summary>
    /// <param name="config">Testcontainer configuration.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the Docker container has been created.</returns>
    Task<string> RunAsync(TestcontainersConfiguration config, CancellationToken ct = default);
  }
}
