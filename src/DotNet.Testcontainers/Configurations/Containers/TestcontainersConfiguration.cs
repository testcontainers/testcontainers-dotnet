namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Network;

  /// <inheritdoc cref="ITestcontainersConfiguration" />
  public readonly struct TestcontainersConfiguration : ITestcontainersConfiguration
  {
#pragma warning disable S107

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersConfiguration" /> struct.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <param name="dockerRegistryAuthenticationConfigurations">The Docker registry authentication configuration.</param>
    /// <param name="image">The Docker image.</param>
    /// <param name="name">The name.</param>
    /// <param name="hostname">The hostname.</param>
    /// <param name="workingDirectory">The working directory.</param>
    /// <param name="entrypoint">The entrypoint.</param>
    /// <param name="command">The command.</param>
    /// <param name="environments">The environment variables.</param>
    /// <param name="labels">The labels.</param>
    /// <param name="exposedPorts">The exposed ports.</param>
    /// <param name="portBindings">The port bindings.</param>
    /// <param name="mounts">The volumes.</param>
    /// <param name="networks">The networks.</param>
    /// <param name="outputConsumer">The output consumer.</param>
    /// <param name="waitStrategies">The wait strategies.</param>
    /// <param name="startupCallback">The startup callback.</param>
    /// <param name="autoRemove">A value indicating whether the Testcontainer is removed by the Docker daemon or not.</param>
    /// <param name="privileged">A value indicating whether the Testcontainer has extended  privilegesor not.</param>
    public TestcontainersConfiguration(
      Uri endpoint,
      IDockerRegistryAuthenticationConfiguration dockerRegistryAuthenticationConfigurations,
      IDockerImage image,
      string name,
      string hostname,
      string workingDirectory,
      IEnumerable<string> entrypoint,
      IEnumerable<string> command,
      IReadOnlyDictionary<string, string> environments,
      IReadOnlyDictionary<string, string> labels,
      IReadOnlyDictionary<string, string> exposedPorts,
      IReadOnlyDictionary<string, string> portBindings,
      IEnumerable<IMount> mounts,
      IEnumerable<IDockerNetwork> networks,
      IOutputConsumer outputConsumer,
      IEnumerable<IWaitUntil> waitStrategies,
      Func<ITestcontainersContainer, CancellationToken, Task> startupCallback,
      bool? autoRemove,
      bool? privileged)
    {
      this.AutoRemove = autoRemove;
      this.Privileged = privileged;
      this.Endpoint = endpoint;
      this.DockerRegistryAuthConfig = dockerRegistryAuthenticationConfigurations;
      this.Image = image;
      this.Name = name;
      this.Hostname = hostname;
      this.WorkingDirectory = workingDirectory;
      this.Entrypoint = entrypoint;
      this.Command = command;
      this.Environments = environments;
      this.Labels = labels;
      this.ExposedPorts = exposedPorts;
      this.PortBindings = portBindings;
      this.Mounts = mounts;
      this.Networks = networks;
      this.OutputConsumer = outputConsumer;
      this.WaitStrategies = waitStrategies;
      this.StartupCallback = startupCallback;
    }

#pragma warning restore S107

    /// <inheritdoc />
    public bool? AutoRemove { get; }

    /// <inheritdoc />
    public bool? Privileged { get; }

    /// <inheritdoc />
    public Uri Endpoint { get; }

    /// <inheritdoc />
    public IDockerRegistryAuthenticationConfiguration DockerRegistryAuthConfig { get; }

    /// <inheritdoc />
    public IDockerImage Image { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Hostname { get; }

    /// <inheritdoc />
    public string WorkingDirectory { get; }

    /// <inheritdoc />
    public IEnumerable<string> Entrypoint { get; }

    /// <inheritdoc />
    public IEnumerable<string> Command { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Environments { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Labels { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> ExposedPorts { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> PortBindings { get; }

    /// <inheritdoc />
    public IEnumerable<IMount> Mounts { get; }

    /// <inheritdoc />>
    public IEnumerable<IDockerNetwork> Networks { get; }

    /// <inheritdoc />
    public IOutputConsumer OutputConsumer { get; }

    /// <inheritdoc />
    public IEnumerable<IWaitUntil> WaitStrategies { get; }

    /// <inheritdoc />
    public Func<ITestcontainersContainer, CancellationToken, Task> StartupCallback { get; }
  }
}
