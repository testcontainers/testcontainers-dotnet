namespace DotNet.Testcontainers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// This class represents a filesystem mount.
  /// </summary>
  public interface IMount
  {
    MountType Type { get; }

    /// <summary>
    /// Gets the Docker mount access mode.
    /// </summary>
    AccessMode AccessMode { get; }

    [NotNull]
    string Source { get; }

    /// <summary>
    /// Gets the absolute path of a file or directory to mount in the container.
    /// </summary>
    [NotNull]
    string Target { get; }
  }
}
