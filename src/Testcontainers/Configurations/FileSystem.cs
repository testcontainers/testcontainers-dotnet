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
    /// The file system of the host machine.
    /// </summary>
    Host = 0,

    /// <summary>
    /// The container file system.
    /// </summary>
    Container = 1,
  }
}
