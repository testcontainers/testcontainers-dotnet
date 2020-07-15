namespace DotNet.Testcontainers.Images.Configurations
{
  using System.IO;

  /// <inheritdoc cref="IImageFromDockerfileConfiguration" />
  internal sealed class ImageFromDockerfileConfiguration : IImageFromDockerfileConfiguration
  {
    public ImageFromDockerfileConfiguration() : this(CreateDockerImage())
    {
    }

    public ImageFromDockerfileConfiguration(
      IDockerImage image,
      string dockerfile = "Dockerfile",
      string dockerfileDirectory = ".",
      bool deleteIfExists = true)
    {
      this.DeleteIfExists = deleteIfExists;
      this.Dockerfile = dockerfile;
      this.DockerfileDirectory = dockerfileDirectory;
      this.Image = image;
    }

    /// <inheritdoc />
    public bool DeleteIfExists { get; }

    /// <inheritdoc />
    public string Dockerfile { get; }

    /// <inheritdoc />
    public string DockerfileDirectory { get; }

    /// <inheritdoc />
    public IDockerImage Image { get; }

    private static IDockerImage CreateDockerImage()
    {
      return new DockerImage("Testcontainers", Path.GetRandomFileName().Substring(0, 8), string.Empty);
    }
  }
}
