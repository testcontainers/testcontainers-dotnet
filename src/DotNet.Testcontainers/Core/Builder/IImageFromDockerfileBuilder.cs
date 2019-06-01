namespace DotNet.Testcontainers.Core.Builder
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Images;

  public interface IImageFromDockerfileBuilder
  {
    IImageFromDockerfileBuilder WithName(string name);

    IImageFromDockerfileBuilder WithName(IDockerImage name);

    IImageFromDockerfileBuilder WithDockerfileDirectory(string dockerfileDirectory);

    IImageFromDockerfileBuilder WithDeleteIfExists(bool deleteIfExists);

    Task<string> Build();
  }
}
