namespace DotNet.Testcontainers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// Indicates the file system for file operations.
  /// </summary>
  [PublicAPI]
  public enum FileSystem
  {
    /// <summary>
    /// The test host file system.
    /// </summary>
    Host = 0,

    /// <summary>
    /// The container file system.
    /// </summary>
    Container = 1,
  }
}
