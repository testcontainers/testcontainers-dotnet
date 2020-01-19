namespace DotNet.Testcontainers.Containers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.OutputConsumers;
  using DotNet.Testcontainers.Containers.WaitStrategies;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Services;

  /// <summary>
  /// This class represents the fluent Testcontainer builder. Each change creates a new instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.
  /// With this behaviour we can reuse previous configured configurations and create similar Testcontainer with only little effort.
  /// </summary>
  /// <example>
  /// var builder = new builder&lt;TestcontainersContainer&gt;()
  ///   .WithName(&quot;nginx&quot;)
  ///   .WithImage(&quot;nginx&quot;)
  ///   .WithEntrypoint(&quot;...&quot;)
  ///   .WithCommand(&quot;...&quot;);
  ///
  /// var http = builder
  ///   .WithPortBinding(80, 08)
  ///   .Build();
  ///
  /// var https = builder
  ///   .WithPortBinding(443, 443)
  ///   .Build();
  /// </example>
  /// <typeparam name="TDockerContainer">Type of <see cref="IDockerContainer" />.</typeparam>
  public sealed class TestcontainersBuilder<TDockerContainer> : ITestcontainersBuilder<TDockerContainer>
    where TDockerContainer : IDockerContainer
  {
    private readonly ITestcontainersConfiguration configuration;

    private readonly Action<TDockerContainer> moduleConfiguration;

    public TestcontainersBuilder() : this(
      Apply(),
      testcontainer => { })
    {
    }

    private TestcontainersBuilder(
      ITestcontainersConfiguration configuration,
      Action<TDockerContainer> moduleConfiguration)
    {
      this.configuration = configuration;
      this.moduleConfiguration = moduleConfiguration;
    }

    public ITestcontainersBuilder<TDockerContainer> ConfigureContainer(Action<TDockerContainer> moduleConfiguration)
    {
      return Build(this, this.configuration, moduleConfiguration);
    }

    public ITestcontainersBuilder<TDockerContainer> WithImage(string image)
    {
      return this.WithImage(new DockerImage(image));
    }

    public ITestcontainersBuilder<TDockerContainer> WithImage(IDockerImage image)
    {
      return Build(this, Apply(image: image));
    }

    public ITestcontainersBuilder<TDockerContainer> WithName(string name)
    {
      return Build(this, Apply(name: name));
    }

    public ITestcontainersBuilder<TDockerContainer> WithWorkingDirectory(string workingDirectory)
    {
      return Build(this, Apply(workingDirectory: workingDirectory));
    }

    public ITestcontainersBuilder<TDockerContainer> WithEntrypoint(params string[] entrypoint)
    {
      return Build(this, Apply(entrypoint: entrypoint));
    }

    public ITestcontainersBuilder<TDockerContainer> WithCommand(params string[] command)
    {
      return Build(this, Apply(command: command));
    }

    public ITestcontainersBuilder<TDockerContainer> WithEnvironment(string name, string value)
    {
      var environments = new Dictionary<string, string> { { name, value } };
      return Build(this, Apply(environments: environments));
    }

    public ITestcontainersBuilder<TDockerContainer> WithLabel(string name, string value)
    {
      var labels = new Dictionary<string, string> { { name, value } };
      return Build(this, Apply(labels: labels));
    }

    public ITestcontainersBuilder<TDockerContainer> WithExposedPort(int port)
    {
      return this.WithExposedPort($"{port}");
    }

    public ITestcontainersBuilder<TDockerContainer> WithExposedPort(string port)
    {
      var exposedPorts = new Dictionary<string, string> { { port, port } };
      return Build(this, Apply(exposedPorts: exposedPorts));
    }

    public ITestcontainersBuilder<TDockerContainer> WithPortBinding(int port, bool assignRandomHostPort = false)
    {
      return this.WithPortBinding($"{port}", assignRandomHostPort);
    }

    public ITestcontainersBuilder<TDockerContainer> WithPortBinding(int hostPort, int containerPort)
    {
      return this.WithPortBinding($"{hostPort}", $"{containerPort}");
    }

    public ITestcontainersBuilder<TDockerContainer> WithPortBinding(string port, bool assignRandomHostPort = false)
    {
      var hostPort = assignRandomHostPort ? $"{TestcontainersNetworkService.GetAvailablePort()}" : port;
      return this.WithPortBinding(hostPort, port);
    }

    public ITestcontainersBuilder<TDockerContainer> WithPortBinding(string hostPort, string containerPort)
    {
      var portBindings = new Dictionary<string, string> { { hostPort, containerPort } };
      return Build(this, Apply(portBindings: portBindings));
    }

    public ITestcontainersBuilder<TDockerContainer> WithMount(string source, string destination)
    {
      var mounts = new IBind[] { new Mount(source, destination, AccessMode.ReadWrite) };
      return Build(this, Apply(mounts: mounts));
    }

    public ITestcontainersBuilder<TDockerContainer> WithCleanUp(bool cleanUp)
    {
      return Build(this, Apply(cleanUp: cleanUp));
    }

    public ITestcontainersBuilder<TDockerContainer> WithDockerEndpoint(string endpoint)
    {
      return Build(this, Apply(endpoint: new Uri(endpoint)));
    }

    public ITestcontainersBuilder<TDockerContainer> WithOutputConsumer(IOutputConsumer outputConsumer)
    {
      return Build(this, Apply(outputConsumer: outputConsumer));
    }

    public ITestcontainersBuilder<TDockerContainer> WithWaitStrategy(IWaitUntil waitStrategy)
    {
      return Build(this, Apply(waitStrategy: waitStrategy));
    }

    public TDockerContainer Build()
    {
      // Create container instance.
      var container = (TDockerContainer)Activator.CreateInstance(typeof(TDockerContainer), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { this.configuration }, null);

      // Apply specific container configuration.
      this.moduleConfiguration.Invoke(container);

      return container;
    }

#pragma warning disable S107

    private static ITestcontainersConfiguration Apply(
      Uri endpoint = null,
      IDockerImage image = null,
      string name = null,
      string workingDirectory = null,
      IEnumerable<string> entrypoint = null,
      IEnumerable<string> command = null,
      IReadOnlyDictionary<string, string> environments = null,
      IReadOnlyDictionary<string, string> labels = null,
      IReadOnlyDictionary<string, string> exposedPorts = null,
      IReadOnlyDictionary<string, string> portBindings = null,
      IEnumerable<IBind> mounts = null,
      IOutputConsumer outputConsumer = null,
      IWaitUntil waitStrategy = null,
      bool cleanUp = true)
    {
      return new TestcontainersConfiguration(
        endpoint ?? DockerApiEndpoint.Local,
        image,
        name,
        workingDirectory,
        entrypoint,
        command,
        environments,
        labels,
        exposedPorts,
        portBindings,
        mounts,
        outputConsumer ?? DoNotConsumeStdoutOrStderr.OutputConsumer,
        waitStrategy ?? WaitUntilContainerIsRunning.WaitStrategy,
        cleanUp);
    }

#pragma warning restore S107

    private static ITestcontainersBuilder<TDockerContainer> Build(
      TestcontainersBuilder<TDockerContainer> previous,
      ITestcontainersConfiguration next,
      Action<TDockerContainer> moduleConfiguration = null)
    {
      var cleanUp = next.CleanUp && previous.configuration.CleanUp;
      var endpoint = Merge(next.Endpoint, previous.configuration.Endpoint, DockerApiEndpoint.Local);
      var image = Merge(next.Image, previous.configuration.Image);
      var name = Merge(next.Name, previous.configuration.Name);
      var workingDirectory = Merge(next.WorkingDirectory, previous.configuration.WorkingDirectory);
      var entrypoint = Merge(next.Entrypoint, previous.configuration.Entrypoint);
      var command = Merge(next.Command, previous.configuration.Command);
      var environments = Merge(next.Environments, previous.configuration.Environments);
      var labels = Merge(next.Labels, previous.configuration.Labels);
      var exposedPorts = Merge(next.ExposedPorts, previous.configuration.ExposedPorts);
      var portBindings = Merge(next.PortBindings, previous.configuration.PortBindings);
      var mounts = Merge(next.Mounts, previous.configuration.Mounts);
      var outputConsumer = Merge(next.OutputConsumer, previous.configuration.OutputConsumer, DoNotConsumeStdoutOrStderr.OutputConsumer);
      var waitStrategy = Merge(next.WaitStrategy, previous.configuration.WaitStrategy, WaitUntilContainerIsRunning.WaitStrategy);

      var mergedConfigurations = Apply(
        endpoint,
        image,
        name,
        workingDirectory,
        entrypoint,
        command,
        environments,
        labels,
        exposedPorts,
        portBindings,
        mounts,
        outputConsumer,
        waitStrategy,
        cleanUp);

      return new TestcontainersBuilder<TDockerContainer>(mergedConfigurations, moduleConfiguration ?? previous.moduleConfiguration);
    }

    /// <summary>
    /// Returns the changed Testcontainer configuration object. If there is no change, the previous Testcontainer configuration object is returned.
    /// </summary>
    /// <param name="next">Changed Testcontainer configuration object.</param>
    /// <param name="previous">Previous Testcontainer configuration object.</param>
    /// <param name="defaultConfiguration">Default Testcontainer configuration.</param>
    /// <typeparam name="T">Any class.</typeparam>
    /// <returns>Changed Testcontainer configuration object. If there is no change, the previous Testcontainer configuration object.</returns>
    private static T Merge<T>(T next, T previous, T defaultConfiguration = null)
      where T : class
    {
      return next == null || next.Equals(defaultConfiguration) ? previous : next;
    }

    /// <summary>
    /// Merges all existing and new Testcontainer configuration changes. If there are no changes, the previous Testcontainer configurations are returned.
    /// </summary>
    /// <param name="next">Changed Testcontainer configuration.</param>
    /// <param name="previous">Previous Testcontainer configuration.</param>
    /// <typeparam name="T">Type of <see cref="IReadOnlyDictionary{TKey,TValue}" />.</typeparam>
    /// <returns>An updated Testcontainer configuration.</returns>
    private static IEnumerable<T> Merge<T>(IEnumerable<T> next, IEnumerable<T> previous)
      where T : class
    {
      if (next == null || previous == null)
      {
        return next ?? previous;
      }
      else
      {
        return next.Concat(previous).ToArray();
      }
    }

    /// <summary>
    /// Merges all existing and new Testcontainer configuration changes. If there are no changes, the previous Testcontainer configurations are returned.
    /// </summary>
    /// <param name="next">Changed Testcontainer configuration.</param>
    /// <param name="previous">Previous Testcontainer configuration.</param>
    /// <typeparam name="T">Type of <see cref="IReadOnlyDictionary{TKey,TValue}" />.</typeparam>
    /// <returns>An updated Testcontainer configuration.</returns>
    private static IReadOnlyDictionary<T, T> Merge<T>(IReadOnlyDictionary<T, T> next, IReadOnlyDictionary<T, T> previous)
      where T : class
    {
      if (next == null || previous == null)
      {
        return next ?? previous;
      }
      else
      {
        return next.Concat(previous.Where(item => !next.Keys.Contains(item.Key))).ToDictionary(item => item.Key, item => item.Value);
      }
    }
  }
}
