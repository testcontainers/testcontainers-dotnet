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

    public TestcontainersBuilder()
    {
    }

    protected TestcontainersBuilder(
      string name = null,
      IDockerImage image = null,
      IReadOnlyDictionary<string, string> exposedPorts = null,
      IReadOnlyDictionary<string, string> portBindings = null,
      bool cleanUp = true)
    {
      this.name = name;
      this.image = image;
      this.exposedPorts = exposedPorts;
      this.portBindings = portBindings;
      this.cleanUp = cleanUp;
    }

    internal override bool CleanUp => this.cleanUp;

    internal override string Name => this.name;

    internal override IDockerImage Image => this.image;

    internal override IReadOnlyDictionary<string, string> ExposedPorts => this.exposedPorts;

    internal override IReadOnlyDictionary<string, string> PortBindings => this.portBindings;

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

    public override IDockerContainer Build()
    {
      return new TestcontainersContainer(
        this.Name,
        this.Image,
        this.ExposedPorts,
        this.PortBindings,
        this.CleanUp);
    }

    private static ContainerBuilder Build(
      ContainerBuilder old,
      string name = null,
      IDockerImage image = null,
      IReadOnlyDictionary<string, string> exposedPorts = null,
      IReadOnlyDictionary<string, string> portBindings = null,
      bool cleanUp = true)
    {
      // Is any functional method from C# language-ext possible here?
      if (exposedPorts == null)
      {
        exposedPorts = old.ExposedPorts;
      }
      else
      {
        exposedPorts = exposedPorts.Concat(old.ExposedPorts).ToDictionary(key => key.Key, value => value.Value);
      }

      if (portBindings == null)
      {
        portBindings = old.PortBindings;
      }
      else
      {
        portBindings = portBindings.Concat(old.PortBindings).ToDictionary(key => key.Key, value => value.Value);
      }

      return new TestcontainersBuilder(
        name ?? old.Name,
        image ?? old.Image,
        exposedPorts,
        portBindings,
        cleanUp);
    }
  }
}
