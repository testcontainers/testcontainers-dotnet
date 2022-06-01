namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Networks;
  using DotNet.Testcontainers.Volumes;

  /// <summary>
  /// This class represents the fluent Testcontainer builder. Each change creates a new instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.
  /// With this behaviour we can reuse previous configured configurations and create similar Testcontainer with only little effort.
  /// </summary>
  /// <example>
  /// <code>
  ///   var builder = new builder&lt;TestcontainersContainer&gt;()
  ///     .WithName(&quot;nginx&quot;)
  ///     .WithImage(&quot;nginx&quot;)
  ///     .WithEntrypoint(&quot;...&quot;)
  ///     .WithCommand(&quot;...&quot;);
  ///   <br />
  ///   var http = builder
  ///     .WithPortBinding(80, 08)
  ///     .Build();
  ///   <br />
  ///   var https = builder
  ///     .WithPortBinding(443, 443)
  ///     .Build();
  /// </code>
  /// </example>
  /// <typeparam name="TDockerContainer">Type of <see cref="ITestcontainersContainer" />.</typeparam>
  public class TestcontainersBuilder<TDockerContainer> : AbstractBuilder<ITestcontainersBuilder<TDockerContainer>, ITestcontainersConfiguration>, ITestcontainersBuilder<TDockerContainer>
    where TDockerContainer : ITestcontainersContainer
  {
    private readonly Action<TDockerContainer> mergeModuleConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersBuilder{TDockerContainer}" /> class.
    /// </summary>
    public TestcontainersBuilder()
      : this(
        new TestcontainersConfiguration(
          endpoint: TestcontainersSettings.OS.DockerApiEndpoint,
          dockerRegistryAuthenticationConfigurations: default(DockerRegistryAuthenticationConfiguration),
          labels: DefaultLabels.Instance,
          outputConsumer: Consume.DoNotConsumeStdoutAndStderr(),
          waitStrategies: Wait.ForUnixContainer().Build(),
          startupCallback: (testcontainers, ct) => Task.CompletedTask,
          autoRemove: false,
          privileged: false),
        _ => { })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersBuilder{TDockerContainer}" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker container configuration.</param>
    /// <param name="moduleConfiguration">The module configuration.</param>
    private TestcontainersBuilder(
      ITestcontainersConfiguration dockerResourceConfiguration,
      Action<TDockerContainer> moduleConfiguration)
      : base(dockerResourceConfiguration)
    {
      this.mergeModuleConfiguration = moduleConfiguration;
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> ConfigureContainer(Action<TDockerContainer> moduleConfiguration)
    {
      return this.MergeNewConfiguration(this.DockerResourceConfiguration, moduleConfiguration);
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithImage(string image)
    {
      return this.WithImage(new DockerImage(image));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithImage(IDockerImage image)
    {
      var modifiedImage = MaybePrependPrefixToImageName(image);
      return this.MergeNewConfiguration(new TestcontainersConfiguration(image: modifiedImage));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithName(string name)
    {
      return this.MergeNewConfiguration(new TestcontainersConfiguration(name: name));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithHostname(string hostname)
    {
      return this.MergeNewConfiguration(new TestcontainersConfiguration(hostname: hostname));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithWorkingDirectory(string workingDirectory)
    {
      return this.MergeNewConfiguration(new TestcontainersConfiguration(workingDirectory: workingDirectory));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithEntrypoint(params string[] entrypoint)
    {
      return this.MergeNewConfiguration(new TestcontainersConfiguration(entrypoint: entrypoint));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithCommand(params string[] command)
    {
      return this.MergeNewConfiguration(new TestcontainersConfiguration(command: command));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithEnvironment(string name, string value)
    {
      var environments = new Dictionary<string, string> { { name, value } };
      return this.MergeNewConfiguration(new TestcontainersConfiguration(environments: environments));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithEnvironment(IReadOnlyDictionary<string, string> environments)
    {
      return this.MergeNewConfiguration(new TestcontainersConfiguration(environments: environments));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithExposedPort(int port)
    {
      return this.WithExposedPort($"{port}");
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithExposedPort(string port)
    {
      var exposedPorts = new Dictionary<string, string> { { port, port } };
      return this.MergeNewConfiguration(new TestcontainersConfiguration(exposedPorts: exposedPorts));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithPortBinding(int port, bool assignRandomHostPort = false)
    {
      return this.WithPortBinding($"{port}", assignRandomHostPort);
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithPortBinding(int hostPort, int containerPort)
    {
      return this.WithPortBinding($"{hostPort}", $"{containerPort}");
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithPortBinding(string port, bool assignRandomHostPort = false)
    {
      var hostPort = assignRandomHostPort ? null : port;
      return this.WithPortBinding(hostPort, port);
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithPortBinding(string hostPort, string containerPort)
    {
      var portBindings = new Dictionary<string, string> { { containerPort, hostPort } };
      return this.MergeNewConfiguration(new TestcontainersConfiguration(portBindings: portBindings));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithMount(string source, string destination)
    {
      return this.WithBindMount(source, destination);
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithMount(string source, string destination, AccessMode accessMode)
    {
      return this.WithBindMount(source, destination, accessMode);
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithBindMount(string source, string destination)
    {
      return this.WithBindMount(source, destination, AccessMode.ReadWrite);
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithBindMount(string source, string destination, AccessMode accessMode)
    {
      var mounts = new IMount[] { new BindMount(source, destination, accessMode) };
      return this.MergeNewConfiguration(new TestcontainersConfiguration(mounts: mounts));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithVolumeMount(string source, string destination)
    {
      return this.WithVolumeMount(source, destination, AccessMode.ReadWrite);
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithVolumeMount(string source, string destination, AccessMode accessMode)
    {
      return this.WithVolumeMount(new DockerVolume(source), destination, accessMode);
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithVolumeMount(IDockerVolume source, string destination)
    {
      return this.WithVolumeMount(source, destination, AccessMode.ReadWrite);
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithVolumeMount(IDockerVolume source, string destination, AccessMode accessMode)
    {
      var mounts = new IMount[] { new VolumeMount(source, destination, accessMode) };
      return this.MergeNewConfiguration(new TestcontainersConfiguration(mounts: mounts));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithNetwork(string id, string name)
    {
      return this.WithNetwork(new DockerNetwork(id, name));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithNetwork(IDockerNetwork dockerNetwork)
    {
      var networks = new[] { dockerNetwork };
      return this.MergeNewConfiguration(new TestcontainersConfiguration(networks: networks));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithAutoRemove(bool autoRemove)
    {
      return this.MergeNewConfiguration(new TestcontainersConfiguration(autoRemove: autoRemove));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithPrivileged(bool privileged)
    {
      return this.MergeNewConfiguration(new TestcontainersConfiguration(privileged: privileged));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithRegistryAuthentication(string registryEndpoint, string username, string password)
    {
      return this.MergeNewConfiguration(new TestcontainersConfiguration(dockerRegistryAuthenticationConfigurations: new DockerRegistryAuthenticationConfiguration(registryEndpoint, username, password)));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithOutputConsumer(IOutputConsumer outputConsumer)
    {
      return this.MergeNewConfiguration(new TestcontainersConfiguration(outputConsumer: outputConsumer));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithWaitStrategy(IWaitForContainerOS waitStrategy)
    {
      return this.MergeNewConfiguration(new TestcontainersConfiguration(waitStrategies: waitStrategy.Build()));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public ITestcontainersBuilder<TDockerContainer> WithStartupCallback(Func<IRunningDockerContainer, CancellationToken, Task> startupCallback)
    {
      return this.MergeNewConfiguration(new TestcontainersConfiguration(startupCallback: startupCallback));
    }

    /// <inheritdoc cref="ITestcontainersBuilder{TDockerContainer}" />
    public TDockerContainer Build()
    {
      Guard.Argument(this.DockerResourceConfiguration.Image, nameof(ITestcontainersConfiguration.Image))
        .NotNull();

#pragma warning disable S3011

      // Create container instance.
      var container = (TDockerContainer)Activator.CreateInstance(typeof(TDockerContainer), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { this.DockerResourceConfiguration, TestcontainersSettings.Logger }, null);

#pragma warning restore S3011

      // Apply specific container configuration.
      this.mergeModuleConfiguration.Invoke(container);

      return container;
    }

#pragma warning disable S107

    /// <inheritdoc />
    protected override ITestcontainersBuilder<TDockerContainer> MergeNewConfiguration(IDockerResourceConfiguration dockerResourceConfiguration)
    {
      return this.MergeNewConfiguration(new TestcontainersConfiguration(dockerResourceConfiguration));
    }

    /// <summary>
    /// Merges the current with the new Docker resource configuration.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The new Docker resource configuration.</param>
    /// <param name="moduleConfiguration">The module configuration.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.</returns>
    protected virtual ITestcontainersBuilder<TDockerContainer> MergeNewConfiguration(ITestcontainersConfiguration dockerResourceConfiguration, Action<TDockerContainer> moduleConfiguration = null)
    {
      var autoRemove = BuildConfiguration.Combine(dockerResourceConfiguration.AutoRemove, this.DockerResourceConfiguration.AutoRemove);
      var privileged = BuildConfiguration.Combine(dockerResourceConfiguration.Privileged, this.DockerResourceConfiguration.Privileged);

      var endpoint = BuildConfiguration.Combine(dockerResourceConfiguration.Endpoint, this.DockerResourceConfiguration.Endpoint);
      var image = BuildConfiguration.Combine(dockerResourceConfiguration.Image, this.DockerResourceConfiguration.Image);
      var name = BuildConfiguration.Combine(dockerResourceConfiguration.Name, this.DockerResourceConfiguration.Name);
      var hostname = BuildConfiguration.Combine(dockerResourceConfiguration.Hostname, this.DockerResourceConfiguration.Hostname);
      var workingDirectory = BuildConfiguration.Combine(dockerResourceConfiguration.WorkingDirectory, this.DockerResourceConfiguration.WorkingDirectory);
      var entrypoint = BuildConfiguration.Combine(dockerResourceConfiguration.Entrypoint, this.DockerResourceConfiguration.Entrypoint);
      var command = BuildConfiguration.Combine(dockerResourceConfiguration.Command, this.DockerResourceConfiguration.Command);
      var environments = BuildConfiguration.Combine(dockerResourceConfiguration.Environments, this.DockerResourceConfiguration.Environments);
      var labels = BuildConfiguration.Combine(dockerResourceConfiguration.Labels, this.DockerResourceConfiguration.Labels);
      var exposedPorts = BuildConfiguration.Combine(dockerResourceConfiguration.ExposedPorts, this.DockerResourceConfiguration.ExposedPorts);
      var portBindings = BuildConfiguration.Combine(dockerResourceConfiguration.PortBindings, this.DockerResourceConfiguration.PortBindings);
      var mounts = BuildConfiguration.Combine(dockerResourceConfiguration.Mounts, this.DockerResourceConfiguration.Mounts);
      var networks = BuildConfiguration.Combine(dockerResourceConfiguration.Networks, this.DockerResourceConfiguration.Networks);

      var dockerRegistryAuthConfig = new[] { dockerResourceConfiguration.DockerRegistryAuthConfig, this.DockerResourceConfiguration.DockerRegistryAuthConfig }.First(config => config != null);
      var outputConsumer = new[] { dockerResourceConfiguration.OutputConsumer, this.DockerResourceConfiguration.OutputConsumer }.First(config => config != null);
      var waitStrategies = new[] { dockerResourceConfiguration.WaitStrategies, this.DockerResourceConfiguration.WaitStrategies }.First(config => config != null);
      var startupCallback = new[] { dockerResourceConfiguration.StartupCallback, this.DockerResourceConfiguration.StartupCallback }.First(config => config != null);

      var updatedDockerResourceConfiguration = new TestcontainersConfiguration(endpoint, dockerRegistryAuthConfig, image, name, hostname, workingDirectory, entrypoint, command, environments, labels, exposedPorts, portBindings, mounts, networks, outputConsumer, waitStrategies, startupCallback, autoRemove, privileged);
      return new TestcontainersBuilder<TDockerContainer>(updatedDockerResourceConfiguration, moduleConfiguration ?? this.mergeModuleConfiguration);
    }

    private static IDockerImage MaybePrependPrefixToImageName(IDockerImage originalImage)
    {
      var shouldPrependPrefix = !string.IsNullOrWhiteSpace(TestcontainersSettings.DockerHubImagePrefix);
      if (!shouldPrependPrefix)
      {
        return originalImage;
      }

      var hostedOnDockerHub = string.IsNullOrWhiteSpace(originalImage.GetHostname());
      if (!hostedOnDockerHub)
      {
        return originalImage;
      }

      return new DockerImage(originalImage.Repository.Length == 0 ? TestcontainersSettings.DockerHubImagePrefix : $"{TestcontainersSettings.DockerHubImagePrefix}/{originalImage.Repository}", originalImage.Name, originalImage.Tag);
    }
  }
}
