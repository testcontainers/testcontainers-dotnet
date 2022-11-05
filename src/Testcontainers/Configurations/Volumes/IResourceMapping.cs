namespace DotNet.Testcontainers.Configurations
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// This class represents a read-only filesystem resource mapping.
  /// </summary>
  public interface IResourceMapping : IMount
  {
    /// <summary>
    /// Gets the byte array content of the resource mapping.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the byte array content of the resource mapping has been read.</returns>
    Task<byte[]> GetAllBytesAsync(CancellationToken ct = default);
  }
}
