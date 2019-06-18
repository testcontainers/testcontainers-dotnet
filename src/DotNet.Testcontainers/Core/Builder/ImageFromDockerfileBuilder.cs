namespace DotNet.Testcontainers.Core.Builder
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Core.Images;
  using DotNet.Testcontainers.Core.Models;

  public sealed class ImageFromDockerfileBuilder : IImageFromDockerfileBuilder
  {
    private readonly ImageFromDockerfileConfiguration configuration = new ImageFromDockerfileConfiguration();

    public IImageFromDockerfileBuilder WithName(string name)
    {
      return this.WithName(new TestcontainersImage(name));
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
