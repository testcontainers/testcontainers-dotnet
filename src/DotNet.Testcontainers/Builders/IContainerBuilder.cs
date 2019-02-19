namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;

  public interface IContainerBuilder
  {
    /// <summary>
    /// Sets the Docker image, which is used to create the Docker container instances.
    /// </summary>
    /// <param name="image">Docker image name.</param>
    /// <returns>A configured instance of <see cref="IContainerBuilder"/>.</returns>
    IContainerBuilder WithImage(string image);

    /// <summary>
    /// Sets the Docker image, which is used to create the Docker container instances.
    /// </summary>
    /// <param name="image">Docker image instance.</param>
    /// <returns>A configured instance of <see cref="IContainerBuilder"/>.</returns>
    IContainerBuilder WithImage(IDockerImage image);

    /// <summary>
    /// Sets the port of the Docker container to expose, without publishing the port to the host system’s interfaces.
    /// </summary>
    /// <param name="port">Port to expose.</param>
    /// <returns>A configured instance of <see cref="IContainerBuilder"/>.</returns>
    IContainerBuilder WithExposedPort(int port);

    /// <summary>
    /// Exposes the port of the Docker container, without publishing the port to the host system’s interfaces.
    /// </summary>
    /// <param name="port">Port to expose.</param>
    /// <returns>A configured instance of <see cref="IContainerBuilder"/>.</returns>
    IContainerBuilder WithExposedPort(string port);

    /// <summary>
    /// Binds the port of the Docker container to the same port of the host machine.
    /// </summary>
    /// <param name="port">Port to bind between Docker container and host machine.</param>
    /// <returns>A configured instance of <see cref="IContainerBuilder"/>.</returns>
    IContainerBuilder WithPortBindings(int port);

    /// <summary>
    /// Binds the port of the Docker container to the specified port of the host machine.
    /// </summary>
    /// <param name="hostPort">Port of the host machine.</param>
    /// <param name="containerPort">Port of the Docker container.</param>
    /// <returns>A configured instance of <see cref="IContainerBuilder"/>.</returns>
    IContainerBuilder WithPortBindings(int hostPort, int containerPort);

    /// <summary>
    /// Binds the port of the Docker container to the same port of the host machine.
    /// </summary>
    /// <param name="port">Port to bind between Docker container and host machine.</param>
    /// <returns>A configured instance of <see cref="IContainerBuilder"/>.</returns>
    IContainerBuilder WithPortBindings(string port);

    /// <summary>
    /// Binds the port of the Docker container to the specified port of the host machine.
    /// </summary>
    /// <param name="hostPort">Port of the host machine.</param>
    /// <param name="containerPort">Port of the Docker container.</param>
    /// <returns>A configured instance of <see cref="IContainerBuilder"/>.</returns>
    IContainerBuilder WithPortBindings(string hostPort, string containerPort);

    IContainerBuilder WaitingFor();

    /// <summary>
    /// Builds the instance of <see cref="IDockerContainer"/> with the given configuration.
    /// </summary>
    /// <returns>A configured instance of <see cref="IDockerContainer"/>.</returns>
    IDockerContainer Build();
  }
}
