namespace DotNet.Testcontainers.Core.Builder
{
  using DotNet.Testcontainers.Core.Containers;
  using DotNet.Testcontainers.Core.Images;

  public interface ITestcontainersBuilder
  {
    /// <summary>
    /// Sets the Docker image, which is used to create the Docker container instances.
    /// </summary>
    /// <param name="image">Docker image name.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder"/>.</returns>
    ITestcontainersBuilder WithImage(string image);

    /// <summary>
    /// Sets the Docker image, which is used to create the Docker container instances.
    /// </summary>
    /// <param name="image">Docker image instance.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder"/>.</returns>
    ITestcontainersBuilder WithImage(IDockerImage image);

    /// <summary>
    /// Sets the name of the Docker container.
    /// </summary>
    /// <param name="name">Docker container name.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder"/>.</returns>
    ITestcontainersBuilder WithName(string name);

    /// <summary>
    /// Sets the port of the Docker container to expose, without publishing the port to the host system’s interfaces.
    /// </summary>
    /// <param name="port">Port to expose.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder"/>.</returns>
    ITestcontainersBuilder WithExposedPort(int port);

    /// <summary>
    /// Exposes the port of the Docker container, without publishing the port to the host system’s interfaces.
    /// </summary>
    /// <param name="port">Port to expose.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder"/>.</returns>
    ITestcontainersBuilder WithExposedPort(string port);

    /// <summary>
    /// Binds the port of the Docker container to the same port of the host machine.
    /// </summary>
    /// <param name="port">Port to bind between Docker container and host machine.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder"/>.</returns>
    ITestcontainersBuilder WithPortBinding(int port);

    /// <summary>
    /// Binds the port of the Docker container to the specified port of the host machine.
    /// </summary>
    /// <param name="hostPort">Port of the host machine.</param>
    /// <param name="containerPort">Port of the Docker container.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder"/>.</returns>
    ITestcontainersBuilder WithPortBinding(int hostPort, int containerPort);

    /// <summary>
    /// Binds the port of the Docker container to the same port of the host machine.
    /// </summary>
    /// <param name="port">Port to bind between Docker container and host machine.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder"/>.</returns>
    ITestcontainersBuilder WithPortBinding(string port);

    /// <summary>
    /// Binds the port of the Docker container to the specified port of the host machine.
    /// </summary>
    /// <param name="hostPort">Port of the host machine.</param>
    /// <param name="containerPort">Port of the Docker container.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder"/>.</returns>
    ITestcontainersBuilder WithPortBinding(string hostPort, string containerPort);

    /// <summary>
    /// Binds and mounts the specified host machine volume into the Docker container.
    /// </summary>
    /// <param name="source">An absolute path or a name value within the host machine.</param>
    /// <param name="destination">An absolute path as destination in the container.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder"/>.</returns>
    ITestcontainersBuilder WithMount(string source, string destination);

    /// <summary>
    /// Adds a command to the Docker container to provide defaults for an executing.
    /// </summary>
    /// <param name="command">List of commands, "executable", "param1", "param2" or "param1", "param2".</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder"/>.</returns>
    ITestcontainersBuilder WithCommand(params string[] command);

    /// <summary>
    /// If true, Testcontainers will remove the Docker container on finalize. Otherwise, Testcontainers will keep the Docker container.
    /// </summary>
    /// <param name="cleanUp">True, Testcontainers will remove the Docker container on finalize. Otherwise, Otherwise, Testcontainers will keep it.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder"/>.</returns>
    ITestcontainersBuilder WithCleanUp(bool cleanUp);

    /// <summary>
    /// Builds the instance of <see cref="IDockerContainer"/> with the given configuration.
    /// </summary>
    /// <returns>A configured instance of <see cref="IDockerContainer"/>.</returns>
    IDockerContainer Build();
  }
}
