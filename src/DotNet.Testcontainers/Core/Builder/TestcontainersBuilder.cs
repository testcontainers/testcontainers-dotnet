namespace DotNet.Testcontainers.Core.Builder
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Core.Containers;
  using DotNet.Testcontainers.Core.Images;
  using DotNet.Testcontainers.Core.Models;
  using DotNet.Testcontainers.Diagnostics;
  using static DotNet.Testcontainers.Core.Models.TestcontainersConfiguration;

  public class TestcontainersBuilder : ITestcontainersBuilder
  {
    private readonly TestcontainersConfiguration config = new TestcontainersConfiguration();

    public TestcontainersBuilder()
    {
    }

    internal TestcontainersBuilder(TestcontainersConfiguration config)
    {
      this.config = config;
    }

    public ITestcontainersBuilder WithImage(string image)
    {
      return this.WithImage(new TestcontainersImage(image));
    }

    public ITestcontainersBuilder WithImage(IDockerImage image)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { Image = image.Image },
      });
    }

    public ITestcontainersBuilder WithName(string name)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { Name = name },
      });
    }

    public ITestcontainersBuilder WithWorkingDirectory(string workingDirectory)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { WorkingDirectory = workingDirectory },
      });
    }

    public ITestcontainersBuilder WithEntrypoint(params string[] entrypoint)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { Entrypoint = entrypoint },
      });
    }

    public ITestcontainersBuilder WithCommand(params string[] command)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { Command = command },
      });
    }

    public ITestcontainersBuilder WithEnvironment(string name, string value)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { Environments = new Dictionary<string, string> { { name, value } } },
      });
    }

    public ITestcontainersBuilder WithLabel(string name, string value)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { Labels = new Dictionary<string, string> { { name, value } } },
      });
    }

    public ITestcontainersBuilder WithExposedPort(int port)
    {
      return this.WithExposedPort($"{port}");
    }

    public ITestcontainersBuilder WithExposedPort(string port)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Container = new ContainerConfiguration { Labels = new Dictionary<string, string> { { port, port } } },
      });
    }

    public ITestcontainersBuilder WithPortBinding(int port)
    {
      return this.WithPortBinding(port, port);
    }

    public ITestcontainersBuilder WithPortBinding(int hostPort, int containerPort)
    {
      return this.WithPortBinding($"{hostPort}", $"{containerPort}");
    }

    public ITestcontainersBuilder WithPortBinding(string port)
    {
      return this.WithPortBinding(port, port);
    }

    public ITestcontainersBuilder WithPortBinding(string hostPort, string containerPort)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Host = new HostConfiguration { PortBindings = new Dictionary<string, string> { { hostPort, containerPort } } },
      });
    }

    public ITestcontainersBuilder WithMount(string source, string destination)
    {
      return Build(this, new TestcontainersConfiguration
      {
        Host = new HostConfiguration { Mounts = new Dictionary<string, string> { { source, destination } } },
      });
    }

    public ITestcontainersBuilder WithCleanUp(bool cleanUp)
    {
      return Build(this, new TestcontainersConfiguration
      {
        CleanUp = cleanUp,
      });
    }

    public ITestcontainersBuilder WithOutputConsumer(IOutputConsumer outputConsumer)
    {
      return Build(this, new TestcontainersConfiguration
      {
        OutputConsumer = outputConsumer,
      });
    }

    public ITestcontainersBuilder WithWaitStrategy(WaitStrategy waitStrategy)
    {
      return Build(this, new TestcontainersConfiguration
      {
        WaitStrategy = waitStrategy,
      });
    }

    public IDockerContainer Build()
    {
      return new TestcontainersContainer(this.config);
    }

    private static ITestcontainersBuilder Build(
      TestcontainersBuilder old,
      TestcontainersConfiguration config)
    {
      return new TestcontainersBuilder(config.Merge(old.config));
    }
  }
}
