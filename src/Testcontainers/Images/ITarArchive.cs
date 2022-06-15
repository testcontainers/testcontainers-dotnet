namespace DotNet.Testcontainers.Images
{
  /// <summary>
  /// Collects files into one tar archive file.
  /// </summary>
  internal interface ITarArchive
  {
    /// <summary>
    /// Creates a tar archive file.
    /// </summary>
    /// <returns>Path to the created archive file.</returns>
    string Tar();
  }
}
