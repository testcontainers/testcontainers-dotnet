namespace DotNet.Testcontainers.Volumes
{
  using System.Threading;
  using System.Threading.Tasks;
  using JetBrains.Annotations;

  /// <summary>
  /// A Docker volume.
  /// </summary>
  [PublicAPI]
  public interface IDockerVolume
  {
    /// <summary>
    /// Gets the Docker volume name.
    /// </summary>
    [PublicAPI]
    string Name { get; }

    /// <summary>
    /// Creates the volume.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the volume has been created.</returns>
    [PublicAPI]
    Task CreateAsync(CancellationToken ct = default);

    /// <summary>
    /// Delete the volume.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the volume has been deleted.</returns>
    [PublicAPI]
    Task DeleteAsync(CancellationToken ct = default);
  }
}
