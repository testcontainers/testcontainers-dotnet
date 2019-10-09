namespace DotNet.Testcontainers.Images.Builders
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Client;
  using DotNet.Testcontainers.Images.Configurations;

  public sealed class ImageFromDockerfileBuilder : IImageFromDockerfileBuilder
  {
    private readonly ImageFromDockerfileConfiguration configuration = new ImageFromDockerfileConfiguration();

    public IImageFromDockerfileBuilder WithName(string name)
    {
      return this.WithName(new DockerImage(name));
    }

    public IImageFromDockerfileBuilder WithName(IDockerImage name)
    {
      this.configuration.Image = name.Image;
      return this;
    }

    public IImageFromDockerfileBuilder WithDockerfileDirectory(string dockerfileDirectory)
    {
      this.configuration.DockerfileDirectory = dockerfileDirectory;
      return this;
    }

    public IImageFromDockerfileBuilder WithDeleteIfExists(bool deleteIfExists)
    {
      this.configuration.DeleteIfExists = deleteIfExists;
      return this;
    }

    public Task<string> Build()
    {
      return TestcontainersClient.Instance.BuildAsync(this.configuration);
    }
  }
}
