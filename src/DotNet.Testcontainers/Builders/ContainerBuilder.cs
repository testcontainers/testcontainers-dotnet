namespace DotNet.Testcontainers.Builder
{
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;

  public class ContainerBuilder
  {
    private GenericImage dockerImage;

    public ContainerBuilder WithImage(GenericImage dockerImage)
    {
      this.dockerImage = dockerImage;
      return this;
    }

    public ContainerBuilder WithImage(string dockerImageName)
    {
      this.dockerImage = new GenericImage(dockerImageName);
      return this;
    }

    public ContainerBuilder WithExposedPort()
    {
      return this;
    }

    public GenericContainer Build()
    {
      return new GenericContainer(this.dockerImage);
    }
  }
}
