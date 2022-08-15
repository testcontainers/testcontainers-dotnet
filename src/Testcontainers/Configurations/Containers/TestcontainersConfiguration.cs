namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Networks;

  /// <inheritdoc cref="ITestcontainersConfiguration" />
  internal sealed class TestcontainersConfiguration : DockerResourceConfiguration, ITestcontainersConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersConfiguration" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker container configuration.</param>
    public TestcontainersConfiguration(IDockerResourceConfiguration dockerResourceConfiguration)
      : base(dockerResourceConfiguration)
    {
    }

#pragma warning disable S107

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersConfiguration" /> class.
    /// </summary>
    /// <param name="dockerEndpointAuthenticationConfiguration">The Docker endpoint authentication configuration.</param>
    /// <param name="dockerRegistryAuthenticationConfiguration">The Docker registry authentication configuration.</param>
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
    /// <param name="networkAliases">The container network aliases.</param>
    /// <param name="outputConsumer">The output consumer.</param>
    /// <param name="waitStrategies">The wait strategies.</param>
    /// <param name="parameterModifiers">The actions that modifies the <see cref="CreateContainerParameters" /> configuration.</param>
    /// <param name="startupCallback">The startup callback.</param>
    /// <param name="autoRemove">A value indicating whether the Testcontainer is removed by the Docker daemon or not.</param>
    /// <param name="privileged">A value indicating whether the Testcontainer has extended privileges or not.</param>
    public TestcontainersConfiguration(
      IDockerEndpointAuthenticationConfiguration dockerEndpointAuthenticationConfiguration = null,
      IDockerRegistryAuthenticationConfiguration dockerRegistryAuthenticationConfiguration = null,
      IDockerImage image = null,
      string name = null,
      string hostname = null,
      string workingDirectory = null,
      IEnumerable<string> entrypoint = null,
      IEnumerable<string> command = null,
      IReadOnlyDictionary<string, string> environments = null,
      IReadOnlyDictionary<string, string> labels = null,
      IReadOnlyDictionary<string, string> exposedPorts = null,
      IReadOnlyDictionary<string, string> portBindings = null,
      IEnumerable<IMount> mounts = null,
      IEnumerable<IDockerNetwork> networks = null,
      IEnumerable<string> networkAliases = null,
      IOutputConsumer outputConsumer = null,
      IEnumerable<IWaitUntil> waitStrategies = null,
      IReadOnlyList<Action<CreateContainerParameters>> parameterModifiers = null,
      Func<ITestcontainersContainer, CancellationToken, Task> startupCallback = null,
      bool? autoRemove = null,
      bool? privileged = null)
      : base(dockerEndpointAuthenticationConfiguration, labels)
    {
      this.AutoRemove = autoRemove;
      this.Privileged = privileged;
      this.DockerRegistryAuthConfig = dockerRegistryAuthenticationConfiguration;
      this.Image = image;
      this.Name = name;
      this.Hostname = hostname;
      this.WorkingDirectory = workingDirectory;
      this.Entrypoint = entrypoint;
      this.Command = command;
      this.Environments = environments;
      this.ExposedPorts = exposedPorts;
      this.PortBindings = portBindings;
      this.Mounts = mounts;
      this.Networks = networks;
      this.NetworkAliases = networkAliases;
      this.OutputConsumer = outputConsumer;
      this.WaitStrategies = waitStrategies;
      this.ParameterModifiers = parameterModifiers;
      this.StartupCallback = startupCallback;
    }

#pragma warning restore S107

    /// <inheritdoc />
    public bool? AutoRemove { get; }

    /// <inheritdoc />
    public bool? Privileged { get; }

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
    public IReadOnlyDictionary<string, string> ExposedPorts { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> PortBindings { get; }

    /// <inheritdoc />
    public IEnumerable<IMount> Mounts { get; }

    /// <inheritdoc />>
    public IEnumerable<IDockerNetwork> Networks { get; }

    /// <inheritdoc />>
    public IEnumerable<string> NetworkAliases { get; }

    /// <inheritdoc />
    public IOutputConsumer OutputConsumer { get; }

    /// <inheritdoc />
    public IEnumerable<IWaitUntil> WaitStrategies { get; }

    /// <inheritdoc />
    public IReadOnlyList<Action<CreateContainerParameters>> ParameterModifiers { get; }

    /// <inheritdoc />
    public Func<ITestcontainersContainer, CancellationToken, Task> StartupCallback { get; }
  }
}
