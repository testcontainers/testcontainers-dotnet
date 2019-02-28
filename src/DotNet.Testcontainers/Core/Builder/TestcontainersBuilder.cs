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

    private readonly string workingDirectory;

    private readonly IReadOnlyCollection<string> entrypoint = new List<string>();

    private readonly IReadOnlyCollection<string> command = new List<string>();

    private readonly IReadOnlyDictionary<string, string> environments = new Dictionary<string, string>();

    private readonly IReadOnlyDictionary<string, string> labels = new Dictionary<string, string>();

    private readonly IReadOnlyDictionary<string, string> exposedPorts = new Dictionary<string, string>();

    private readonly IReadOnlyDictionary<string, string> portBindings = new Dictionary<string, string>();

    private readonly IReadOnlyDictionary<string, string> mounts = new Dictionary<string, string>();

    private readonly bool cleanUp = true;

    public TestcontainersBuilder()
    {
    }

#pragma warning disable S107

    protected TestcontainersBuilder(
      IDockerImage image,
      string name,
      string workingDirectory,
      IReadOnlyCollection<string> entrypoint,
      IReadOnlyCollection<string> commands,
      IReadOnlyDictionary<string, string> environments,
      IReadOnlyDictionary<string, string> labels,
      IReadOnlyDictionary<string, string> exposedPorts,
      IReadOnlyDictionary<string, string> portBindings,
      IReadOnlyDictionary<string, string> mounts,
      bool cleanUp)
    {
      this.image = image;
      this.name = name;
      this.workingDirectory = workingDirectory;
      this.entrypoint = entrypoint;
      this.command = commands;
      this.environments = environments;
      this.labels = labels;
      this.exposedPorts = exposedPorts;
      this.portBindings = portBindings;
      this.mounts = mounts;
      this.cleanUp = cleanUp;
    }

#pragma warning restore S107

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

    public ITestcontainersBuilder WithWorkingDirectory(string workingDirectory)
    {
      return Build(this, workingDirectory: workingDirectory);
    }

    public ITestcontainersBuilder WithEntrypoint(params string[] entrypoint)
    {
      return Build(this, entrypoint: entrypoint);
    }

    public ITestcontainersBuilder WithCommand(params string[] command)
    {
      return Build(this, command: new ReadOnlyCollection<string>(command));
    }

    public ITestcontainersBuilder WithEnvironment(string name, string value)
    {
      return Build(this, environments: HashMap((name, value)).ToDictionary());
    }

    public ITestcontainersBuilder WithLabel(string name, string value)
    {
      return Build(this, labels: HashMap((name, value)).ToDictionary());
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

    public ITestcontainersBuilder WithCleanUp(bool cleanUp)
    {
      return Build(this, cleanUp: cleanUp);
    }

    public IDockerContainer Build()
    {
      var configuration = default(TestcontainersConfiguration);
      configuration.Container.Image = this.image.Image;
      configuration.Container.Name = this.name;
      configuration.Container.WorkingDirectory = this.workingDirectory;
      configuration.Container.Entrypoint = this.entrypoint;
      configuration.Container.Command = this.command;
      configuration.Container.Environments = this.environments;
      configuration.Container.ExposedPorts = this.exposedPorts;
      configuration.Container.Labels = this.labels;
      configuration.Host.PortBindings = this.portBindings;
      configuration.Host.Mounts = this.mounts;

      return new TestcontainersContainer(configuration, this.cleanUp);
    }

#pragma warning disable S107

    private static ITestcontainersBuilder Build(
      TestcontainersBuilder old,
      IDockerImage image = null,
      string name = null,
      string workingDirectory = null,
      IReadOnlyCollection<string> entrypoint = null,
      IReadOnlyCollection<string> command = null,
      IReadOnlyDictionary<string, string> environments = null,
      IReadOnlyDictionary<string, string> exposedPorts = null,
      IReadOnlyDictionary<string, string> labels = null,
      IReadOnlyDictionary<string, string> portBindings = null,
      IReadOnlyDictionary<string, string> mounts = null,
      bool cleanUp = true)
    {
      Merge(old.environments, ref environments);
      Merge(old.exposedPorts, ref exposedPorts);
      Merge(old.labels, ref labels);
      Merge(old.portBindings, ref portBindings);
      Merge(old.entrypoint, ref entrypoint);
      Merge(old.command, ref command);
      Merge(old.mounts, ref mounts);

      return new TestcontainersBuilder(
        image ?? old.image,
        name ?? old.name,
        workingDirectory ?? old.workingDirectory,
        entrypoint,
        command,
        environments,
        labels,
        exposedPorts,
        portBindings,
        mounts,
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
