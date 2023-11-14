namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Networks;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IContainerConfiguration" />
  [PublicAPI]
  public class ContainerConfiguration : ResourceConfiguration<CreateContainerParameters>, IContainerConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="imagePullPolicy">The image pull policy.</param>
    /// <param name="name">The name.</param>
    /// <param name="hostname">The hostname.</param>
    /// <param name="macAddress">The MAC address.</param>
    /// <param name="workingDirectory">The working directory.</param>
    /// <param name="entrypoint">The entrypoint.</param>
    /// <param name="command">The command.</param>
    /// <param name="environments">A dictionary of environment variables.</param>
    /// <param name="exposedPorts">A dictionary of exposed ports.</param>
    /// <param name="portBindings">A dictionary of port bindings.</param>
    /// <param name="resourceMappings">A list of resource mappings.</param>
    /// <param name="containers">A list of containers.</param>
    /// <param name="mounts">A list of mounts.</param>
    /// <param name="networks">A list of networks.</param>
    /// <param name="networkAliases">A list of network-scoped aliases.</param>
    /// <param name="extraHosts">A list of extra hosts.</param>
    /// <param name="outputConsumer">The output consumer.</param>
    /// <param name="waitStrategies">The wait strategies.</param>
    /// <param name="startupCallback">The startup callback.</param>
    /// <param name="autoRemove">A value indicating whether Docker removes the container after it exits or not.</param>
    /// <param name="privileged">A value indicating whether the privileged flag is set or not.</param>
    public ContainerConfiguration(
      IImage image = null,
      Func<ImageInspectResponse, bool> imagePullPolicy = null,
      string name = null,
      string hostname = null,
      string macAddress = null,
      string workingDirectory = null,
      IEnumerable<string> entrypoint = null,
      IEnumerable<string> command = null,
      IReadOnlyDictionary<string, string> environments = null,
      IReadOnlyDictionary<string, string> exposedPorts = null,
      IReadOnlyDictionary<string, string> portBindings = null,
      IEnumerable<IResourceMapping> resourceMappings = null,
      IEnumerable<IContainer> containers = null,
      IEnumerable<IMount> mounts = null,
      IEnumerable<INetwork> networks = null,
      IEnumerable<string> networkAliases = null,
      IEnumerable<string> extraHosts = null,
      IOutputConsumer outputConsumer = null,
      IEnumerable<IWaitUntil> waitStrategies = null,
      Func<IContainer, CancellationToken, Task> startupCallback = null,
      bool? autoRemove = null,
      bool? privileged = null)
    {
      AutoRemove = autoRemove;
      Privileged = privileged;
      Image = image;
      ImagePullPolicy = imagePullPolicy;
      Name = name;
      Hostname = hostname;
      MacAddress = macAddress;
      WorkingDirectory = workingDirectory;
      Entrypoint = entrypoint;
      Command = command;
      Environments = environments;
      ExposedPorts = exposedPorts;
      PortBindings = portBindings;
      ResourceMappings = resourceMappings;
      Containers = containers;
      Mounts = mounts;
      Networks = networks;
      NetworkAliases = networkAliases;
      ExtraHosts = extraHosts;
      OutputConsumer = outputConsumer;
      WaitStrategies = waitStrategies;
      StartupCallback = startupCallback;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ContainerConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
      : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ContainerConfiguration(IContainerConfiguration resourceConfiguration)
      : this(new ContainerConfiguration(), resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public ContainerConfiguration(IContainerConfiguration oldValue, IContainerConfiguration newValue)
      : base(oldValue, newValue)
    {
      Image = BuildConfiguration.Combine(oldValue.Image, newValue.Image);
      ImagePullPolicy = BuildConfiguration.Combine(oldValue.ImagePullPolicy, newValue.ImagePullPolicy);
      Name = BuildConfiguration.Combine(oldValue.Name, newValue.Name);
      Hostname = BuildConfiguration.Combine(oldValue.Hostname, newValue.Hostname);
      MacAddress = BuildConfiguration.Combine(oldValue.MacAddress, newValue.MacAddress);
      WorkingDirectory = BuildConfiguration.Combine(oldValue.WorkingDirectory, newValue.WorkingDirectory);
      Entrypoint = BuildConfiguration.Combine<IEnumerable<string>>(oldValue.Entrypoint, newValue.Entrypoint);
      Command = BuildConfiguration.Combine(oldValue.Command, newValue.Command);
      Environments = BuildConfiguration.Combine(oldValue.Environments, newValue.Environments);
      ExposedPorts = BuildConfiguration.Combine(oldValue.ExposedPorts, newValue.ExposedPorts);
      PortBindings = BuildConfiguration.Combine(oldValue.PortBindings, newValue.PortBindings);
      ResourceMappings = BuildConfiguration.Combine(oldValue.ResourceMappings, newValue.ResourceMappings);
      Containers = BuildConfiguration.Combine(oldValue.Containers, newValue.Containers);
      Mounts = BuildConfiguration.Combine(oldValue.Mounts, newValue.Mounts);
      Networks = BuildConfiguration.Combine(oldValue.Networks, newValue.Networks);
      NetworkAliases = BuildConfiguration.Combine(oldValue.NetworkAliases, newValue.NetworkAliases);
      ExtraHosts = BuildConfiguration.Combine(oldValue.ExtraHosts, newValue.ExtraHosts);
      OutputConsumer = BuildConfiguration.Combine(oldValue.OutputConsumer, newValue.OutputConsumer);
      WaitStrategies = BuildConfiguration.Combine<IEnumerable<IWaitUntil>>(oldValue.WaitStrategies, newValue.WaitStrategies);
      StartupCallback = BuildConfiguration.Combine(oldValue.StartupCallback, newValue.StartupCallback);
      AutoRemove = (oldValue.AutoRemove.HasValue && oldValue.AutoRemove.Value) || (newValue.AutoRemove.HasValue && newValue.AutoRemove.Value);
      Privileged = (oldValue.Privileged.HasValue && oldValue.Privileged.Value) || (newValue.Privileged.HasValue && newValue.Privileged.Value);
    }

    /// <inheritdoc />
    public bool? AutoRemove { get; }

    /// <inheritdoc />
    public bool? Privileged { get; }

    /// <inheritdoc />
    public IImage Image { get; }

    /// <inheritdoc />
    public Func<ImageInspectResponse, bool> ImagePullPolicy { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Hostname { get; }

    /// <inheritdoc />
    public string MacAddress { get; }

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
    public IEnumerable<IResourceMapping> ResourceMappings { get; }

    /// <inheritdoc />
    public IEnumerable<IContainer> Containers { get; }

    /// <inheritdoc />
    public IEnumerable<IMount> Mounts { get; }

    /// <inheritdoc />
    public IEnumerable<INetwork> Networks { get; }

    /// <inheritdoc />
    public IEnumerable<string> NetworkAliases { get; }

    /// <inheritdoc />
    public IEnumerable<string> ExtraHosts { get; }

    /// <inheritdoc />
    public IOutputConsumer OutputConsumer { get; }

    /// <inheritdoc />
    public IEnumerable<IWaitUntil> WaitStrategies { get; }

    /// <inheritdoc />
    public Func<IContainer, CancellationToken, Task> StartupCallback { get; }
  }
}
