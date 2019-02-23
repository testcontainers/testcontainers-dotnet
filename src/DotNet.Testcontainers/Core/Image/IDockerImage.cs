namespace DotNet.Testcontainers.Core.Image
{
  public interface IDockerImage
  {
    string Repository { get; }

    string Name { get; }

    string Tag { get; }

    string Image { get; set; }
  }
}
