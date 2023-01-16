namespace DotNet.Testcontainers.Networks
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using JetBrains.Annotations;

  /// <summary>
  /// A network instance.
  /// </summary>
  [PublicAPI]
  public interface INetwork : IDockerNetwork, IFutureResource
  {
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <exception cref="InvalidOperationException">Network has not been created.</exception>
    [NotNull]
    new string Name { get; }

    /// <summary>
    /// Creates the network.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the network has been created.</returns>
    new Task CreateAsync(CancellationToken ct = default);

    /// <summary>
    /// Deletes the network.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the network has been deleted.</returns>
    new Task DeleteAsync(CancellationToken ct = default);
  }
}
