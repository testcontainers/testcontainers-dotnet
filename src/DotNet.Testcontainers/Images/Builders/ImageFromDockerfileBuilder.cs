namespace DotNet.Testcontainers.Images.Builders
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Images.Configurations;
  using DotNet.Testcontainers.Internals;

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
        new ImageFromDockerfileConfiguration(name, this.configuration.Dockerfile, this.configuration.DockerfileDirectory, this.configuration.DeleteIfExists));
    }

    public IImageFromDockerfileBuilder WithDockerfile(string dockerfile)
    {
      return new ImageFromDockerfileBuilder(
        new ImageFromDockerfileConfiguration(this.configuration.Image, dockerfile, this.configuration.DockerfileDirectory, this.configuration.DeleteIfExists));
    }

    public IImageFromDockerfileBuilder WithDockerfileDirectory(string dockerfileDirectory)
    {
      return new ImageFromDockerfileBuilder(
        new ImageFromDockerfileConfiguration(this.configuration.Image, this.configuration.Dockerfile, dockerfileDirectory, this.configuration.DeleteIfExists));
    }

    public IImageFromDockerfileBuilder WithDeleteIfExists(bool deleteIfExists)
    {
      return new ImageFromDockerfileBuilder(
        new ImageFromDockerfileConfiguration(this.configuration.Image, this.configuration.Dockerfile, this.configuration.DockerfileDirectory, deleteIfExists));
    }

    public async Task<string> Build()
    {
      await new SynchronizationContextRemover();
      return await new TestcontainersClient().BuildAsync(this.configuration);
    }
  }
}
