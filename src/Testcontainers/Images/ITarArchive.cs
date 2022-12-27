namespace DotNet.Testcontainers.Images
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// Collects files into one tar archive file.
  /// </summary>
  internal interface ITarArchive
  {
    /// <summary>
    /// Creates a tar archive file.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the tar archive file has been created.</returns>
    Task<string> Tar(CancellationToken ct = default);
  }
}
