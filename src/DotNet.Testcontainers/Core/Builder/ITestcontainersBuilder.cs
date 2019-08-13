namespace DotNet.Testcontainers.Core.Builder
{
  using System;
  using DotNet.Testcontainers.Core.Containers;
  using DotNet.Testcontainers.Core.Images;
  using DotNet.Testcontainers.Core.Wait;
  using DotNet.Testcontainers.Diagnostics;

  public interface ITestcontainersBuilder<T>
  {
    ITestcontainersBuilder<T> ConfigureContainer(Action<T> configureContainer);

    /// <summary>
    /// Sets the Docker image, which is used to create the Testcontainer instances.
    /// </summary>
    /// <param name="image">Docker image name.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithImage(string image);

    /// <summary>
    /// Sets the Docker image, which is used to create the Testcontainer instances.
    /// </summary>
    /// <param name="image">Docker image instance.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithImage(IDockerImage image);

    /// <summary>
    /// Sets the name of the Testcontainer.
    /// </summary>
    /// <param name="name">Testcontainers name.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithName(string name);

    /// <summary>
    /// Overrides the working directory of the Testcontainer for the instruction sets.
    /// </summary>
    /// <param name="workingDirectory">Working directory.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithWorkingDirectory(string workingDirectory);

    /// <summary>
    /// Overrides the entrypoint of the Testcontainer to configure an executable.
    /// </summary>
    /// <param name="entrypoint">Entrypoint executable.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithEntrypoint(params string[] entrypoint);

    /// <summary>
    /// Overrides the command of the Testcontainer to provide defaults for an executing.
    /// </summary>
    /// <param name="command">List of commands, "executable", "param1", "param2" or "param1", "param2".</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithCommand(params string[] command);

    /// <summary>
    /// Exports the environment variable in the Testcontainer.
    /// </summary>
    /// <param name="name">Environment variable name.</param>
    /// <param name="value">Environment variable value.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithEnvironment(string name, string value);

    /// <summary>
    /// Adds an user-defined metadata to the Testcontainer.
    /// </summary>
    /// <param name="name">Label name.</param>
    /// <param name="value">Label value.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithLabel(string name, string value);

    /// <summary>
    /// Sets the port of the Testcontainer to expose, without publishing the port to the host system’s interfaces.
    /// </summary>
    /// <param name="port">Port to expose.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithExposedPort(int port);

    /// <summary>
    /// Exposes the port of the Testcontainer, without publishing the port to the host system’s interfaces.
    /// </summary>
    /// <param name="port">Port to expose.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithExposedPort(string port);

    /// <summary>
    /// Binds the port of the Testcontainer to the same port of the host machine.
    /// </summary>
    /// <param name="port">Port to bind between Testcontainer and host machine.</param>
    /// <param name="assignRandomHostPort">If true, Testcontainer will bind the port to a random host port, otherwise the host and container ports are the same.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithPortBinding(int port, bool assignRandomHostPort = false);

    /// <summary>
    /// Binds the port of the Testcontainer to the specified port of the host machine.
    /// </summary>
    /// <param name="hostPort">Port of the host machine.</param>
    /// <param name="containerPort">Port of the Testcontainer.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithPortBinding(int hostPort, int containerPort);

    /// <summary>
    /// Binds the port of the Testcontainer to the same port of the host machine.
    /// </summary>
    /// <param name="port">Port to bind between Testcontainer and host machine.</param>
    /// <param name="assignRandomHostPort">If true, Testcontainer will bind the port to a random host port, otherwise the host and container ports are the same.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithPortBinding(string port, bool assignRandomHostPort = false);

    /// <summary>
    /// Binds the port of the Testcontainer to the specified port of the host machine.
    /// </summary>
    /// <param name="hostPort">Port of the host machine.</param>
    /// <param name="containerPort">Port of the Testcontainer.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithPortBinding(string hostPort, string containerPort);

    /// <summary>
    /// Binds and mounts the specified host machine volume into the Testcontainer.
    /// </summary>
    /// <param name="source">An absolute path or a name value within the host machine.</param>
    /// <param name="destination">An absolute path as destination in the container.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithMount(string source, string destination);

    /// <summary>
    /// If true, Testcontainer will remove the Testcontainer on finalize. Otherwise, Testcontainer will keep the Testcontainer.
    /// </summary>
    /// <param name="cleanUp">True, Testcontainer will remove the Testcontainer on finalize. Otherwise, Testcontainer will keep it.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithCleanUp(bool cleanUp);

    /// <summary>
    /// Sets the output consumer to capture the Testcontainer stdout and stderr messages.
    /// </summary>
    /// <param name="outputConsumer">Output consumer to capture stdout and strerr.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithOutputConsumer(IOutputConsumer outputConsumer);

    /// <summary>
    /// Sets the wait strategy to complete the Testcontainer asynchronous start task.
    /// </summary>
    /// <param name="waitStrategy">Wait strategy to complete the Testcontainer start, default wait strategy implementation <see cref="WaitUntilContainerIsRunning"/>.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{T}"/>.</returns>
    ITestcontainersBuilder<T> WithWaitStrategy(IWaitUntil waitStrategy);

    /// <summary>
    /// Builds the instance of <see cref="IDockerContainer"/> with the given configuration.
    /// </summary>
    /// <returns>A configured instance of <see cref="IDockerContainer"/>.</returns>
    T Build();
  }
}
