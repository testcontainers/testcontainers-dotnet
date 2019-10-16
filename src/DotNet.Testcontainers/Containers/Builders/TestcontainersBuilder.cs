namespace DotNet.Testcontainers.Containers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.Modules;
  using DotNet.Testcontainers.Containers.OutputConsumers;
  using DotNet.Testcontainers.Containers.WaitStrategies;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Services;
  using static Configurations.TestcontainersConfiguration;

  /// <summary>
  /// This class represents the fluent Testcontainer builder. Each change creates a new instance of <see cref="ITestcontainersBuilder{T}" />.
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
  /// <typeparam name="T">Type of <see cref="TestcontainersContainer" />.</typeparam>
  public sealed class TestcontainersBuilder<T> : ITestcontainersBuilder<T>
    where T : TestcontainersContainer
  {
    private readonly TestcontainersConfiguration config = new TestcontainersConfiguration();

    private readonly Action<T> overrideConfiguration;

    public TestcontainersBuilder()
    {
    }

    private TestcontainersBuilder(
      TestcontainersConfiguration config,
      Action<T> overrideConfiguration)
    {
      this.config = config;
      this.overrideConfiguration = overrideConfiguration;
    }

    public ITestcontainersBuilder<T> ConfigureContainer(Action<T> moduleConfiguration)
    {
      return Build(this, this.config, moduleConfiguration);
    }

    public ITestcontainersBuilder<T> WithImage(string image)
    {
      return this.WithImage(new DockerImage(image));
    }

    public ITestcontainersBuilder<T> WithImage(IDockerImage image)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { Image = image.Image },
      });
    }

    public ITestcontainersBuilder<T> WithName(string name)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { Name = name },
      });
    }

    public ITestcontainersBuilder<T> WithWorkingDirectory(string workingDirectory)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { WorkingDirectory = workingDirectory },
      });
    }

    public ITestcontainersBuilder<T> WithEntrypoint(params string[] entrypoint)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { Entrypoint = entrypoint },
      });
    }

    public ITestcontainersBuilder<T> WithCommand(params string[] command)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { Command = command },
      });
    }

    public ITestcontainersBuilder<T> WithEnvironment(string name, string value)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { Environments = new Dictionary<string, string> { { name, value } } },
      });
    }

    public ITestcontainersBuilder<T> WithLabel(string name, string value)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { Labels = new Dictionary<string, string> { { name, value } } },
      });
    }

    public ITestcontainersBuilder<T> WithExposedPort(int port)
    {
      return this.WithExposedPort($"{port}");
    }

    public ITestcontainersBuilder<T> WithExposedPort(string port)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { ExposedPorts = new Dictionary<string, string> { { port, port } } },
      });
    }

    public ITestcontainersBuilder<T> WithPortBinding(int port, bool assignRandomHostPort = false)
    {
      return this.WithPortBinding($"{port}", assignRandomHostPort);
    }

    public ITestcontainersBuilder<T> WithPortBinding(int hostPort, int containerPort)
    {
      return this.WithPortBinding($"{hostPort}", $"{containerPort}");
    }

    public ITestcontainersBuilder<T> WithPortBinding(string port, bool assignRandomHostPort = false)
    {
      var hostPort = assignRandomHostPort ? $"{TestcontainersNetworkService.GetAvailablePort()}" : port;
      return this.WithPortBinding(hostPort, port);
    }

    public ITestcontainersBuilder<T> WithPortBinding(string hostPort, string containerPort)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Host = new HostConfiguration { PortBindings = new Dictionary<string, string> { { hostPort, containerPort } } },
      });
    }

    public ITestcontainersBuilder<T> WithMount(string source, string destination)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Host = new HostConfiguration { Mounts = new Dictionary<string, string> { { source, destination } } },
      });
    }

    public ITestcontainersBuilder<T> WithCleanUp(bool cleanUp)
    {
      return Build(this, new TestcontainersConfiguration
      {
        CleanUp = cleanUp,
      });
    }

    public ITestcontainersBuilder<T> WithOutputConsumer(IOutputConsumer outputConsumer)
    {
      return Build(this, new TestcontainersConfiguration
      {
        OutputConsumer = outputConsumer,
      });
    }

    public ITestcontainersBuilder<T> WithWaitStrategy(IWaitUntil waitStrategy)
    {
      return Build(this, new TestcontainersConfiguration
      {
        WaitStrategy = waitStrategy,
      });
    }

    public T Build()
    {
      // Create container instance.
      var container = (T)Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { this.config }, null);

      // Apply specific container configuration.
      this.overrideConfiguration?.Invoke(container);

      return container;
    }

    private static ITestcontainersBuilder<T> Build(
      TestcontainersBuilder<T> old,
      TestcontainersConfiguration config,
      Action<T> configureContainer = null)
    {
      configureContainer = configureContainer ?? old.overrideConfiguration;
      return new TestcontainersBuilder<T>(config.Merge(old.config), configureContainer);
    }
  }
}
