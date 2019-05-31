namespace DotNet.Testcontainers.Core.Builder
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Core.Images;
  using DotNet.Testcontainers.Core.Models;

  public class ImageFromDockerfileBuilder : IImageFromDockerfileBuilder
  {
    private readonly ImageFromDockerfileConfiguration configuration = new ImageFromDockerfileConfiguration();

    public IImageFromDockerfileBuilder WithName(string image)
    {
      return this.WithName(new TestcontainersImage(image));
    }

    public IImageFromDockerfileBuilder WithName(IDockerImage image)
    {
      this.configuration.Image = image.Image;
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
