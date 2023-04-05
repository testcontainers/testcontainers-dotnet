namespace DotNet.Testcontainers
{
  using System.Threading;
  using System.Threading.Tasks;
  using JetBrains.Annotations;

  /// <summary>
  /// A future resource instance.
  /// </summary>
  [PublicAPI]
  public interface IFutureResource
  {
    /// <summary>
    /// Creates the resource.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the resource has been created.</returns>
    Task CreateAsync(CancellationToken ct = default);

    /// <summary>
    /// Deletes the resource.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the resource has been deleted.</returns>
    Task DeleteAsync(CancellationToken ct = default);
  }
}
