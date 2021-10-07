namespace DotNet.Testcontainers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// This class represents a filesystem mount.
  /// </summary>
  public interface IMount
  {
    /// <summary>
    /// Gets the absolute path of a file or directory to mount in the container.
    /// </summary>
    [NotNull]
    string ContainerPath { get; }

    /// <summary>
    /// Gets the Docker mount access mode.
    /// </summary>
    AccessMode AccessMode { get; }
  }
}
