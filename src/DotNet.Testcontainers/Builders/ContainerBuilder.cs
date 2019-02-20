namespace DotNet.Testcontainers.Builders
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;

  public abstract class ContainerBuilder
  {
    public abstract IDockerImage Image { get; }

    public abstract IReadOnlyDictionary<string, string> ExposedPorts { get; }

    public abstract IReadOnlyDictionary<string, string> PortBindings { get; }

    /// <summary>
    /// Sets the Docker image, which is used to create the Docker container instances.
    /// </summary>
    /// <param name="image">Docker image name.</param>
    /// <returns>A configured instance of <see cref="ContainerBuilder"/>.</returns>
    public abstract ContainerBuilder WithImage(string image);

    /// <summary>
    /// Sets the Docker image, which is used to create the Docker container instances.
    /// </summary>
    /// <param name="image">Docker image instance.</param>
    /// <returns>A configured instance of <see cref="ContainerBuilder"/>.</returns>
    public abstract ContainerBuilder WithImage(IDockerImage image);

    /// <summary>
    /// Sets the port of the Docker container to expose, without publishing the port to the host system’s interfaces.
    /// </summary>
    /// <param name="port">Port to expose.</param>
    /// <returns>A configured instance of <see cref="ContainerBuilder"/>.</returns>
    public abstract ContainerBuilder WithExposedPort(int port);

    /// <summary>
    /// Exposes the port of the Docker container, without publishing the port to the host system’s interfaces.
    /// </summary>
    /// <param name="port">Port to expose.</param>
    /// <returns>A configured instance of <see cref="ContainerBuilder"/>.</returns>
    public abstract ContainerBuilder WithExposedPort(string port);

    /// <summary>
    /// Binds the port of the Docker container to the same port of the host machine.
    /// </summary>
    /// <param name="port">Port to bind between Docker container and host machine.</param>
    /// <returns>A configured instance of <see cref="ContainerBuilder"/>.</returns>
    public abstract ContainerBuilder WithPortBinding(int port);

    /// <summary>
    /// Binds the port of the Docker container to the specified port of the host machine.
    /// </summary>
    /// <param name="hostPort">Port of the host machine.</param>
    /// <param name="containerPort">Port of the Docker container.</param>
    /// <returns>A configured instance of <see cref="ContainerBuilder"/>.</returns>
    public abstract ContainerBuilder WithPortBinding(int hostPort, int containerPort);

    /// <summary>
    /// Binds the port of the Docker container to the same port of the host machine.
    /// </summary>
    /// <param name="port">Port to bind between Docker container and host machine.</param>
    /// <returns>A configured instance of <see cref="ContainerBuilder"/>.</returns>
    public abstract ContainerBuilder WithPortBinding(string port);

    /// <summary>
    /// Binds the port of the Docker container to the specified port of the host machine.
    /// </summary>
    /// <param name="hostPort">Port of the host machine.</param>
    /// <param name="containerPort">Port of the Docker container.</param>
    /// <returns>A configured instance of <see cref="ContainerBuilder"/>.</returns>
    public abstract ContainerBuilder WithPortBinding(string hostPort, string containerPort);

    /// <summary>
    /// Builds the instance of <see cref="IDockerContainer"/> with the given configuration.
    /// </summary>
    /// <returns>A configured instance of <see cref="IDockerContainer"/>.</returns>
    public abstract IDockerContainer Build();
  }
}
