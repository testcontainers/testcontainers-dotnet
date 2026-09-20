namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;

  /// <summary>
  /// This class represents the BuildKit image builder.
  /// </summary>
  internal interface IBuildKitImageOperations
  {
    /// <summary>
    /// Builds a Docker image from a Dockerfile with BuildKit.
    /// </summary>
    /// <param name="configuration">The Dockerfile configuration.</param>
    /// <param name="dockerfileArchive">The tar archive that contains the build context.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the Docker image has been built.</returns>
    Task<string> BuildAsync(IBuildKitImageFromDockerfileConfiguration configuration, ITarArchive dockerfileArchive, CancellationToken ct = default);
  }
}
