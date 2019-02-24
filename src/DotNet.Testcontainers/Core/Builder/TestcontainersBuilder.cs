namespace DotNet.Testcontainers.Core.Builder
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using DotNet.Testcontainers.Core.Containers;
  using DotNet.Testcontainers.Core.Images;
  using DotNet.Testcontainers.Core.Models;
  using static LanguageExt.Prelude;

  public class TestcontainersBuilder : ITestcontainersBuilder
  {
    private readonly IDockerImage image = new TestcontainersImage();

    private readonly string name;

    private readonly IReadOnlyDictionary<string, string> exposedPorts = new Dictionary<string, string>();

    private readonly IReadOnlyDictionary<string, string> portBindings = new Dictionary<string, string>();

    private readonly IReadOnlyDictionary<string, string> mounts = new Dictionary<string, string>();

    private readonly IReadOnlyCollection<string> command = new List<string>();

    private readonly bool cleanUp = true;

    public TestcontainersBuilder()
    {
    }

    protected TestcontainersBuilder(
      IDockerImage image,
      string name,
      IReadOnlyDictionary<string, string> exposedPorts,
      IReadOnlyDictionary<string, string> portBindings,
      IReadOnlyDictionary<string, string> mounts,
      IReadOnlyCollection<string> commands,
      bool cleanUp)
    {
      this.name = name;
      this.image = image;
      this.exposedPorts = exposedPorts;
      this.portBindings = portBindings;
      this.mounts = mounts;
      this.command = commands;
      this.cleanUp = cleanUp;
    }

    public ITestcontainersBuilder WithImage(string image)
    {
      return this.WithImage(new TestcontainersImage(image));
    }

    public ITestcontainersBuilder WithImage(IDockerImage image)
    {
      return Build(this, image: image);
    }

    public ITestcontainersBuilder WithName(string name)
    {
      return Build(this, name: name);
    }

    public ITestcontainersBuilder WithExposedPort(int port)
    {
      return this.WithExposedPort($"{port}");
    }

    public ITestcontainersBuilder WithExposedPort(string port)
    {
      return Build(this, exposedPorts: HashMap((port, port)).ToDictionary());
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
      return Build(this, portBindings: HashMap((hostPort, containerPort)).ToDictionary());
    }

    public ITestcontainersBuilder WithMount(string source, string destination)
    {
      return Build(this, mounts: HashMap((source, destination)).ToDictionary());
    }

    public ITestcontainersBuilder WithCommand(params string[] command)
    {
      return Build(this, commands: new ReadOnlyCollection<string>(command));
    }

    public ITestcontainersBuilder WithCleanUp(bool cleanUp)
    {
      return Build(this, cleanUp: cleanUp);
    }

    public IDockerContainer Build()
    {
      var configuration = default(TestcontainersConfiguration);
      configuration.Container.Image = this.image.Image;
      configuration.Container.Name = this.name;
      configuration.Container.ExposedPorts = this.exposedPorts;
      configuration.Container.Command = this.command;
      configuration.Host.PortBindings = this.portBindings;
      configuration.Host.Mounts = this.mounts;

      return new TestcontainersContainer(configuration, this.cleanUp);
    }

#pragma warning disable S107 // Any changes to reduce the amount of parameters?

    private static ITestcontainersBuilder Build(
      TestcontainersBuilder old,
      IDockerImage image = null,
      string name = null,
      IReadOnlyDictionary<string, string> exposedPorts = null,
      IReadOnlyDictionary<string, string> portBindings = null,
      IReadOnlyDictionary<string, string> mounts = null,
      IReadOnlyCollection<string> commands = null,
      bool cleanUp = true)
    {
      Merge(old.exposedPorts, ref exposedPorts);
      Merge(old.portBindings, ref portBindings);
      Merge(old.mounts, ref mounts);
      Merge(old.command, ref commands);

      return new TestcontainersBuilder(
        image ?? old.image,
        name ?? old.name,
        exposedPorts,
        portBindings,
        mounts,
        commands,
        cleanUp);
    }

#pragma warning restore S107

    private static void Merge<T>(IReadOnlyDictionary<T, T> previous, ref IReadOnlyDictionary<T, T> next)
    {
      next = Optional(next).Match(
        Some: some => previous.Concat(some).ToDictionary(key => key.Key, value => value.Value),
        None: () => previous);
    }

    private static void Merge<T>(IReadOnlyCollection<T> previous, ref IReadOnlyCollection<T> next)
    {
      next = Optional(next).Match(
        Some: some => previous.Concat(some).ToList(),
        None: () => previous);
    }
  }
}
