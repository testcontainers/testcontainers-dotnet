namespace DotNet.Testcontainers.Networks
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// A Docker network.
  /// </summary>
  public interface IDockerNetwork
  {
    /// <summary>
    /// Gets the id.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Creates the network.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the network has been created.</returns>
    Task CreateAsync(CancellationToken ct = default);

    /// <summary>
    /// Deletes the network.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the network has been deleted.</returns>
    Task DeleteAsync(CancellationToken ct = default);
  }
}
