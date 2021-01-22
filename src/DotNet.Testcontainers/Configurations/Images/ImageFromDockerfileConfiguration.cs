namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Images;

  /// <inheritdoc cref="IImageFromDockerfileConfiguration" />
  internal sealed class ImageFromDockerfileConfiguration : IImageFromDockerfileConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    /// <param name="dockerfile">The Dockerfile.</param>
    /// <param name="dockerfileDirectory">The Dockerfile directory.</param>
    /// <param name="deleteIfExists">A value indicating whether an existing image is removed or not.</param>
    /// <param name="labels">A list of labels.</param>
    public ImageFromDockerfileConfiguration(
      IDockerImage image,
      string dockerfile,
      string dockerfileDirectory,
      bool deleteIfExists,
      IReadOnlyDictionary<string, string> labels)
    {
      this.Image = image;
      this.Dockerfile = dockerfile;
      this.DockerfileDirectory = dockerfileDirectory;
      this.DeleteIfExists = deleteIfExists;
      this.Labels = labels;
    }

    /// <inheritdoc />
    public bool DeleteIfExists { get; }

    /// <inheritdoc />
    public string Dockerfile { get; }

    /// <inheritdoc />
    public string DockerfileDirectory { get; }

    /// <inheritdoc />
    public IDockerImage Image { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Labels { get; }
  }
}
