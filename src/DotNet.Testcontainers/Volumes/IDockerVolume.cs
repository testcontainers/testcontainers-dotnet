namespace DotNet.Testcontainers.Volumes
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// A Docker volume.
  /// </summary>
  public interface IDockerVolume
  {
    /// <summary>
    /// Gets the Docker volume name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Creates the volume.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the network has been created.</returns>
    Task CreateAsync(CancellationToken ct = default);

    /// <summary>
    /// Removes the volume.
    /// </summary>
    /// <param name="force">Force the removal of the volume.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the network has been deleted.</returns>
    Task RemoveAsync(bool? force = null, CancellationToken ct = default);
  }
}
