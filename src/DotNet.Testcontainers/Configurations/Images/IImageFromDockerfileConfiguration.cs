namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Images;

  /// <summary>
  /// A Dockerfile configuration.
  /// </summary>
  public interface IImageFromDockerfileConfiguration : IDockerResourceConfiguration
  {
    /// <summary>
    /// Gets a value indicating whether an existing image is removed or not.
    /// </summary>
    bool DeleteIfExists { get; }

    /// <summary>
    /// Gets the Dockerfile.
    /// </summary>
    string Dockerfile { get; }

    /// <summary>
    /// Gets the Dockerfile directory.
    /// </summary>
    string DockerfileDirectory { get; }

    /// <summary>
    /// Gets the Docker image.
    /// </summary>
    IDockerImage Image { get; }
  }
}
