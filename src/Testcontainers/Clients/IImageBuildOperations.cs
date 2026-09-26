namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;

  /// <summary>
  /// A Docker image builder.
  /// </summary>
  /// <typeparam name="TConfiguration">The Dockerfile configuration type.</typeparam>
  internal interface IImageBuildOperations<in TConfiguration>
    where TConfiguration : IImageFromDockerfileConfiguration
  {
    /// <summary>
    /// Gets the image build parameters of the Docker image build.
    /// </summary>
    /// <param name="configuration">The Dockerfile configuration.</param>
    /// <returns>The image build parameters of the Docker image build.</returns>
    ImageBuildParameters GetBuildParameters(TConfiguration configuration);

    /// <summary>
    /// Builds a Docker image from a Dockerfile.
    /// </summary>
    /// <param name="configuration">The Dockerfile configuration.</param>
    /// <param name="buildParameters">The image build parameters.</param>
    /// <param name="dockerfileArchive">The tar archive that contains the build context.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the Docker image has been built.</returns>
    Task<string> BuildAsync(TConfiguration configuration, ImageBuildParameters buildParameters, ITarArchive dockerfileArchive, CancellationToken ct = default);
  }
}
