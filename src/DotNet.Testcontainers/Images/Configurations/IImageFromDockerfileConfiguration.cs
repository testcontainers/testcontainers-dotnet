namespace DotNet.Testcontainers.Images.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// A Dockerfile configuration.
  /// </summary>
  public interface IImageFromDockerfileConfiguration
  {
    /// <summary>
    /// If true, an existing image is removed. Otherwise, it is kept.
    /// </summary>
    bool DeleteIfExists { get; }

    /// <summary>
    /// Gets the Dockerfile.
    /// </summary>
    [NotNull]
    string Dockerfile { get; }

    /// <summary>
    /// Gets the Dockerfile directory.
    /// </summary>
    [NotNull]
    string DockerfileDirectory { get; }

    /// <summary>
    /// Gets the Docker image.
    /// </summary>
    [NotNull]
    IDockerImage Image { get; }
  }
}
