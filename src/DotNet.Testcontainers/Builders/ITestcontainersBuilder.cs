namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Network;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Testcontainer builder.
  /// </summary>
  /// <typeparam name="TDockerContainer">Type of <see cref="ITestcontainersContainer" />.</typeparam>
  public interface ITestcontainersBuilder<out TDockerContainer>
    where TDockerContainer : ITestcontainersContainer
  {
    /// <summary>
    /// Sets the module configuration of the Testcontainer to override custom properties.
    /// </summary>
    /// <param name="moduleConfiguration">Module configuration action.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> ConfigureContainer(Action<TDockerContainer> moduleConfiguration);

    /// <summary>
    /// Sets the Docker image, which is used to create the Testcontainer instances.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithImage(string image);

    /// <summary>
    /// Sets the Docker image, which is used to create the Testcontainer instances.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithImage(IDockerImage image);

    /// <summary>
    /// Sets the name of the Testcontainer.
    /// </summary>
    /// <param name="name">Testcontainers name.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithName(string name);

    /// <summary>
    /// Sets the hostname of the Testcontainer.
    /// </summary>
    /// <param name="hostname">Testcontainers hostname.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    ITestcontainersBuilder<TDockerContainer> WithHostname(string hostname);

    /// <summary>
    /// Overrides the working directory of the Testcontainer for the instruction sets.
    /// </summary>
    /// <param name="workingDirectory">Working directory.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithWorkingDirectory(string workingDirectory);

    /// <summary>
    /// Overrides the entrypoint of the Testcontainer to configure an executable.
    /// </summary>
    /// <param name="entrypoint">Entrypoint executable.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithEntrypoint(params string[] entrypoint);

    /// <summary>
    /// Overrides the command of the Testcontainer to provide defaults for an executing.
    /// </summary>
    /// <param name="command">List of commands, "executable", "param1", "param2" or "param1", "param2".</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithCommand(params string[] command);

    /// <summary>
    /// Exports the environment variable in the Testcontainer.
    /// </summary>
    /// <param name="name">Environment variable name.</param>
    /// <param name="value">Environment variable value.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithEnvironment(string name, string value);

    /// <summary>
    /// Adds user-defined metadata to the Testcontainer.
    /// </summary>
    /// <param name="name">Label name.</param>
    /// <param name="value">Label value.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithLabel(string name, string value);

    /// <summary>
    /// Sets the port of the Testcontainer to expose, without publishing the port to the host system’s interfaces.
    /// </summary>
    /// <param name="port">Port to expose.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithExposedPort(int port);

    /// <summary>
    /// Exposes the port of the Testcontainer, without publishing the port to the host system’s interfaces.
    /// </summary>
    /// <param name="port">Port to expose.</param>
    /// <remarks>Append /tcp|udp|sctp to change the protocol e.g. "53/udp". Default: tcp.</remarks>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithExposedPort(string port);

    /// <summary>
    /// Binds the port of the Testcontainer to the same port of the host machine.
    /// </summary>
    /// <param name="port">Port to bind between Testcontainer and host machine.</param>
    /// <param name="assignRandomHostPort">If true, Testcontainer will bind the port to a random host port, otherwise the host and container ports are the same.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithPortBinding(int port, bool assignRandomHostPort = false);

    /// <summary>
    /// Binds the port of the Testcontainer to the specified port of the host machine.
    /// </summary>
    /// <param name="hostPort">Port of the host machine.</param>
    /// <param name="containerPort">Port of the Testcontainer.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithPortBinding(int hostPort, int containerPort);

    /// <summary>
    /// Binds the port of the Testcontainer to the same port of the host machine.
    /// </summary>
    /// <param name="port">Port to bind between Testcontainer and host machine.</param>
    /// <param name="assignRandomHostPort">If true, Testcontainer will bind the port to a random host port, otherwise the host and container ports are the same.</param>
    /// <remarks>Append /tcp|udp|sctp to change the protocol e.g. "53/udp". Default: tcp.</remarks>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithPortBinding(string port, bool assignRandomHostPort = false);

    /// <summary>
    /// Binds the port of the Testcontainer to the specified port of the host machine.
    /// </summary>
    /// <param name="hostPort">Port of the host machine.</param>
    /// <param name="containerPort">Port of the Testcontainer.</param>
    /// <remarks>Append /tcp|udp|sctp to <see cref="containerPort" /> to change the protocol e.g. "53/udp". Default: tcp.</remarks>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithPortBinding(string hostPort, string containerPort);

    /// <summary>
    /// Binds and mounts the specified host machine volume into the Testcontainer.
    /// </summary>
    /// <param name="source">An absolute path or a name value within the host machine.</param>
    /// <param name="destination">An absolute path as destination in the container.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithMount(string source, string destination);

    /// <summary>
    /// Binds and mounts the specified host machine volume into the Testcontainer.
    /// </summary>
    /// <param name="source">An absolute path or a name value within the host machine.</param>
    /// <param name="destination">An absolute path as destination in the container.</param>
    /// <param name="accessMode">Volume access mode.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithMount(string source, string destination, AccessMode accessMode);

    /// <summary>
    /// Connects to the specified network.
    /// </summary>
    /// <param name="id">Id of the network to connect to.</param>
    /// <param name="name">Name of the network to connect to.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithNetwork(string id, string name);

    /// <summary>
    /// Connects to the specified network.
    /// </summary>
    /// <param name="dockerNetwork">Network to connect container to.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithNetwork(IDockerNetwork dockerNetwork);

    /// <summary>
    /// If true, Testcontainer will remove the Testcontainer on finalize. Otherwise, Testcontainer will keep the Testcontainer.
    /// </summary>
    /// <param name="cleanUp">True, Testcontainer will remove the Testcontainer on finalize. Otherwise, Testcontainer will keep it.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithCleanUp(bool cleanUp);

    /// <summary>
    /// Sets the Docker API endpoint.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithDockerEndpoint(string endpoint);

    /// <summary>
    /// Sets the Docker registry authentication configuration to authenticate against private Docker registries.
    /// </summary>
    /// <param name="registryEndpoint">Docker registry endpoint.</param>
    /// <param name="username">Username to authenticate.</param>
    /// <param name="password">Password to authenticate.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    [Obsolete("Be aware we will replace this method in the future. Instead, we will use the local Docker credential store. Then, no additional configurations are necessary.")]
    ITestcontainersBuilder<TDockerContainer> WithRegistryAuthentication(string registryEndpoint, string username, string password);

    /// <summary>
    /// Sets the output consumer to capture the Testcontainer stdout and stderr messages.
    /// </summary>
    /// <param name="outputConsumer">Output consumer to capture stdout and stderr.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithOutputConsumer(IOutputConsumer outputConsumer);

    /// <summary>
    /// Sets the wait strategies to complete the Testcontainer asynchronous start task.
    /// </summary>
    /// <param name="waitStrategy">Wait strategy to complete the Testcontainer start, default wait strategy implementation is <see cref="UntilContainerIsRunning" />.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    /// <remarks>Multiple wait strategies are executed one after the other.</remarks>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithWaitStrategy(IWaitForContainerOS waitStrategy);

    /// <summary>
    /// Sets the startup callback to invoke after the Testcontainer start.
    /// </summary>
    /// <param name="startupCallback">The callback method to invoke.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    /// <remarks>Is invoked once after the Testcontainer is started and before the wait strategies are executed.</remarks>
    [PublicAPI]
    ITestcontainersBuilder<TDockerContainer> WithStartupCallback(Func<IRunningDockerContainer, CancellationToken, Task> startupCallback);

    /// <summary>
    /// Builds the instance of <see cref="ITestcontainersContainer" /> with the given configuration.
    /// </summary>
    /// <returns>A configured instance of <see cref="ITestcontainersContainer" />.</returns>
    [PublicAPI]
    TDockerContainer Build();
  }
}
