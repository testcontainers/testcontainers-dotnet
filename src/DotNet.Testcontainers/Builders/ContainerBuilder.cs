namespace DotNet.Testcontainers.Builders
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;

  public abstract class ContainerBuilder
  {
    internal abstract bool CleanUp { get; }

    internal abstract string Name { get; }

    internal abstract IDockerImage Image { get; }

    internal abstract IReadOnlyDictionary<string, string> ExposedPorts { get; }

    internal abstract IReadOnlyDictionary<string, string> PortBindings { get; }

    internal abstract IReadOnlyDictionary<string, string> Volumes { get; }

    /// <summary>
    /// If true, Testcontainers will remove the Docker container on finalize. Otherwise, Testcontainers will keep the Docker container.
    /// </summary>
    /// <param name="cleanUp">True, Testcontainers will remove the Docker container on finalize. Otherwise, Otherwise, Testcontainers will keep it.</param>
    /// <returns>A configured instance of <see cref="ContainerBuilder"/>.</returns>
    public abstract ContainerBuilder WithCleanUp(bool cleanUp);

    /// <summary>
    /// Sets the name of the Docker container.
    /// </summary>
    /// <param name="name">Docker container name.</param>
    /// <returns>A configured instance of <see cref="ContainerBuilder"/>.</returns>
    public abstract ContainerBuilder WithName(string name);

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
    /// Binds and mounts the specified host machine volume into the Docker container.
    /// </summary>
    /// <param name="source">An absolute path or a name value within the host machine.</param>
    /// <param name="destination">An absolute path as destination in the container.</param>
    /// <returns>A configured instance of <see cref="ContainerBuilder"/>.</returns>
    public abstract ContainerBuilder WithVolume(string source, string destination);

    /// <summary>
    /// Builds the instance of <see cref="IDockerContainer"/> with the given configuration.
    /// </summary>
    /// <returns>A configured instance of <see cref="IDockerContainer"/>.</returns>
    public abstract IDockerContainer Build();
  }
}
