namespace DotNet.Testcontainers.Images
{
  using System;
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
    /// Gets the registry.
    /// </summary>
    [CanBeNull]
    string Registry { get; }

    /// <summary>
    /// Gets the tag.
    /// </summary>
    [CanBeNull]
    string Tag { get; }

    /// <summary>
    /// Gets the digest.
    /// </summary>
    [CanBeNull]
    string Digest { get; }

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

    /// <summary>
    /// Checks if the tag matches either the latest or nightly tag.
    /// </summary>
    /// <returns>True if the tag matches the latest or nightly tag, otherwise false.</returns>
    bool MatchLatestOrNightly();

    /// <summary>
    /// Checks if the tag matches the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match the tag against.</param>
    /// <returns>True if the tag matches the predicate, otherwise false.</returns>
    bool MatchVersion(Predicate<string> predicate);

    /// <summary>
    /// Checks if the tag matches the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match the tag against.</param>
    /// <returns>True if the tag matches the predicate, otherwise false.</returns>
    bool MatchVersion(Predicate<Version> predicate);
  }
}
