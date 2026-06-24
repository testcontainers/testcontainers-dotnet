namespace DotNet.Testcontainers.Images
{
  using DotNet.Testcontainers.Configurations;

  /// <summary>
  /// Provides extension methods for the <see cref="IImage" /> interface.
  /// </summary>
  internal static class IImageExtensions
  {
    /// <summary>
    /// Applies the configured image name substitution and then the Docker Hub image name prefix.
    /// </summary>
    /// <param name="image">The original <see cref="IImage" /> instance.</param>
    /// <returns>
    /// A new <see cref="IImage" /> instance with the configured substitution and Docker Hub image
    /// name prefix applied, or the original instance if neither is configured.
    /// </returns>
    public static IImage ApplyImageNameSubstitution(this IImage image)
    {
      var substitution = TestcontainersSettings.ImageNameSubstitution;
      var substitute = substitution == null ? image : substitution(image) ?? image;
      return substitute.ApplyHubImageNamePrefix();
    }

    /// <summary>
    /// Applies the configured Docker Hub image name prefix.
    /// </summary>
    /// <param name="image">The original <see cref="IImage" /> instance.</param>
    /// <returns>
    /// A new <see cref="IImage" /> instance with the configured Docker Hub image name prefix
    /// applied, or the original instance if no prefix is configured.
    /// </returns>
    public static IImage ApplyHubImageNamePrefix(this IImage image)
    {
      if (string.IsNullOrEmpty(TestcontainersSettings.HubImageNamePrefix))
      {
        return image;
      }

      if (!string.IsNullOrEmpty(image.GetHostname()))
      {
        return image;
      }

      return new DockerImage(image.Repository, image.Registry, image.Tag, image.Digest, image.Platform, TestcontainersSettings.HubImageNamePrefix);
    }
  }
}
