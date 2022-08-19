namespace DotNet.Testcontainers.Networks
{
  using System.Threading;
  using System.Threading.Tasks;
  using JetBrains.Annotations;

  /// <summary>
  /// A Docker network.
  /// </summary>
  [PublicAPI]
  public interface IDockerNetwork
  {
    /// <summary>
    /// Gets the Docker network id.
    /// </summary>
    [PublicAPI]
    string Id { get; }

    /// <summary>
    /// Gets the Docker network name.
    /// </summary>
    [PublicAPI]
    string Name { get; }

    /// <summary>
    /// Creates the network.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the network has been created.</returns>
    [PublicAPI]
    Task CreateAsync(CancellationToken ct = default);

    /// <summary>
    /// Deletes the network.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the network has been deleted.</returns>
    [PublicAPI]
    Task DeleteAsync(CancellationToken ct = default);
  }
}
