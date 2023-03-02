namespace DotNet.Testcontainers.Images
{
  using JetBrains.Annotations;

  /// <summary>
  /// An image instance.
  /// </summary>
  [PublicAPI]
  public interface IImage
  {
    /// <summary>
    /// Gets the repository.
    /// </summary>
    [NotNull]
    string Repository { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    [NotNull]
    string Name { get; }

    /// <summary>
    /// Gets the tag.
    /// </summary>
    [NotNull]
    string Tag { get; }

    /// <summary>
    /// Gets the full image name.
    /// </summary>
    /// <remarks>
    /// The full image name, like "foo/bar:1.0.0" or "bar:latest" based on the components values.
    /// </remarks>
    [NotNull]
    string FullName { get; }

    /// <summary>
    /// Gets the registry hostname.
    /// </summary>
    /// <returns>The registry hostname.</returns>
    [CanBeNull]
    string GetHostname();
  }
}
