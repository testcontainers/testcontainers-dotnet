namespace DotNet.Testcontainers.Images
{
  using JetBrains.Annotations;

  /// <summary>
  /// This class represents a Docker image.
  /// </summary>
  public interface IDockerImage
  {
    /// <summary>
    /// Gets the Docker image repository name.
    /// </summary>
    [NotNull]
    string Repository { get; }

    /// <summary>
    /// Gets the Docker image name.
    /// </summary>
    [NotNull]
    string Name { get; }

    /// <summary>
    /// Gets the Docker image tag.
    /// </summary>
    [NotNull]
    string Tag { get; }

    /// <summary>
    /// Gets the full Docker image name.
    /// </summary>
    /// <remarks>
    /// Full Docker image name, like "foo/bar:1.0.0" or "bar:latest" based on the components values.
    /// </remarks>
    [NotNull]
    string FullName { get; }

    /// <summary>
    /// Gets the Docker registry hostname.
    /// </summary>
    /// <returns>The Docker registry hostname.</returns>
    [CanBeNull]
    string GetHostname();
  }
}
