namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Images;

  /// <inheritdoc cref="IImageFromDockerfileConfiguration" />
  internal sealed class ImageFromDockerfileConfiguration : DockerResourceConfiguration, IImageFromDockerfileConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileConfiguration" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    public ImageFromDockerfileConfiguration(IDockerResourceConfiguration dockerResourceConfiguration)
      : base(dockerResourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileConfiguration" /> class.
    /// </summary>
    /// <param name="dockerEndpointAuthenticationConfiguration">The Docker endpoint authentication configuration.</param>
    /// <param name="image">The Docker image.</param>
    /// <param name="dockerfile">The Dockerfile.</param>
    /// <param name="dockerfileDirectory">The Dockerfile directory.</param>
    /// <param name="deleteIfExists">A value indicating whether an existing image is removed or not.</param>
    /// <param name="labels">A list of labels.</param>
    public ImageFromDockerfileConfiguration(
      IDockerEndpointAuthenticationConfiguration dockerEndpointAuthenticationConfiguration = null,
      IDockerImage image = null,
      string dockerfile = null,
      string dockerfileDirectory = null,
      bool deleteIfExists = true,
      IReadOnlyDictionary<string, string> labels = null)
      : base(dockerEndpointAuthenticationConfiguration, labels)
    {
      this.Image = image;
      this.Dockerfile = dockerfile;
      this.DockerfileDirectory = dockerfileDirectory;
      this.DeleteIfExists = deleteIfExists;
    }

    /// <inheritdoc />
    public bool DeleteIfExists { get; }

    /// <inheritdoc />
    public string Dockerfile { get; }

    /// <inheritdoc />
    public string DockerfileDirectory { get; }

    /// <inheritdoc />
    public IDockerImage Image { get; }
  }
}
