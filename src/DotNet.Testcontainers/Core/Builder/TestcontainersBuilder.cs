namespace DotNet.Testcontainers.Core.Builder
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using DotNet.Testcontainers.Core.Containers;
  using DotNet.Testcontainers.Core.Images;
  using DotNet.Testcontainers.Core.Models;
  using DotNet.Testcontainers.Core.Wait;
  using DotNet.Testcontainers.Diagnostics;
  using static DotNet.Testcontainers.Core.Models.TestcontainersConfiguration;

  public sealed class TestcontainersBuilder<T> : ITestcontainersBuilder<T>
    where T : TestcontainersContainer
  {
    private readonly TestcontainersConfiguration config = new TestcontainersConfiguration();

    private readonly Action<T> configureContainer;

    public TestcontainersBuilder()
    {
    }

    private TestcontainersBuilder(
      TestcontainersConfiguration config,
      Action<T> configureContainer)
    {
      this.config = config;
      this.configureContainer = configureContainer;
    }

    public ITestcontainersBuilder<T> ConfigureContainer(Action<T> configureContainer)
    {
      return Build(this, this.config, configureContainer);
    }

    public ITestcontainersBuilder<T> WithImage(string image)
    {
      return this.WithImage(new TestcontainersImage(image));
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
      var hostPort = assignRandomHostPort ? TestcontainersNetworkService.GetAvailablePort() : port;
      return this.WithPortBinding(hostPort, port);
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
      this.configureContainer?.Invoke(container);

      return container;
    }

    private static ITestcontainersBuilder<T> Build(
      TestcontainersBuilder<T> old,
      TestcontainersConfiguration config,
      Action<T> configureContainer = null)
    {
      configureContainer = configureContainer ?? old.configureContainer;
      return new TestcontainersBuilder<T>(config.Merge(old.config), configureContainer);
    }
  }
}
