namespace DotNet.Testcontainers.Volumes
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using JetBrains.Annotations;

  /// <summary>
  /// A volume instance.
  /// </summary>
  [PublicAPI]
  public interface IVolume : IDockerVolume
  {
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <exception cref="InvalidOperationException">Volume has not been created.</exception>
    [NotNull]
    new string Name { get; }

    /// <summary>
    /// Creates the volume.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the volume has been created.</returns>
    new Task CreateAsync(CancellationToken ct = default);

    /// <summary>
    /// Delete the volume.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the volume has been deleted.</returns>
    new Task DeleteAsync(CancellationToken ct = default);
  }
}
