namespace DotNet.Testcontainers.Builder
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;

  public class ContainerBuilder
  {
    private readonly GenericImage image = new GenericImage();

    private readonly IDictionary<string, string> exposedPort = new Dictionary<string, string>();

    private readonly IDictionary<string, string> portBindings = new Dictionary<string, string>();

    public ContainerBuilder WithImage(string image)
    {
      this.image.Image = image;
      return this;
    }

    public ContainerBuilder WithImage(GenericImage image)
    {
      this.image.Image = image.Image;
      return this;
    }

    public ContainerBuilder WithExposedPort(string port)
    {
      this.exposedPort.Add(port, port);
      return this;
    }

    public ContainerBuilder WithPortBindings(int port)
    {
      return this.WithPortBindings(port, port);
    }

    public ContainerBuilder WithPortBindings(int hostPort, int containerPort)
    {
      return this.WithPortBindings(hostPort.ToString(), containerPort.ToString());
    }

    public ContainerBuilder WithPortBindings(string port)
    {
      return this.WithPortBindings(port, port);
    }

    public ContainerBuilder WithPortBindings(string hostPort, string containerPort)
    {
      this.portBindings.Add(hostPort, containerPort);
      return this;
    }

    public GenericContainer Build()
    {
      return new GenericContainer(this.image, this.exposedPort, this.portBindings);
    }
  }
}
