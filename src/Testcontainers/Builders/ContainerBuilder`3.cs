namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Networks;
  using DotNet.Testcontainers.Volumes;
  using JetBrains.Annotations;

  /// <summary>
  /// An abstract fluent Docker container builder.
  /// </summary>
  /// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
  /// <typeparam name="TContainerEntity">The resource entity.</typeparam>
  /// <typeparam name="TConfigurationEntity">The configuration entity.</typeparam>
  [PublicAPI]
  public abstract class ContainerBuilder<TBuilderEntity, TContainerEntity, TConfigurationEntity> : AbstractBuilder<TBuilderEntity, TContainerEntity, CreateContainerParameters, TConfigurationEntity>, IContainerBuilder<TBuilderEntity, TContainerEntity>
    where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity>
    where TContainerEntity : IContainer
    where TConfigurationEntity : IContainerConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    protected ContainerBuilder(TConfigurationEntity dockerResourceConfiguration)
      : base(dockerResourceConfiguration)
    {
    }

    [Obsolete("Required for extension method module backwards compatibility.")]
    protected virtual Action<TContainerEntity> ModuleConfiguration { get; }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public virtual TBuilderEntity ConfigureContainer(Action<TContainerEntity> moduleConfiguration)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithImage(string image)
    {
      return this.WithImage(new DockerImage(image));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithImage(IImage image)
    {
      if (string.IsNullOrEmpty(TestcontainersSettings.HubImageNamePrefix))
      {
        return this.Clone(new ContainerConfiguration(image: image));
      }

      if (!string.IsNullOrEmpty(image.GetHostname()))
      {
        return this.Clone(new ContainerConfiguration(image: image));
      }

      return this.Clone(new ContainerConfiguration(image: new DockerImage(image.Repository, image.Name, image.Tag, TestcontainersSettings.HubImageNamePrefix)));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithImagePullPolicy(Func<ImagesListResponse, bool> imagePullPolicy)
    {
      return this.Clone(new ContainerConfiguration(imagePullPolicy: imagePullPolicy));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithName(string name)
    {
      return this.Clone(new ContainerConfiguration(name: name));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithHostname(string hostname)
    {
      return this.Clone(new ContainerConfiguration(hostname: hostname));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithMacAddress(string macAddress)
    {
      return this.Clone(new ContainerConfiguration(macAddress: macAddress));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithWorkingDirectory(string workingDirectory)
    {
      return this.Clone(new ContainerConfiguration(workingDirectory: workingDirectory));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithEntrypoint(params string[] entrypoint)
    {
      return this.Clone(new ContainerConfiguration(entrypoint: entrypoint));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithCommand(params string[] command)
    {
      return this.Clone(new ContainerConfiguration(command: command));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithEnvironment(string name, string value)
    {
      var environments = new Dictionary<string, string> { { name, value } };
      return this.Clone(new ContainerConfiguration(environments: environments));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithEnvironment(IReadOnlyDictionary<string, string> environments)
    {
      return this.Clone(new ContainerConfiguration(environments: environments));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithExposedPort(int port)
    {
      return this.WithExposedPort(port.ToString(CultureInfo.InvariantCulture));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithExposedPort(string port)
    {
      var exposedPorts = new Dictionary<string, string> { { port, port } };
      return this.Clone(new ContainerConfiguration(exposedPorts: exposedPorts));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithPortBinding(int port, bool assignRandomHostPort = false)
    {
      return this.WithPortBinding(port.ToString(CultureInfo.InvariantCulture), assignRandomHostPort);
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithPortBinding(int hostPort, int containerPort)
    {
      return this.WithPortBinding(hostPort.ToString(CultureInfo.InvariantCulture), containerPort.ToString(CultureInfo.InvariantCulture));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithPortBinding(string port, bool assignRandomHostPort = false)
    {
      var hostPort = assignRandomHostPort ? "0" : port;
      return this.WithPortBinding(hostPort, port);
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithPortBinding(string hostPort, string containerPort)
    {
      // Prepare Java alignment. Use an empty string instead of 0: https://github.com/docker/for-mac/issues/5588#issuecomment-934600089.
      hostPort = "0".Equals(hostPort, StringComparison.OrdinalIgnoreCase) ? string.Empty : hostPort;

      var portBindings = new Dictionary<string, string> { { containerPort, hostPort } };
      return this.Clone(new ContainerConfiguration(portBindings: portBindings)).WithExposedPort(containerPort);
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithResourceMapping(string source, string destination)
    {
      return this.WithResourceMapping(new FileResourceMapping(source, destination));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithResourceMapping(byte[] resourceContent, string destination)
    {
      return this.WithResourceMapping(new BinaryResourceMapping(resourceContent, destination));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithResourceMapping(IResourceMapping resourceMapping)
    {
      var resourceMappings = new Dictionary<string, IResourceMapping> { { resourceMapping.Target, resourceMapping } };
      return this.Clone(new ContainerConfiguration(resourceMappings: resourceMappings));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithMount(IMount mount)
    {
      var mounts = new[] { mount };
      return this.Clone(new ContainerConfiguration(mounts: mounts));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithBindMount(string source, string destination)
    {
      return this.WithBindMount(source, destination, AccessMode.ReadWrite);
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithBindMount(string source, string destination, AccessMode accessMode)
    {
      return this.WithMount(new BindMount(source, destination, accessMode));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithVolumeMount(string source, string destination)
    {
      return this.WithVolumeMount(source, destination, AccessMode.ReadWrite);
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithVolumeMount(string source, string destination, AccessMode accessMode)
    {
      var volume = new FromExistingVolume().WithName(source).Build();
      return this.WithVolumeMount(volume, destination, accessMode);
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithVolumeMount(IVolume volume, string destination)
    {
      return this.WithVolumeMount(volume, destination, AccessMode.ReadWrite);
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithVolumeMount(IVolume volume, string destination, AccessMode accessMode)
    {
      return this.WithMount(new VolumeMount(volume, destination, accessMode));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithTmpfsMount(string destination)
    {
      return this.WithTmpfsMount(destination, AccessMode.ReadWrite);
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithTmpfsMount(string destination, AccessMode accessMode)
    {
      return this.WithMount(new TmpfsMount(destination, accessMode));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithNetwork(string id, string name)
    {
      return this.WithNetwork(name);
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithNetwork(string name)
    {
      var network = new FromExistingNetwork().WithName(name).Build();
      return this.WithNetwork(network);
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithNetwork(INetwork network)
    {
      var networks = new[] { network };
      return this.Clone(new ContainerConfiguration(networks: networks));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithNetworkAliases(params string[] networkAliases)
    {
      return this.WithNetworkAliases(networkAliases.AsEnumerable());
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithNetworkAliases(IEnumerable<string> networkAliases)
    {
      return this.Clone(new ContainerConfiguration(networkAliases: networkAliases));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithAutoRemove(bool autoRemove)
    {
      return this.Clone(new ContainerConfiguration(autoRemove: autoRemove));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithPrivileged(bool privileged)
    {
      return this.Clone(new ContainerConfiguration(privileged: privileged));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithOutputConsumer(IOutputConsumer outputConsumer)
    {
      return this.Clone(new ContainerConfiguration(outputConsumer: outputConsumer));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithWaitStrategy(IWaitForContainerOS waitStrategy)
    {
      return this.Clone(new ContainerConfiguration(waitStrategies: waitStrategy.Build()));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithStartupCallback(Func<IContainer, CancellationToken, Task> startupCallback)
    {
      return this.Clone(new ContainerConfiguration(startupCallback: startupCallback));
    }

    /// <inheritdoc cref="IContainerBuilder{TBuilderEntity, TContainerEntity}" />
    public TBuilderEntity WithCreateContainerParametersModifier(Action<CreateContainerParameters> parameterModifier)
    {
      return this.WithCreateParameterModifier(parameterModifier);
    }

    public TBuilderEntity WithImage(IDockerImage image)
    {
      return this.WithImage(new DockerImage(image));
    }

    public TBuilderEntity WithVolumeMount(IDockerVolume volume, string destination)
    {
      return this.WithVolumeMount(new DockerVolume(volume), destination);
    }

    public TBuilderEntity WithVolumeMount(IDockerVolume volume, string destination, AccessMode accessMode)
    {
      return this.WithVolumeMount(new DockerVolume(volume), destination, accessMode);
    }

    /// <inheritdoc cref="IAbstractBuilder{TBuilderEntity, TResourceEntity, TCreateResourceEntity}" />
    protected override TBuilderEntity Init()
    {
      return base.Init().WithImagePullPolicy(PullPolicy.Missing).WithOutputConsumer(Consume.DoNotConsumeStdoutAndStderr()).WithWaitStrategy(Wait.ForUnixContainer()).WithStartupCallback((_, ct) => Task.CompletedTask);
    }

    /// <inheritdoc cref="IAbstractBuilder{TBuilderEntity, TResourceEntity, TCreateResourceEntity}" />
    protected override void Validate()
    {
      base.Validate();

      _ = Guard.Argument(this.DockerResourceConfiguration.Image, nameof(IContainerConfiguration.Image))
        .NotNull();
    }

    /// <summary>
    /// Clones the Docker resource builder configuration.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    protected abstract TBuilderEntity Clone(IContainerConfiguration resourceConfiguration);

    /// <inheritdoc cref="NetworkBuilder" />
    private sealed class FromExistingNetwork : NetworkBuilder
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="FromExistingNetwork" /> class.
      /// </summary>
      public FromExistingNetwork()
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="FromExistingNetwork" /> class.
      /// </summary>
      /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
      public FromExistingNetwork(INetworkConfiguration dockerResourceConfiguration)
        : base(dockerResourceConfiguration)
      {
      }

      /// <inheritdoc />
      public override INetwork Build()
      {
        return new DockerNetwork(this.DockerResourceConfiguration.Name);
      }

      /// <inheritdoc />
      protected override NetworkBuilder Merge(INetworkConfiguration oldValue, INetworkConfiguration newValue)
      {
        return new FromExistingNetwork(new NetworkConfiguration(oldValue, newValue));
      }

      /// <inheritdoc cref="INetwork" />
      private sealed class DockerNetwork : INetwork
      {
        /// <summary>
        /// Initializes a new instance of the <see cref="DockerNetwork" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DockerNetwork(string name)
        {
          this.Name = name;
        }

        /// <inheritdoc cref="INetwork" />
        public string Name { get; }

        /// <inheritdoc cref="INetwork" />
        public Task CreateAsync(CancellationToken ct = default)
        {
          return Task.CompletedTask;
        }

        /// <inheritdoc cref="INetwork" />
        public Task DeleteAsync(CancellationToken ct = default)
        {
          return Task.CompletedTask;
        }
      }
    }

    /// <inheritdoc cref="VolumeBuilder" />
    private sealed class FromExistingVolume : VolumeBuilder
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="FromExistingVolume" /> class.
      /// </summary>
      public FromExistingVolume()
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="FromExistingVolume" /> class.
      /// </summary>
      /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
      public FromExistingVolume(IVolumeConfiguration dockerResourceConfiguration)
        : base(dockerResourceConfiguration)
      {
      }

      /// <inheritdoc />
      public override IVolume Build()
      {
        return new DockerVolume(this.DockerResourceConfiguration.Name);
      }

      /// <inheritdoc />
      protected override VolumeBuilder Merge(IVolumeConfiguration oldValue, IVolumeConfiguration newValue)
      {
        return new FromExistingVolume(new VolumeConfiguration(oldValue, newValue));
      }

      /// <inheritdoc cref="IVolume" />
      private sealed class DockerVolume : IVolume
      {
        /// <summary>
        /// Initializes a new instance of the <see cref="DockerVolume" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DockerVolume(string name)
        {
          this.Name = name;
        }

        /// <inheritdoc cref="IVolume" />
        public string Name { get; }

        /// <inheritdoc cref="IVolume" />
        public Task CreateAsync(CancellationToken ct = default)
        {
          return Task.CompletedTask;
        }

        /// <inheritdoc cref="IVolume" />
        public Task DeleteAsync(CancellationToken ct = default)
        {
          return Task.CompletedTask;
        }
      }
    }
  }
}
