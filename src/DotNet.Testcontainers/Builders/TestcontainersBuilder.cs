namespace DotNet.Testcontainers.Builder
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;

  public class TestcontainersBuilder : IContainerBuilder
  {
    private readonly TestcontainersImage image = new TestcontainersImage();

    private readonly IDictionary<string, string> exposedPort = new Dictionary<string, string>();

    private readonly IDictionary<string, string> portBindings = new Dictionary<string, string>();

    public IContainerBuilder WithImage(string image)
    {
      return this.WithImage(new TestcontainersImage(image));
    }

    public IContainerBuilder WithImage(IDockerImage image)
    {
      this.image.Image = image.Image;
      return this;
    }

    public IContainerBuilder WithExposedPort(int port)
    {
      return this.WithExposedPort(port.ToString());
    }

    public IContainerBuilder WithExposedPort(string port)
    {
      this.exposedPort.Add(port, port);
      return this;
    }

    public IContainerBuilder WithPortBindings(int port)
    {
      return this.WithPortBindings(port, port);
    }

    public IContainerBuilder WithPortBindings(int hostPort, int containerPort)
    {
      return this.WithPortBindings(hostPort.ToString(), containerPort.ToString());
    }

    public IContainerBuilder WithPortBindings(string port)
    {
      return this.WithPortBindings(port, port);
    }

    public IContainerBuilder WithPortBindings(string hostPort, string containerPort)
    {
      this.portBindings.Add(hostPort, containerPort);
      return this;
    }

    public IDockerContainer Build()
    {
      return new TestcontainersContainer(this.image, this.exposedPort, this.portBindings);
    }
  }
}
