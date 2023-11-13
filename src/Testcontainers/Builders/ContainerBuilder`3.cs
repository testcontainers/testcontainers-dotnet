namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.IO;
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
    where TBuilderEntity : ContainerBuilder<TBuilderEntity, TContainerEntity, TConfigurationEntity>
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

    /// <inheritdoc />
    public TBuilderEntity DependsOn(IContainer container)
    {
      var containers = new[] { container };
      return Clone(new ContainerConfiguration(containers: containers));
    }

    /// <inheritdoc />
    public TBuilderEntity DependsOn(INetwork network)
    {
      return WithNetwork(network);
    }

    /// <inheritdoc />
    public TBuilderEntity DependsOn(IVolume volume, string destination)
    {
      return WithVolumeMount(volume, destination);
    }

    /// <inheritdoc />
    public TBuilderEntity DependsOn(IVolume volume, string destination, AccessMode accessMode)
    {
      return WithVolumeMount(volume, destination, accessMode);
    }

    /// <inheritdoc />
    public TBuilderEntity WithImage(string image)
    {
      return WithImage(new DockerImage(image));
    }

    /// <inheritdoc />
    public TBuilderEntity WithImage(IImage image)
    {
      if (string.IsNullOrEmpty(TestcontainersSettings.HubImageNamePrefix))
      {
        return Clone(new ContainerConfiguration(image: image));
      }

      if (!string.IsNullOrEmpty(image.GetHostname()))
      {
        return Clone(new ContainerConfiguration(image: image));
      }

      return Clone(new ContainerConfiguration(image: new DockerImage(image.Repository, image.Name, image.Tag, TestcontainersSettings.HubImageNamePrefix)));
    }

    /// <inheritdoc />
    public TBuilderEntity WithImagePullPolicy(Func<ImageInspectResponse, bool> imagePullPolicy)
    {
      return Clone(new ContainerConfiguration(imagePullPolicy: imagePullPolicy));
    }

    /// <inheritdoc />
    public TBuilderEntity WithName(string name)
    {
      return Clone(new ContainerConfiguration(name: name));
    }

    /// <inheritdoc />
    public TBuilderEntity WithHostname(string hostname)
    {
      return Clone(new ContainerConfiguration(hostname: hostname));
    }

    /// <inheritdoc />
    public TBuilderEntity WithMacAddress(string macAddress)
    {
      return Clone(new ContainerConfiguration(macAddress: macAddress));
    }

    /// <inheritdoc />
    public TBuilderEntity WithWorkingDirectory(string workingDirectory)
    {
      return Clone(new ContainerConfiguration(workingDirectory: workingDirectory));
    }

    /// <inheritdoc />
    public TBuilderEntity WithEntrypoint(params string[] entrypoint)
    {
      return Clone(new ContainerConfiguration(entrypoint: entrypoint));
    }

    /// <inheritdoc />
    public TBuilderEntity WithCommand(params string[] command)
    {
      return Clone(new ContainerConfiguration(command: command));
    }

    /// <inheritdoc />
    public TBuilderEntity WithEnvironment(string name, string value)
    {
      var environments = new Dictionary<string, string> { { name, value } };
      return Clone(new ContainerConfiguration(environments: environments));
    }

    /// <inheritdoc />
    public TBuilderEntity WithEnvironment(IReadOnlyDictionary<string, string> environments)
    {
      return Clone(new ContainerConfiguration(environments: environments));
    }

    /// <inheritdoc />
    public TBuilderEntity WithExposedPort(int port)
    {
      return WithExposedPort(port.ToString(CultureInfo.InvariantCulture));
    }

    /// <inheritdoc />
    public TBuilderEntity WithExposedPort(string port)
    {
      var exposedPorts = new Dictionary<string, string> { { port, port } };
      return Clone(new ContainerConfiguration(exposedPorts: exposedPorts));
    }

    /// <inheritdoc />
    public TBuilderEntity WithPortBinding(int port, bool assignRandomHostPort = false)
    {
      return WithPortBinding(port.ToString(CultureInfo.InvariantCulture), assignRandomHostPort);
    }

    /// <inheritdoc />
    public TBuilderEntity WithPortBinding(int hostPort, int containerPort)
    {
      return WithPortBinding(hostPort.ToString(CultureInfo.InvariantCulture), containerPort.ToString(CultureInfo.InvariantCulture));
    }

    /// <inheritdoc />
    public TBuilderEntity WithPortBinding(string port, bool assignRandomHostPort = false)
    {
      // Use an empty string instead of "0": https://github.com/docker/for-mac/issues/5588#issuecomment-934600089.
      var hostPort = assignRandomHostPort ? string.Empty : port;
      return WithPortBinding(hostPort, port);
    }

    /// <inheritdoc />
    public TBuilderEntity WithPortBinding(string hostPort, string containerPort)
    {
      // Remove this together with TestcontainersSettings.ResourceReaperPublicHostPort.
      hostPort = "0".Equals(hostPort, StringComparison.OrdinalIgnoreCase) ? string.Empty : hostPort;

      var portBindings = new Dictionary<string, string> { { containerPort, hostPort } };
      return Clone(new ContainerConfiguration(portBindings: portBindings)).WithExposedPort(containerPort);
    }

    /// <inheritdoc />
    public TBuilderEntity WithResourceMapping(IResourceMapping resourceMapping)
    {
      var resourceMappings = new[] { resourceMapping };
      return Clone(new ContainerConfiguration(resourceMappings: resourceMappings));
    }

    /// <inheritdoc />
    public TBuilderEntity WithResourceMapping(byte[] resourceContent, string filePath, UnixFileModes fileMode = Unix.FileMode644)
    {
      return WithResourceMapping(new BinaryResourceMapping(resourceContent, filePath, fileMode));
    }

    /// <inheritdoc />
    public TBuilderEntity WithResourceMapping(string source, string target, UnixFileModes fileMode = Unix.FileMode644)
    {
      var fileAttributes = File.GetAttributes(source);

      if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
      {
        return WithResourceMapping(new DirectoryInfo(source), target, fileMode);
      }
      else
      {
        return WithResourceMapping(new FileInfo(source), target, fileMode);
      }
    }

    /// <inheritdoc />
    public TBuilderEntity WithResourceMapping(DirectoryInfo source, string target, UnixFileModes fileMode = Unix.FileMode644)
    {
      return WithResourceMapping(new FileResourceMapping(source.FullName, target, fileMode));
    }

    /// <inheritdoc />
    public TBuilderEntity WithResourceMapping(FileInfo source, string target, UnixFileModes fileMode = Unix.FileMode644)
    {
      return WithResourceMapping(new FileResourceMapping(source.FullName, target, fileMode));
    }

    /// <inheritdoc />
    public TBuilderEntity WithResourceMapping(FileInfo source, FileInfo target, UnixFileModes fileMode = Unix.FileMode644)
    {
      using (var fileStream = source.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      {
        using (var streamReader = new BinaryReader(fileStream))
        {
          var resourceContent = streamReader.ReadBytes((int)streamReader.BaseStream.Length);
          return WithResourceMapping(resourceContent, target.ToString(), fileMode);
        }
      }
    }

    /// <inheritdoc />
    public TBuilderEntity WithMount(IMount mount)
    {
      var mounts = new[] { mount };
      return Clone(new ContainerConfiguration(mounts: mounts));
    }

    /// <inheritdoc />
    public TBuilderEntity WithBindMount(string source, string destination)
    {
      return WithBindMount(source, destination, AccessMode.ReadWrite);
    }

    /// <inheritdoc />
    public TBuilderEntity WithBindMount(string source, string destination, AccessMode accessMode)
    {
      return WithMount(new BindMount(source, destination, accessMode));
    }

    /// <inheritdoc />
    public TBuilderEntity WithVolumeMount(string source, string destination)
    {
      return WithVolumeMount(source, destination, AccessMode.ReadWrite);
    }

    /// <inheritdoc />
    public TBuilderEntity WithVolumeMount(string source, string destination, AccessMode accessMode)
    {
      var volume = new FromExistingVolume().WithName(source).Build();
      return WithVolumeMount(volume, destination, accessMode);
    }

    /// <inheritdoc />
    public TBuilderEntity WithVolumeMount(IVolume volume, string destination)
    {
      return WithVolumeMount(volume, destination, AccessMode.ReadWrite);
    }

    /// <inheritdoc />
    public TBuilderEntity WithVolumeMount(IVolume volume, string destination, AccessMode accessMode)
    {
      return WithMount(new VolumeMount(volume, destination, accessMode));
    }

    /// <inheritdoc />
    public TBuilderEntity WithTmpfsMount(string destination)
    {
      return WithTmpfsMount(destination, AccessMode.ReadWrite);
    }

    /// <inheritdoc />
    public TBuilderEntity WithTmpfsMount(string destination, AccessMode accessMode)
    {
      return WithMount(new TmpfsMount(destination, accessMode));
    }

    /// <inheritdoc />
    public TBuilderEntity WithNetwork(string name)
    {
      var network = new FromExistingNetwork().WithName(name).Build();
      return WithNetwork(network);
    }

    /// <inheritdoc />
    public TBuilderEntity WithNetwork(INetwork network)
    {
      var networks = new[] { network };
      return Clone(new ContainerConfiguration(networks: networks));
    }

    /// <inheritdoc />
    public TBuilderEntity WithNetworkAliases(params string[] networkAliases)
    {
      return WithNetworkAliases(networkAliases.AsEnumerable());
    }

    /// <inheritdoc />
    public TBuilderEntity WithNetworkAliases(IEnumerable<string> networkAliases)
    {
      return Clone(new ContainerConfiguration(networkAliases: networkAliases));
    }

    /// <inheritdoc />
    public TBuilderEntity WithExtraHost(string hostname, string ipAddress)
    {
      var extraHosts = new[] { string.Join(":", hostname, ipAddress) };
      return Clone(new ContainerConfiguration(extraHosts: extraHosts));
    }

    /// <inheritdoc />
    public TBuilderEntity WithAutoRemove(bool autoRemove)
    {
      return Clone(new ContainerConfiguration(autoRemove: autoRemove));
    }

    /// <inheritdoc />
    public TBuilderEntity WithPrivileged(bool privileged)
    {
      return Clone(new ContainerConfiguration(privileged: privileged));
    }

    /// <inheritdoc />
    public TBuilderEntity WithOutputConsumer(IOutputConsumer outputConsumer)
    {
      return Clone(new ContainerConfiguration(outputConsumer: outputConsumer));
    }

    /// <inheritdoc />
    public TBuilderEntity WithWaitStrategy(IWaitForContainerOS waitStrategy)
    {
      return Clone(new ContainerConfiguration(waitStrategies: waitStrategy.Build()));
    }

    /// <inheritdoc />
    public TBuilderEntity WithStartupCallback(Func<IContainer, CancellationToken, Task> startupCallback)
    {
      return Clone(new ContainerConfiguration(startupCallback: startupCallback));
    }

    /// <inheritdoc />
    protected override TBuilderEntity Init()
    {
      return base.Init().WithImagePullPolicy(PullPolicy.Missing).WithPortForwarding().WithOutputConsumer(Consume.DoNotConsumeStdoutAndStderr()).WithWaitStrategy(Wait.ForUnixContainer()).WithStartupCallback((_, ct) => Task.CompletedTask);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
      base.Validate();

      _ = Guard.Argument(DockerResourceConfiguration.Image, nameof(IContainerConfiguration.Image))
        .NotNull();
    }

    /// <summary>
    /// Clones the Docker resource builder configuration.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    protected abstract TBuilderEntity Clone(IContainerConfiguration resourceConfiguration);

    private TBuilderEntity WithPortForwarding()
    {
      const string hostname = "host.testcontainers.internal";
      return PortForwardingContainer.Instance != null && TestcontainersStates.Running.Equals(PortForwardingContainer.Instance.State) ? WithExtraHost(hostname, PortForwardingContainer.Instance.IpAddress) : Clone(new ContainerConfiguration());
    }

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
        return new DockerNetwork(DockerResourceConfiguration.Name);
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
          Name = name;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Task CreateAsync(CancellationToken ct = default)
        {
          return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DeleteAsync(CancellationToken ct = default)
        {
          return Task.CompletedTask;
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
          return default;
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
        return new DockerVolume(DockerResourceConfiguration.Name);
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
          Name = name;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Task CreateAsync(CancellationToken ct = default)
        {
          return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DeleteAsync(CancellationToken ct = default)
        {
          return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
          return default;
        }
      }
    }
  }
}
