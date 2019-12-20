namespace DotNet.Testcontainers.Images.Builders
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Images.Configurations;

  public sealed class ImageFromDockerfileBuilder : IImageFromDockerfileBuilder
  {
    private readonly IImageFromDockerfileConfiguration configuration;

    public ImageFromDockerfileBuilder() : this(new ImageFromDockerfileConfiguration())
    {
    }

    private ImageFromDockerfileBuilder(IImageFromDockerfileConfiguration configuration)
    {
      this.configuration = configuration;
    }

    public IImageFromDockerfileBuilder WithName(string name)
    {
      return this.WithName(new DockerImage(name));
    }

    public IImageFromDockerfileBuilder WithName(IDockerImage name)
    {
      return new ImageFromDockerfileBuilder(
        new ImageFromDockerfileConfiguration(name, this.configuration.DockerfileDirectory, this.configuration.DeleteIfExists));
    }

    public IImageFromDockerfileBuilder WithDockerfileDirectory(string dockerfileDirectory)
    {
      return new ImageFromDockerfileBuilder(
        new ImageFromDockerfileConfiguration(this.configuration.Image, dockerfileDirectory, this.configuration.DeleteIfExists));
    }

    public IImageFromDockerfileBuilder WithDeleteIfExists(bool deleteIfExists)
    {
      return new ImageFromDockerfileBuilder(
        new ImageFromDockerfileConfiguration(this.configuration.Image, this.configuration.DockerfileDirectory, deleteIfExists));
    }

    public Task<string> Build()
    {
      return new TestcontainersClient(DockerApiEndpoint.Local).BuildAsync(this.configuration);
    }
  }
}
