namespace DotNet.Testcontainers.Core.Builder
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Images;

  public interface IImageFromDockerfileBuilder
  {
    IImageFromDockerfileBuilder WithName(string name);

    IImageFromDockerfileBuilder WithName(IDockerImage image);

    IImageFromDockerfileBuilder WithDockerfileDirectory(string dockerfileDirectory);

    IImageFromDockerfileBuilder WithDeleteIfExits(bool deleteIfExits);

    Task<string> Build();
  }
}
