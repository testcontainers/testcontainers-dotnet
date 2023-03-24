namespace DotNet.Testcontainers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// This class represents a filesystem mount.
  /// </summary>
  [PublicAPI]
  public interface IMount : IFutureResource
  {
    /// <summary>
    /// Gets the Docker mount type.
    /// </summary>
    MountType Type { get; }

    /// <summary>
    /// Gets the Docker mount access mode.
    /// </summary>
    AccessMode AccessMode { get; }

    /// <summary>
    /// Gets the absolute path of a file, a directory or volume to mount on the host system.
    /// </summary>
    [NotNull]
    string Source { get; }

    /// <summary>
    /// Gets the absolute path of a file or directory to mount in the container.
    /// </summary>
    [NotNull]
    string Target { get; }
  }
}
