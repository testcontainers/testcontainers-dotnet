namespace DotNet.Testcontainers.Images
{
  using DotNet.Testcontainers.Configurations;

  /// <summary>
  /// Provides extension methods for the <see cref="IImage" /> interface.
  /// </summary>
  internal static class IImageExtensions
  {
    /// <summary>
    /// Applies the Docker Hub image name prefix if it is configured.
    /// </summary>
    /// <param name="image">The original <see cref="IImage" /> instance.</param>
    /// <returns>
    /// A new <see cref="IImage" /> instance with the Docker Hub image name prefix
    /// applied, or the original instance if no prefix is set.
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

      return new DockerImage(image.Repository, image.Registry, image.Tag, image.Digest, TestcontainersSettings.HubImageNamePrefix);
    }
  }
}
