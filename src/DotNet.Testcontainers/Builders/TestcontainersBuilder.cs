namespace DotNet.Testcontainers.Builder
{
  using System.Collections.Generic;
  using System.Linq;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using static LanguageExt.Prelude;

  public class TestcontainersBuilder : ContainerBuilder
  {
    private readonly bool cleanUp;

    private readonly string name;

    private readonly IDockerImage image = new TestcontainersImage();

    private readonly IReadOnlyDictionary<string, string> exposedPorts = new Dictionary<string, string>();

    private readonly IReadOnlyDictionary<string, string> portBindings = new Dictionary<string, string>();

    private readonly IReadOnlyDictionary<string, string> volumes = new Dictionary<string, string>();

    public TestcontainersBuilder()
    {
    }

    protected TestcontainersBuilder(
      string name,
      IDockerImage image,
      IReadOnlyDictionary<string, string> exposedPorts,
      IReadOnlyDictionary<string, string> portBindings,
      IReadOnlyDictionary<string, string> volumes,
      bool cleanUp)
    {
      this.name = name;
      this.image = image;
      this.exposedPorts = exposedPorts;
      this.portBindings = portBindings;
      this.volumes = volumes;
      this.cleanUp = cleanUp;
    }

    internal override bool CleanUp => this.cleanUp;

    internal override string Name => this.name;

    internal override IDockerImage Image => this.image;

    internal override IReadOnlyDictionary<string, string> ExposedPorts => this.exposedPorts;

    internal override IReadOnlyDictionary<string, string> PortBindings => this.portBindings;

    internal override IReadOnlyDictionary<string, string> Volumes => this.volumes;

    public override ContainerBuilder WithCleanUp(bool cleanUp)
    {
      return Build(this, cleanUp: cleanUp);
    }

    public override ContainerBuilder WithName(string name)
    {
      return Build(this, name: name);
    }

    public override ContainerBuilder WithImage(string image)
    {
      return this.WithImage(new TestcontainersImage(image));
    }

    public override ContainerBuilder WithImage(IDockerImage image)
    {
      return Build(this, image: image);
    }

    public override ContainerBuilder WithExposedPort(int port)
    {
      return this.WithExposedPort($"{port}");
    }

    public override ContainerBuilder WithExposedPort(string port)
    {
      return Build(this, exposedPorts: HashMap((port, port)).ToDictionary());
    }

    public override ContainerBuilder WithPortBinding(int port)
    {
      return this.WithPortBinding(port, port);
    }

    public override ContainerBuilder WithPortBinding(int hostPort, int containerPort)
    {
      return this.WithPortBinding($"{hostPort}", $"{containerPort}");
    }

    public override ContainerBuilder WithPortBinding(string port)
    {
      return this.WithPortBinding(port, port);
    }

    public override ContainerBuilder WithPortBinding(string hostPort, string containerPort)
    {
      return Build(this, portBindings: HashMap((hostPort, containerPort)).ToDictionary());
    }

    public override ContainerBuilder WithVolume(string source, string destination)
    {
      return Build(this, volumes: this.volumes);
    }

    public override IDockerContainer Build()
    {
      return new TestcontainersContainer(
        this.Name,
        this.Image,
        this.ExposedPorts,
        this.PortBindings,
        this.Volumes,
        this.CleanUp);
    }

    private static ContainerBuilder Build(
      ContainerBuilder old,
      string name = null,
      IDockerImage image = null,
      IReadOnlyDictionary<string, string> exposedPorts = null,
      IReadOnlyDictionary<string, string> portBindings = null,
      IReadOnlyDictionary<string, string> volumes = null,
      bool cleanUp = true)
    {
      // Is any functional method from C# language-ext possible here?
      Merge(old.ExposedPorts, ref exposedPorts);
      Merge(old.PortBindings, ref portBindings);
      Merge(old.Volumes, ref volumes);

      return new TestcontainersBuilder(
        name ?? old.Name,
        image ?? old.Image,
        exposedPorts,
        portBindings,
        volumes,
        cleanUp);
    }

    private static void Merge<T>(IReadOnlyDictionary<T, T> previous, ref IReadOnlyDictionary<T, T> next)
    {
      if (notnull(next))
      {
        next = previous.Concat(next).ToDictionary(key => key.Key, value => value.Value);
      }
      else
      {
        next = previous;
      }
    }
  }
}
