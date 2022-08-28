namespace DotNet.Testcontainers.Images
{
  using JetBrains.Annotations;

  /// <summary>
  /// This class represents a Docker image.
  /// </summary>
  [PublicAPI]
  public interface IDockerImage
  {
    /// <summary>
    /// Gets the Docker image repository name.
    /// </summary>
    [PublicAPI]
    [NotNull]
    string Repository { get; }

    /// <summary>
    /// Gets the Docker image name.
    /// </summary>
    [PublicAPI]
    [NotNull]
    string Name { get; }

    /// <summary>
    /// Gets the Docker image tag.
    /// </summary>
    [PublicAPI]
    [NotNull]
    string Tag { get; }

    /// <summary>
    /// Gets the full Docker image name.
    /// </summary>
    /// <remarks>
    /// Full Docker image name, like "foo/bar:1.0.0" or "bar:latest" based on the components values.
    /// </remarks>
    [PublicAPI]
    [NotNull]
    string FullName { get; }

    /// <summary>
    /// Gets the Docker registry hostname.
    /// </summary>
    /// <returns>The Docker registry hostname.</returns>
    [PublicAPI]
    [CanBeNull]
    string GetHostname();
  }
}
